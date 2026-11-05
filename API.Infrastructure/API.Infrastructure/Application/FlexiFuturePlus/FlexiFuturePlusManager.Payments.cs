using System.Data;
using DAL.Model;
using DAL.Model.FlexiContributions;
using DAL.Model.FlexiFuture;
using DAL.Model.Pensioner;
using DAL.ModelView.FlexiFuturePlus;
using DAL.ModelView.Pension;
using DAL.ModelView.Settings;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using FlexiPaymentMode = DAL.ModelView.FlexiFuturePlus.PaymentMode;
using Newtonsoft.Json;

namespace API.Infrastructure.Application.FlexiFuturePlus;

public partial class FlexiFuturePlusManager
{
    private const double MinimumContribution = 1d;
    private const int MaxPaymentInstructionBytes = 20 * 1024 * 1024;

    private static readonly HashSet<string> AllowedInstructionExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".webp",
        ".pdf", ".doc", ".docx"
    };

    private static readonly Dictionary<string, string> InstructionMimeTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        [".pdf"] = "application/pdf",
        [".jpg"] = "image/jpeg",
        [".jpeg"] = "image/jpeg",
        [".png"] = "image/png",
        [".gif"] = "image/gif",
        [".bmp"] = "image/bmp",
        [".tiff"] = "image/tiff",
        [".webp"] = "image/webp",
        [".doc"] = "application/msword",
        [".docx"] = "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
    };

    private async Task<FlexiFuturePlusResponse<FlexiFuturePlusPaymentResultDto>> AddPaymentInternalAsync(
        FlexiFuturePlusAddPaymentRequestDto request,
        PartnerContext partner,
        string? ipAddress,
        string? browser,
        CancellationToken cancellationToken)
    {
        var response = new FlexiFuturePlusResponse<FlexiFuturePlusPaymentResultDto>();
        try
        {
            var validationMessage = ValidatePaymentRequest(request);
            if (validationMessage != null)
            {
                response.Success = false;
                response.Message = validationMessage;
                return response;
            }

            var quote = await ResolvePaymentQuoteAsync(request, cancellationToken)
                ?? throw new InvalidOperationException("Quote was not found.");

            if (!string.IsNullOrWhiteSpace(quote.PartnerCode)
                && !string.Equals(quote.PartnerCode, partner.PartnerCode, StringComparison.OrdinalIgnoreCase))
            {
                response.Success = false;
                response.Message = "You do not have access to this quote.";
                return response;
            }

            var policy = await _akiba.FlexiFuturePolicies
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.QuoteId == quote.Id, cancellationToken)
                ?? throw new InvalidOperationException("FlexiFuture policy was not found for this quote.");

            var customer = await _akiba.customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == policy.CustomerId, cancellationToken)
                ?? throw new InvalidOperationException($"Customer '{policy.CustomerId}' was not found.");

            var flexiFundId = await ResolveFlexiFundIdAsync(policy.Id, cancellationToken);
            if (request.PaymentMode is FlexiPaymentMode.Checkoff or FlexiPaymentMode.Ratiba)
            {
                return await SavePaymentInstructionAsync(
                    request,
                    policy,
                    flexiFundId,
                    ipAddress,
                    browser,
                    cancellationToken);
            }

            var isMpesa = request.PaymentMode == FlexiPaymentMode.Mpesa;
            var suppliedReference = string.IsNullOrWhiteSpace(request.PaymentReference)
                ? null
                : request.PaymentReference.Trim();
            if (suppliedReference != null)
            {
                var duplicateReferenceMessage = await ValidatePaymentReferenceUniqueAsync(
                    suppliedReference,
                    cancellationToken);
                if (duplicateReferenceMessage != null)
                {
                    response.Success = false;
                    response.Message = duplicateReferenceMessage;
                    return response;
                }
            }

            var paymentReference = isMpesa ? null : suppliedReference;

            var contributionId = Guid.NewGuid();
            var now = DateTimeOffset.UtcNow;
            var contribution = new FlexiContribution
            {
                Id = contributionId,
                CustomerId = policy.CustomerId,
                Total_Contribution = request.Amount,
                Naration = request.Narration,
                Reference = paymentReference,
                ThirdpartyRef = paymentReference,
                FlexiFuturePolicyId = policy.Id,
                FlexiFundId = flexiFundId,
                BankReference = request.PaymentMode == FlexiPaymentMode.Bank ? paymentReference : null,
                PaymentMode = request.PaymentMode,
                PaymentStatus = PaymentStatus.Pending,
                PaymentAcknowledged = false,
                Approved = false,
                FullName = customer.Fullname,
                Idnumber = customer.Idnumber,
                Created = now,
                CreatedBy = policy.CustomerId.ToString(),
                CreatedFromIP = Truncate(ipAddress, 100),
                CreatedFromBrowser = Truncate(browser, 1000)
            };

            _akiba.FlexiContributions.Add(contribution);
            await _akiba.SaveChangesAsync(cancellationToken);

            var result = new FlexiFuturePlusPaymentResultDto
            {
                ContributionId = contribution.Id,
                RecordType = FlexiContributionRecordTypes.FlexiContribution,
                PaymentMode = request.PaymentMode,
                PaymentStatus = PaymentStatus.Pending.ToString()
            };

            if (request.PaymentMode != FlexiPaymentMode.Mpesa)
            {
                response.Success = true;
                response.Message = "Contribution recorded successfully.";
                response.Data = result;
                return response;
            }

            var msisdn = NormalizeMsisdn(request.PhoneNumber ?? customer.PhoneNumber);
            if (msisdn == null)
            {
                const string phoneMessage = "A valid PhoneNumber is required for M-Pesa payments.";
                await RejectUnsentMpesaContributionAsync(contribution, phoneMessage, null, cancellationToken);
                result.PaymentStatus = PaymentStatus.Rejected.ToString();
                response.Success = false;
                response.Message = phoneMessage;
                response.Data = result;
                return response;
            }

            var memberNo = FirstNonBlank(customer.MemberNumber, policy.PolicyNo, customer.Idnumber, customer.Id.ToString());
            DAL.ModelView.ResponseDTO stk;
            try
            {
                stk = await _ipay.ProcessSTK(
                    new STKContributionDTO
                    {
                        MemberNo = memberNo,
                        PensionerId = customer.Id.ToString(),
                        Amount = request.Amount,
                        Phonenumber = msisdn,
                        TrnCode = contribution.Id.ToString(),
                        AccountType = CustomerType.Individual,
                        ProcessBatch = false
                    },
                    EndPointType.Mpesa_STK_FlexiFuture_CallbackUrl);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "FlexiFuture STK push was not triggered for contribution {ContributionId}",
                    contribution.Id);
                const string stkMessage = "The M-Pesa prompt could not be sent. Please try again.";
                await RejectUnsentMpesaContributionAsync(contribution, stkMessage, null, cancellationToken);
                result.PaymentStatus = PaymentStatus.Rejected.ToString();
                response.Success = false;
                response.Message = stkMessage;
                response.Data = result;
                return response;
            }

            result.StkTransactionId = stk.ProductRef;

            if (!stk.Success)
            {
                var stkMessage = string.IsNullOrWhiteSpace(stk.ErrorMsg)
                    ? "The M-Pesa prompt could not be sent. Please try again."
                    : stk.ErrorMsg;
                _logger.LogWarning(
                    "FlexiFuture STK push was not triggered for contribution {ContributionId}: {Message}",
                    contribution.Id,
                    stkMessage);
                await RejectUnsentMpesaContributionAsync(
                    contribution,
                    stkMessage,
                    stk.ProductRef,
                    cancellationToken);
                result.PaymentStatus = PaymentStatus.Rejected.ToString();
                response.Success = false;
                response.Message = stkMessage;
                response.Data = result;
                return response;
            }

            if (!string.IsNullOrWhiteSpace(request.CallbackUrl) && !string.IsNullOrWhiteSpace(stk.ProductRef))
            {
                var callback = new CallBackResponse
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.Now,
                    CallbackUrl = request.CallbackUrl,
                    PartnerCode = partner.PartnerCode,
                    PartnerId = partner.PartnerId.ToString(),
                    Productenum = Productenum.flexifutureplus,
                    RequestType = "FlexiFutureContribution",
                    Request = JsonConvert.SerializeObject(request),
                    Response = string.Empty,
                    CorrelationId = stk.ProductRef,
                    RequestId = suppliedReference ?? contribution.Id.ToString(),
                    CallbackProcessed = false,
                    ProcessResponse = false,
                    Responded = false,
                    Status = "Pending",
                    StatusMessage = "Pending callback dispatch."
                };
                _db.callBackResponse.Add(callback);
                await _db.SaveChangesAsync(cancellationToken);
                result.CorrelationId = callback.CorrelationId;
            }

            response.Success = true;
            response.Message = $"An M-Pesa prompt has been sent to {msisdn}. Enter your PIN to complete the payment.";
            response.Data = result;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = FlexiFutureErrorHandling.ToClientMessage(ex, _logger, nameof(AddPaymentInternalAsync));
        }

        return response;
    }

    private async Task<FlexiFuturePlusResponse<FlexiFuturePlusPaymentStatusDto>> GetPaymentStatusInternalAsync(
        FlexiFuturePlusPaymentStatusRequestDto request,
        PartnerContext partner,
        CancellationToken cancellationToken)
    {
        var response = new FlexiFuturePlusResponse<FlexiFuturePlusPaymentStatusDto>();
        try
        {
            if (request.ContributionId == Guid.Empty)
            {
                response.Success = false;
                response.StatusCode = 400;
                response.Message = "contributionId is required.";
                return response;
            }

            if ((!request.QuoteId.HasValue || request.QuoteId.Value == Guid.Empty)
                && string.IsNullOrWhiteSpace(request.QuoteNumber))
            {
                response.Success = false;
                response.StatusCode = 400;
                response.Message = "Provide quoteId or quoteNumber.";
                return response;
            }

            var quote = await ResolvePaymentQuoteAsync(
                new FlexiFuturePlusAddPaymentRequestDto
                {
                    QuoteId = request.QuoteId,
                    QuoteNumber = request.QuoteNumber
                },
                cancellationToken);

            if (quote == null)
            {
                response.Success = false;
                response.StatusCode = 404;
                response.Message = "Quote was not found.";
                return response;
            }

            if (!string.IsNullOrWhiteSpace(quote.PartnerCode)
                && !string.Equals(quote.PartnerCode, partner.PartnerCode, StringComparison.OrdinalIgnoreCase))
            {
                response.Success = false;
                response.StatusCode = 400;
                response.Message = "You do not have access to this quote.";
                return response;
            }

            var policy = await _akiba.FlexiFuturePolicies
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.QuoteId == quote.Id, cancellationToken);

            if (policy == null)
            {
                response.Success = false;
                response.StatusCode = 404;
                response.Message = "Payment was not found for this quote.";
                return response;
            }

            var contribution = await _akiba.FlexiContributions
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    c => c.Id == request.ContributionId
                         && c.FlexiFuturePolicyId == policy.Id
                         && c.DeletedBy == null,
                    cancellationToken);

            if (contribution != null)
            {
                response.Success = true;
                response.Message = "Payment status retrieved successfully.";
                response.Data = new FlexiFuturePlusPaymentStatusDto
                {
                    ContributionId = contribution.Id,
                    RecordType = FlexiContributionRecordTypes.FlexiContribution,
                    PaymentMode = contribution.PaymentMode,
                    PaymentStatus = (contribution.PaymentStatus ?? PaymentStatus.Pending).ToString(),
                    Amount = contribution.Total_Contribution,
                    QuoteId = quote.Id,
                    QuoteNumber = quote.QuoteNumber,
                    RejectedReason = contribution.RejectedReason
                };
                return response;
            }

            var instruction = await _akiba.PaymentInstructions
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    p => p.Id == request.ContributionId
                         && p.FlexiFuturePolicyId == policy.Id
                         && p.DeletedBy == null,
                    cancellationToken);

            if (instruction == null)
            {
                response.Success = false;
                response.StatusCode = 404;
                response.Message = "Payment was not found for this quote.";
                return response;
            }

            response.Success = true;
            response.Message = "Payment status retrieved successfully.";
            response.Data = new FlexiFuturePlusPaymentStatusDto
            {
                PaymentInstructionId = instruction.Id,
                RecordType = FlexiContributionRecordTypes.PaymentInstruction,
                PaymentMode = instruction.FlexiContributionType,
                PaymentStatus = (instruction.PaymentStatus ?? PaymentStatus.Pending).ToString(),
                ProcessingStatus = instruction.ProcessingStatus.ToString(),
                Amount = instruction.Total_Contribution,
                QuoteId = quote.Id,
                QuoteNumber = quote.QuoteNumber,
                RejectedReason = instruction.RejectedReason
            };
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.StatusCode = 400;
            response.Message = FlexiFutureErrorHandling.ToClientMessage(ex, _logger, nameof(GetPaymentStatusInternalAsync));
        }

        return response;
    }

    /// <summary>
    /// The contribution was saved before the STK call. If Safaricom never sent the prompt,
    /// leave it Rejected so it is not treated as a payment waiting for a PIN.
    /// </summary>
    private async Task RejectUnsentMpesaContributionAsync(
        FlexiContribution contribution,
        string reason,
        string? stkTransactionId,
        CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        contribution.PaymentStatus = PaymentStatus.Rejected;
        contribution.Approved = false;
        contribution.PaymentAcknowledged = false;
        contribution.RejectedReason = reason;
        contribution.RejectedBy = "SYSTEM";
        contribution.RejectedDate = now.UtcDateTime;
        contribution.LastModified = now;
        contribution.LastModifiedBy = "SYSTEM";
        await _akiba.SaveChangesAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(stkTransactionId))
            return;

        if (_akiba.Connection.State != ConnectionState.Open)
            _akiba.Connection.Open();

        await _akiba.Connection.ExecuteAsync(
            """
            UPDATE dbo.sTKPushMpesaTransactions
               SET Finalized = '1',
                   Processed = '0',
                   ResultCode = '1',
                   ResultDesc = @Reason,
                   ResultOn = GETDATE()
             WHERE CONVERT(nvarchar(50), Id) = @Id
               AND ISNULL(Finalized, '0') IN ('0', 'false', 'False')
            """,
            new { Reason = reason, Id = stkTransactionId });
    }

    private async Task<FlexiFutureQuote?> ResolvePaymentQuoteAsync(
        FlexiFuturePlusAddPaymentRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (request.QuoteId.HasValue && request.QuoteId.Value != Guid.Empty)
            {
                return await _akiba.FlexiFutureQuotes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(q => q.Id == request.QuoteId.Value, cancellationToken);
            }

            if (!string.IsNullOrWhiteSpace(request.QuoteNumber))
            {
                var quoteNumber = request.QuoteNumber.Trim();
                return await _akiba.FlexiFutureQuotes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(q => q.QuoteNumber == quoteNumber, cancellationToken);
            }

            return null;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to resolve quote for payment.", ex);
        }
    }

    private async Task<string?> ValidatePaymentReferenceUniqueAsync(
        string? paymentReference,
        CancellationToken cancellationToken)
    {
        try
        {
            var normalizedReference = paymentReference?.Trim();
            if (string.IsNullOrWhiteSpace(normalizedReference))
                return null;

            var contributionExists = await _akiba.FlexiContributions
                .AsNoTracking()
                .AnyAsync(
                    c =>
                        (c.Reference != null && c.Reference == normalizedReference)
                        || (c.BankReference != null && c.BankReference == normalizedReference)
                        || (c.ThirdpartyRef != null && c.ThirdpartyRef == normalizedReference),
                    cancellationToken);

            if (contributionExists)
                return "A contribution with this paymentReference already exists.";

            var callbackExists = await _db.callBackResponse
                .AsNoTracking()
                .AnyAsync(c => c.RequestId == normalizedReference, cancellationToken);

            if (callbackExists)
                return "A payment with this paymentReference has already been processed.";

            return null;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to validate payment reference.", ex);
        }
    }

    private static string? ValidatePaymentRequest(FlexiFuturePlusAddPaymentRequestDto request)
    {
        if ((!request.QuoteId.HasValue || request.QuoteId.Value == Guid.Empty)
            && string.IsNullOrWhiteSpace(request.QuoteNumber))
        {
            return "Provide quoteId or quoteNumber.";
        }

        if (request.PaymentMode is not (FlexiPaymentMode.Mpesa or FlexiPaymentMode.Checkoff or FlexiPaymentMode.Ratiba)
            && string.IsNullOrWhiteSpace(request.PaymentReference))
        {
            return "paymentReference is required.";
        }

        if (request.Amount < MinimumContribution)
            return $"Amount must be at least {MinimumContribution:0}.";

        if (request.PaymentMode == FlexiPaymentMode.Mpesa)
        {
            if (string.IsNullOrWhiteSpace(request.PhoneNumber))
                return "PhoneNumber is required for M-Pesa payments.";

            if (NormalizeMsisdn(request.PhoneNumber) == null)
                return "PhoneNumber must be a valid mobile number.";
        }

        if (request.PaymentMode == FlexiPaymentMode.Checkoff)
        {
            var instructionError = ValidatePaymentInstruction(request.PaymentInstruction);
            if (instructionError != null)
                return instructionError;
        }

        return null;
    }

    private static string? ValidatePaymentInstruction(FileUploadDTO? instruction)
    {
        if (instruction == null
            || string.IsNullOrWhiteSpace(instruction.Data)
            || string.IsNullOrWhiteSpace(instruction.Extension))
        {
            return "A signed payment instruction with data and extension is required for Checkoff payments.";
        }

        var extension = NormalizeExtension(instruction.Extension);
        if (extension == null || !AllowedInstructionExtensions.Contains(extension))
            return "Payment instruction must be a PDF, Word, or image file.";

        return null;
    }

    /// <summary>
    /// Checkoff and Ratiba do not create FlexiContribution rows. Checkoff requires the
    /// signed mandate and stores a Submitted payment instruction. Ratiba stores an
    /// Initiated payment instruction without a document.
    /// </summary>
    private async Task<FlexiFuturePlusResponse<FlexiFuturePlusPaymentResultDto>> SavePaymentInstructionAsync(
        FlexiFuturePlusAddPaymentRequestDto request,
        FlexiFuturePolicy policy,
        Guid? flexiFundId,
        string? ipAddress,
        string? browser,
        CancellationToken cancellationToken)
    {
        var response = new FlexiFuturePlusResponse<FlexiFuturePlusPaymentResultDto>();
        var now = DateTimeOffset.UtcNow;
        string? uploadedFileName = null;
        string? uploadedFilePath = null;
        string? uploadedBase64 = null;
        var status = PaymentInstructionStatus.Initiated;

        if (request.PaymentMode == FlexiPaymentMode.Checkoff)
        {
            var saved = TrySavePaymentInstruction(request.PaymentInstruction);
            if (!saved.Success)
            {
                response.Success = false;
                response.Message = saved.Error
                    ?? "Payment instruction upload failed. Allowed formats are PDF, Word and image files.";
                return response;
            }

            uploadedFileName = saved.FileName;
            uploadedFilePath = saved.FilePath;
            uploadedBase64 = saved.DataUri;
            status = PaymentInstructionStatus.Submitted;
        }

        var instruction = await _akiba.PaymentInstructions
            .FirstOrDefaultAsync(
                p => p.CustomerId == policy.CustomerId
                     && p.FlexiFuturePolicyId == policy.Id
                     && p.FlexiContributionType == request.PaymentMode
                     && p.ProcessingStatus == PaymentInstructionStatus.Initiated
                     && p.DeletedBy == null,
                cancellationToken);

        if (instruction == null)
        {
            instruction = new PaymentInstruction
            {
                CustomerId = policy.CustomerId,
                FlexiFuturePolicyId = policy.Id,
                FlexiFundId = flexiFundId,
                FlexiContributionType = request.PaymentMode,
                ProcessingStatus = status,
                Total_Contribution = request.Amount,
                Naration = request.Narration,
                UploadDocumentsName = uploadedFileName,
                UploadDocumentsPath = uploadedFilePath,
                UploadDocumentsBase64 = uploadedBase64,
                PaymentAcknowledged = false,
                PaymentStatus = PaymentStatus.Pending,
                Created = now,
                CreatedBy = policy.CustomerId.ToString(),
                CreatedFromIP = Truncate(ipAddress, 100),
                CreatedFromBrowser = Truncate(browser, 1000)
            };
            _akiba.PaymentInstructions.Add(instruction);
        }
        else
        {
            instruction.Total_Contribution = request.Amount;
            instruction.Naration = request.Narration;
            instruction.FlexiFundId ??= flexiFundId;
            instruction.ProcessingStatus = status;
            instruction.UploadDocumentsName = uploadedFileName ?? instruction.UploadDocumentsName;
            instruction.UploadDocumentsPath = uploadedFilePath ?? instruction.UploadDocumentsPath;
            instruction.UploadDocumentsBase64 = uploadedBase64 ?? instruction.UploadDocumentsBase64;
            instruction.LastModified = now;
            instruction.LastModifiedBy = policy.CustomerId.ToString();
        }

        await _akiba.SaveChangesAsync(cancellationToken);

        response.Success = true;
        response.Message = request.PaymentMode == FlexiPaymentMode.Checkoff
            ? "Checkoff payment instruction recorded successfully."
            : "Ratiba payment instruction recorded successfully. No contribution was created.";
        response.Data = new FlexiFuturePlusPaymentResultDto
        {
            PaymentInstructionId = instruction.Id,
            RecordType = FlexiContributionRecordTypes.PaymentInstruction,
            PaymentMode = request.PaymentMode,
            PaymentStatus = PaymentStatus.Pending.ToString(),
            DocumentName = instruction.UploadDocumentsName
        };
        return response;
    }

    private (bool Success, string? FileName, string? FilePath, string? DataUri, string? Error) TrySavePaymentInstruction(
        FileUploadDTO? document)
    {
        try
        {
            var validationError = ValidatePaymentInstruction(document);
            if (validationError != null || document == null)
                return (false, null, null, null, validationError);

            var extension = NormalizeExtension(document.Extension)!;
            var payload = ExtractBase64Payload(document.Data);
            byte[] fileBytes;
            try
            {
                fileBytes = Convert.FromBase64String(payload);
            }
            catch (FormatException)
            {
                return (false, null, null, null, "Payment instruction data must be valid base64.");
            }

            if (fileBytes.Length == 0)
                return (false, null, null, null, "Payment instruction file is empty.");

            if (fileBytes.Length > MaxPaymentInstructionBytes)
                return (false, null, null, null, "Payment instruction must not exceed 20 MB.");

            var uploadsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Uploads");
            Directory.CreateDirectory(uploadsDir);

            var fileName = $"{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid():N}{extension}";
            var filePath = Path.Combine(uploadsDir, fileName);
            File.WriteAllBytes(filePath, fileBytes);

            return (true, fileName, filePath, ToInstructionDataUri(payload, extension), null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save a FlexiFuture payment instruction.");
            return (false, null, null, null, "Payment instruction upload failed. Please try again.");
        }
    }

    private static string? NormalizeExtension(string? extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
            return null;

        var trimmed = extension.Trim();
        if (trimmed.Contains('/') || trimmed.Contains('\\') || trimmed.Contains(".."))
            return null;

        return trimmed.StartsWith('.') ? trimmed.ToLowerInvariant() : "." + trimmed.ToLowerInvariant();
    }

    private static string ExtractBase64Payload(string data)
    {
        var payload = data.Trim();
        if (payload.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
        {
            var comma = payload.IndexOf(',');
            if (comma >= 0)
                payload = payload[(comma + 1)..];
        }

        return payload
            .Replace("\r", string.Empty, StringComparison.Ordinal)
            .Replace("\n", string.Empty, StringComparison.Ordinal)
            .Replace(" ", string.Empty, StringComparison.Ordinal);
    }

    private static string ToInstructionDataUri(string payload, string extension)
    {
        var mime = InstructionMimeTypes.TryGetValue(extension, out var mimeType)
            ? mimeType
            : "application/octet-stream";
        return $"data:{mime};base64,{payload}";
    }

    private async Task<Guid?> ResolveFlexiFundIdAsync(Guid policyId, CancellationToken cancellationToken)
    {
        var policyKey = policyId.ToString();
        var fundId = await _akiba.FlexiFunds
            .AsNoTracking()
            .Where(f => f.PolicyId == policyKey)
            .Select(f => (Guid?)f.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (fundId == null)
            _logger.LogWarning("No FlexiFund row for policy {PolicyId}; FlexiFundId left null.", policyId);

        return fundId;
    }

    private static string? NormalizeMsisdn(string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return null;

        var digits = new string(phoneNumber.Where(char.IsDigit).ToArray());
        return digits.Length < 9 ? null : digits;
    }

    private static string FirstNonBlank(params string?[] candidates) =>
        candidates.FirstOrDefault(c => !string.IsNullOrWhiteSpace(c)) ?? string.Empty;
}

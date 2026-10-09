using DAL.Model;
using DAL.Model.FlexiContributions;
using DAL.Model.FlexiFuture;
using DAL.Model.Pensioner;
using DAL.ModelView.FlexiFuturePlus;
using DAL.ModelView.Pension;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using FlexiPaymentMode = DAL.ModelView.FlexiFuturePlus.PaymentMode;
using Newtonsoft.Json;

namespace API.Infrastructure.Application.FlexiFuturePlus;

public partial class FlexiFuturePlusManager
{
    private const double MinimumContribution = 1d;

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

            var duplicateReferenceMessage = await ValidatePaymentReferenceUniqueAsync(
                request.PaymentReference,
                cancellationToken);
            if (duplicateReferenceMessage != null)
            {
                response.Success = false;
                response.Message = duplicateReferenceMessage;
                return response;
            }

            var paymentReference = request.PaymentReference!.Trim();
            var flexiFundId = await ResolveFlexiFundIdAsync(policy.Id, cancellationToken);
            var now = DateTimeOffset.UtcNow;
            var contribution = new FlexiContribution
            {
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
                response.Success = false;
                response.Message = "A valid PhoneNumber is required for M-Pesa payments.";
                return response;
            }

            var memberNo = FirstNonBlank(customer.MemberNumber, policy.PolicyNo, customer.Idnumber, customer.Id.ToString());
            var stk = await _ipay.ProcessSTK(
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

            result.StkTransactionId = stk.ProductRef;

            if (!stk.Success)
            {
                _logger.LogWarning(
                    "FlexiFuture STK push failed for contribution {ContributionId}: {Message}",
                    contribution.Id,
                    stk.ErrorMsg);
                response.Success = false;
                response.Message = string.IsNullOrWhiteSpace(stk.ErrorMsg)
                    ? "Contribution recorded, but the M-Pesa prompt could not be sent. Please try again."
                    : stk.ErrorMsg;
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
                    RequestId = paymentReference,
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

        if (string.IsNullOrWhiteSpace(request.PaymentReference))
            return "paymentReference is required.";

        if (request.Amount < MinimumContribution)
            return $"Amount must be at least {MinimumContribution:0}.";

        if (request.PaymentMode == FlexiPaymentMode.Mpesa)
        {
            if (string.IsNullOrWhiteSpace(request.PhoneNumber))
                return "PhoneNumber is required for M-Pesa payments.";

            if (NormalizeMsisdn(request.PhoneNumber) == null)
                return "PhoneNumber must be a valid mobile number.";
        }

        return null;
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

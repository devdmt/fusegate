using Azure.Core;
using DAL.Model;
using DAL.ModelView;
using DAL.ModelView.Flex;
using DAL.ModelView.Pension;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace API.Infrastructure.Application.Flex
{
    internal partial class FlexManager
    {

        public async Task<TransactionStatusDTO> GetContributionStatus(ContributionStatusRequest request)
        {
            var response = new TransactionStatusDTO();
            if (request == null)
            {
                return new TransactionStatusDTO
                {
                    Success = false,
                    ErrorMsg = "Request body is required."
                };
            }
            try
            {
                switch(request.ProductType)
                {
                    case Productenum.NSSF:
                        case Productenum.PRMF:
                        case Productenum.IPP:
                        response=await GetPensionTransactionStatus(request.TransactionReference,Convert.ToDouble(request.Amount));
                        break;
                    case Productenum.flex:
                        response = await GetFlexTransactionStatus(request.TransactionReference, request.Amount);
                        break;
                    default:
                        break;

                }
            }
            catch (Exception ex)
            {
                _isettings.LogRequests(ex.Message, "GetContributionStatus", Interface.RequestType.Error);
                return new TransactionStatusDTO
                {
                    Success = false,
                    ErrorMsg = "An error occurred while processing the request."
                };
            }
            return response;
        }

        public async Task<TransactionStatusDTO> GetPensionTransactionStatus(string paymentReference,double amount)
        {
            var transaction = await _akiba.pensionContributions.AsNoTracking()
                .FirstOrDefaultAsync(c => c.ThirdpartyRef == paymentReference && c.Total_Contribution == amount);
            if (transaction == null)
            {
                return new TransactionStatusDTO
                {
                    Success = false,
                    ErrorMsg = "Transaction not found."
                };
            }
            if (transaction.PaymentStatus == PaymentStatus.Rejected)
            {
                return new TransactionStatusDTO
                {
                    Success = true,
                    ErrorMsg = transaction.RejectedReason,
                    Approved = false,
                    TransactionId = transaction.Id.ToString(),
                    TransactionStatus = transaction.PaymentStatus.ToString(),
                };
            }else if (transaction.PaymentStatus == PaymentStatus.Approved)
            {
                return new TransactionStatusDTO
                {
                    Success = true,
                    ErrorMsg = string.Empty,
                    Approved = true,
                    TransactionId = transaction.ThirdpartyRef?.ToString(),
                    TransactionStatus = transaction.PaymentStatus.ToString(),
                    FinalizedTime = transaction.ApprovedDate.HasValue ? transaction.ApprovedDate.Value.ToString("o") : null,
                    PaymentGatewayReference = transaction.Reference
                };
            }
            else
            {
                return new TransactionStatusDTO
                {
                    Success = true,
                    ErrorMsg = transaction.Naration ,
                    Approved = false,
                    TransactionId = transaction.ThirdpartyRef?  .ToString(),
                    TransactionStatus = transaction.PaymentStatus.ToString(),
                     PaymentGatewayReference = transaction.Reference
                };
            }




            return new TransactionStatusDTO
            {
                Success = true,
                 ErrorMsg = transaction.RejectedReason

            };
        }

        public async Task<TransactionStatusDTO> GetFlexTransactionStatus(string paymentReference, double amount)
        {
            var amountDecimal = (decimal)amount;
            var transaction = await _db.Contributions.AsNoTracking()
                .FirstOrDefaultAsync(c => c.PaymentReference == paymentReference && c.Amount == amountDecimal);

            if (transaction == null)
            {
                return new TransactionStatusDTO
                {
                    Success = false,
                    ErrorMsg = "Transaction not found."
                };
            }
            if (transaction.PaymentStatus == PaymentStatus.Rejected)
            {
                return new TransactionStatusDTO
                {
                    Success = true,
                    ErrorMsg = transaction.FailedReason,
                    Approved = false,
                    TransactionId = transaction.Id.ToString(),
                    TransactionStatus = transaction.PaymentStatus.ToString(),
                };
            }
            else if (transaction.PaymentStatus == PaymentStatus.Approved)
            {
                return new TransactionStatusDTO
                {
                    Success = true,
                    ErrorMsg = string.Empty,
                    Approved = true,
                    TransactionId = transaction.Id.ToString(),
                    TransactionStatus = transaction.PaymentStatus.ToString(),
                    FinalizedTime = transaction.CompletedOn.HasValue ? transaction.CompletedOn.Value.ToString("o") : null,
                    PaymentGatewayReference = transaction.PaymentGatewayRef
                };
            }
            else
            {
                return new TransactionStatusDTO
                {
                    Success = true,
                    ErrorMsg = transaction.Narration??transaction.FailedReason,
                    Approved = false,
                    TransactionId = transaction.Id.ToString(),
                    TransactionStatus = (transaction.PaymentStatus ?? PaymentStatus.Pending).ToString(),
                    PaymentGatewayReference = transaction.PaymentGatewayRef
                };
            }
        }

        public async Task<ResponseDTO> Contribute(ContributeDTO dto,string PartnerCode)
        {
            var response = new ResponseDTO();
            try
            {
                if (dto == null)
                {
                    response.AddError(nameof(dto), "Request body is required.");
                    return response;
                }

                if (string.IsNullOrWhiteSpace(dto.MemberNo))
                    response.AddError(nameof(dto.MemberNo), "Member number is required.");
                if (string.IsNullOrWhiteSpace(PartnerCode))
                    response.AddError(nameof(PartnerCode), "Partner code is required.");
                if (string.IsNullOrWhiteSpace(dto.PaymentReference))
                    response.AddError(nameof(dto.PaymentReference), "Payment reference is required.");
                if (dto.Amount <= 0)
                    response.AddError(nameof(dto.Amount), "Amount must be greater than zero.");
                if (response.HasErrors)
                    return response;
                // Validate callback URL
                if (string.IsNullOrWhiteSpace(dto.CallbackUrl))
                {
                    response.AddError(nameof(dto.CallbackUrl), "Callback URL is required.");
                    return response;
                }
                else
                {
                    if (!Uri.TryCreate(dto.CallbackUrl, UriKind.Absolute, out Uri callbackUri)
                        || (callbackUri.Scheme != Uri.UriSchemeHttp && callbackUri.Scheme != Uri.UriSchemeHttps))
                    {
                        response.AddError(nameof(dto.CallbackUrl), "Callback URL is invalid. It must be a valid HTTP or HTTPS URL.");
                        return response;
                    }
                }
        

                var partner = await _db.GetPartnerAsync(PartnerCode);
                if (partner == null)
                {
                    response.AddError(nameof(PartnerCode), "Partner not found.");
                    return response;
                }

                var partnerIdStored = partner.Id.ToString();
               
 var customer = await _db.customers.AsNoTracking()
                    .FirstOrDefaultAsync(c => c.MemberNo == dto.MemberNo);
                if (customer == null)
                {
                    response.AddError("customer", "No customer found for this member number and partner.");
                    return response;
                }
                var customerProduct = await _db.customerProducts.AsNoTracking()
                    .FirstOrDefaultAsync(cp => cp.CustomerId == customer.Id && cp.Product == (int)dto.Product);
                if (customerProduct == null)
                {
                    response.AddError("customerProduct", "No product enrollment found for this customer.");
                    return response;
                }

                var dupContribution = await _db.Contributions
                    .AnyAsync(c => c.PaymentReference == dto.PaymentReference);
                var dupCallback = await _db.callBackResponse
                    .AnyAsync(c => c.RequestId == dto.PaymentReference);
                if (dupContribution || dupCallback)
                {
                    response.AddError(nameof(dto.PaymentReference), "This payment reference has already been used.");
                    return response;
                }

                var payPhone = string.IsNullOrWhiteSpace(dto.PhoneNumber)
                    ? customer.PhoneNumber
                    : dto.PhoneNumber.Trim();
                if (string.IsNullOrWhiteSpace(payPhone) || !IsValidPhoneNumber(payPhone))
                {
                    response.AddError(nameof(dto.PhoneNumber), "A valid phone number is required for M-Pesa STK.");
                    return response;
                }

                var contributionId = Guid.NewGuid();
                var contribution = new Contribution
                {
                    Id = contributionId,
                    CustomerId = customer.Id,
                    CustomerProductId = customerProduct.Id.ToString(),
                    Product = customerProduct.Product,
                    RefNo = customerProduct.RefNo,
                    Amount = dto.Amount,
                    MemberNo = dto.MemberNo,
                    PhoneNumber = payPhone.Length > 50 ? payPhone[..50] : payPhone,
                    PaymentReference = dto.PaymentReference.Length > 100 ? dto.PaymentReference[..100] : dto.PaymentReference,
                    Completed = false,
                    Processed = 0,
                    Acknowledged = false,
                    PartnerId = partnerIdStored.Length > 50 ? partnerIdStored[..50] : partnerIdStored,
                    PaymentMode = 0,
                    Narration = string.IsNullOrWhiteSpace(dto.Narration)
                        ? null
                        : (dto.Narration.Length > 500 ? dto.Narration[..500] : dto.Narration),
                    PaymentStatus = PaymentStatus.Pending
                };

                _db.Contributions.Add(contribution);
                await _db.SaveChangesAsync();
                
                var stk = await _ipay.ProcessSTK_Insure(new STKContributionDTO
                {
                    MemberNo = dto.MemberNo,
                    PensionerId = customer.Id,
                    Amount = (double)Math.Round(dto.Amount, 2), 
                    Phonenumber = payPhone,
                    TrnCode = contribution.Id.ToString(),
                    ProcessBatch = false
                }, EndPointType.Mpesa_STK_Insure_CallbackUrl);

                if (!stk.Success)
                {
                    contribution.FailedReason = string.IsNullOrEmpty(stk.ErrorMsg)
                        ? "STK request failed."
                        : (stk.ErrorMsg.Length > 500 ? stk.ErrorMsg[..500] : stk.ErrorMsg);
                    contribution.ErrorCode = "STK_FAILED";
                    await _db.SaveChangesAsync();

                    response.Success = false;
                    response.ErrorMsg = stk.ErrorMsg;
                    return response;
                }

                if (!string.IsNullOrEmpty(stk.ProductRef))
                {
                    contribution.ProductRef = stk.ProductRef.Length > 50 ? stk.ProductRef[..50] : stk.ProductRef;
                }

                var callback = new CallBackResponse
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.Now,
                    CallbackUrl = dto.CallbackUrl ?? string.Empty,
                    PartnerCode = partner.PartnerCode,
                    PartnerId = partnerIdStored,
                    RequestType = "FlexContribution",
                    Request = JsonConvert.SerializeObject(dto),
                    Response = string.Empty,
                    CorrelationId = stk.ProductRef ?? string.Empty,
                    RequestId = dto.PaymentReference,
                    CallbackProcessed = false,
                    ProcessResponse = false,
                    Responded = false,
                    Status = "Pending",
                    StatusMessage = "Pending callback dispatch."
                };
                _db.callBackResponse.Add(callback);
                await _db.SaveChangesAsync();

                response.Success = true;
                response.ErrorMsg = stk.ErrorMsg;
                response.ResponseId = callback.CorrelationId;
                response.ProductRef = stk.ProductRef;
                return response;
            }
            catch (Exception ex)
            {
                _isettings.LogRequests(ex.Message, "Contribute", Interface.RequestType.Error);
                response.AddError("contribution", ex.Message);
                return response;
            }
        }
    }
}

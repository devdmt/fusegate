using API.Infrastructure.Interface;
using Azure;
using DAL;
using DAL.Model;
using DAL.ModelView;
using DAL.ModelView.Pension;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace API.Infrastructure.Application.Pension
{
    internal partial class PensionManager
    {
        /// <summary>
        /// Initiates a balance request: validates customer and partner, generates OTP, stores request.
        /// OTP is sent via SMS/email elsewhere; this method does not return the OTP.
        /// </summary>
        /// <param name="request">Must include MemberNumber and PartnerCode.</param>
        /// <returns>RequestId and ExpiresAt on success; meaningful error on failure.</returns>
        public async Task<ResponseDTO<BalanceRequestResponse>> BalanceRequest(BalanceDTORequest request)
        {
            // Guards for null/empty MemberNumber and PartnerCode
            if (string.IsNullOrWhiteSpace(request?.MemberNumber))
            {
                return ResponseDTO<BalanceRequestResponse>.Fail("Member number is required.");
            }
            if (string.IsNullOrWhiteSpace(request?.PartnerCode))
            {
                return ResponseDTO<BalanceRequestResponse>.Fail("Partner code is required.");
            }

            var memberNumber = request.MemberNumber.Trim();
            var partnerCode = request.PartnerCode.Trim();

            try
            {
                // 1) Validate customer using MemberNumber (Akiba Customers use MemberNo)
                var customer = await _akiba.customers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.MemberNumber == memberNumber);

                if (customer == null)
                {
                    return ResponseDTO<BalanceRequestResponse>.Fail("Member number not found.", new[]
                    {
                        new ValidationErrorItem { Field = "MemberNumber", Message = "Member number not found." }
                    });
                }

                var customerId = customer.Id;// Guid.TryParse(customer.Id, out var parsedId) ? parsedId : Guid.Empty;
                if (customerId == Guid.Empty)
                {
                    _settings.LogRequests($"Invalid Customer.Id for MemberNo={memberNumber}", "BalanceRequest", RequestType.Error);
                    return ResponseDTO<BalanceRequestResponse>.Fail("Invalid customer record.");
                }

                // 2) Confirm partner exists and is mapped to pension product (prmf or ipp)
                var partnerProduct = await _db.GetPartnerProductsAsync(Productenum.ipp, partnerCode)
                    ?? await _db.GetPartnerProductsAsync(Productenum.prmf, partnerCode);

                if (partnerProduct == null)
                {
                    return ResponseDTO<BalanceRequestResponse>.Fail(
                        "Partner code is not authorized for pension services.",
                        new[] { new ValidationErrorItem { Field = "PartnerCode", Message = "Partner is not mapped to pension product." } });
                }

                // OTP config from app settings (defaults: length=6, expiry=5 min)
                var otpLength = _otpSettings.Length > 0 ? _otpSettings.Length : 6;
                var expiryMinutes = _otpSettings.ExpiryMinutes > 0 ? _otpSettings.ExpiryMinutes : 5;

                // 3) Generate 6-digit OTP using cryptographically secure RNG, zero-padded
                var otpCode = GenerateSecureOtp(otpLength);

                var nowUtc = DateTime.UtcNow;
                var expiresAt = nowUtc.AddMinutes(expiryMinutes);

                // 4) Invalidate previous unexpired requests for same MemberNumber + PartnerCode
                var existingRequests = await _akiba.pensionerBalanceRequests
                    .Where(r => r.MemberCode == memberNumber && r.PartnerCode == partnerCode && r.ExpireOn > nowUtc)
                    .ToListAsync();

                foreach (var existing in existingRequests)
                {
                    existing.ExpireOn = nowUtc;
                }

                if (existingRequests.Count > 0)
                {
                    await _akiba.SaveChangesAsync();
                }
                string productRef = Guid.NewGuid().ToString("N")[..24];
                // 5) Insert new PensionerBalanceRequest
                var balanceRequest = new PensionerBalanceRequest
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customerId,
                    ProductId =(int) request.ProductType,
                    MemberCode = memberNumber,
                    PartnerCode = partnerCode,
                    OtpCode = otpCode,
                    GeneratedOn = nowUtc,
                    ExpireOn = expiresAt,
                    RequestRef = productRef,
                    PartnerId = partnerCode,
                    AgentCode = request.AgentCode ?? string.Empty,
                    RequestCompleted = false,
                    RequestFailed = false,
                    RequestAuthorised = false,
                    
                };

                _akiba.pensionerBalanceRequests.Add(balanceRequest);
                await _akiba.SaveChangesAsync();


                // 6) Return response (no OTP in response; sent via SMS/email elsewhere)


                
                    var otp = new OTP
                    {
                        Id = Guid.NewGuid(),
                        CustomerId = customerId.ToString(),
                        Code = otpCode,
                        Message = $"Your OTP Code is {otpCode}, Do not share if you did not authorize the transaction",
                        Phonenumber=customer.PhoneNumber,
                         ProductRef=productRef,
                        notificationType = NotificationType.SMS,
                        CreatedAt = DateTime.UtcNow,
                        ExpiredAt = DateTime.UtcNow.AddMinutes(5),
                        IsUsed = false,
                        ISsent = false,
                        ISSMSSent = false,
                        IsEmailSent = false
                    };
                    _db.OTPs.Add(otp);
                  await _db.SaveChangesAsync();
                return ResponseDTO<BalanceRequestResponse>.Ok(new BalanceRequestResponse
                {
                    RequestId = balanceRequest.Id,
                     Message="You'll recieve an SMS to complete the request: expires in 5 minutes",
                      Success=true,
                    ExpiresAt = expiresAt
                });
            }
            catch (Exception ex)
            {
                _settings.LogRequests($"{ex.Message}|{ex.StackTrace}", "BalanceRequest", RequestType.Error);
                return ResponseDTO<BalanceRequestResponse>.Fail("An error occurred while processing your request.");
            }
        }
        public async Task<ResponseDTO<CompleteBalanceResponse>> CompleteBalanceRequest(ViewBalanceDTORequest request)
        {
            // This method would validate the OTP and return the balance if valid.
            // Implementation would include:
            var response = new ResponseDTO<CompleteBalanceResponse>();
            try
            {
            // Validate input
            if (request == null || string.IsNullOrWhiteSpace(request.UniqueRequestId) || string.IsNullOrWhiteSpace(request.OTPCode))
            {
                response.Success = false;
                response.AddError("view-balance","Invalid request payload.");
                return response;
            }

            // 1. Find the balance request by UniqueRequestId
            var balanceRequest = await _akiba.pensionerBalanceRequests
                .FirstOrDefaultAsync(x => x.Id.ToString() == request.UniqueRequestId);

            if (balanceRequest == null)
            {
                response.Success = false;
                response.AddError("view-balance", "Balance request not found.");
               
                return response;
            }

            // 2. Validate OTP (plain string match; if hashing used, hash input here)
            if (!string.Equals(balanceRequest.OtpCode, request.OTPCode))
            {
                response.Success = false;
                response.AddError("view-balance", "Invalid OTP code.");
              
                return response;
            }

            // 3. Is OTP expired?
            if (balanceRequest.ExpireOn < DateTime.UtcNow)
            {
                response.Success = false;
                response.AddError("view-balance", "OTP has expired.");
               
                return response;
            }

            // 4. Already used?
            if (balanceRequest.IsUsed || balanceRequest.UsedAt != null)
            {
                response.Success = false;
                response.AddError("view-balance", "OTP has already been used.");
              
                return response;
            }

            // 5. Guard for user/data integrity
            if (balanceRequest.CustomerId== Guid.Empty)
            {
                response.Success = false;
                response.AddError("view-balance", "Balance request is missing customer or product data.");
                
                return response;
            }

            // Mark as used
            balanceRequest.IsUsed = true;
            balanceRequest.UsedAt = DateTime.UtcNow;
            await _akiba.SaveChangesAsync();

            // 6. Fetch fund record for the user/product
            var pensionFund = await _akiba.PensionerFund
                .FirstOrDefaultAsync(f =>
                    f.CustomerId == balanceRequest.CustomerId &&
                    (int)f.ProductTypes == balanceRequest.ProductId);

            if (pensionFund == null)
            {
                response.Success = false;
                response.AddError("view-balance", "Pension fund record not found for this customer/product.");
                
                return response;
            }

            // 7. Map PensionerFund to CompleteBalanceResponse (map only relevant fields)
            response.Result= new  CompleteBalanceResponse()
            {
                 DeclaredInterest = pensionFund.DeclaredInterest?.ToString(),
                 Employeefunds= pensionFund.Employeefunds ??0,
                    Totalfunds = pensionFund.Totalfunds ?? 0,
                     Employerfunds= pensionFund.Employerfunds ?? 0,
                       EVC_Contribution= pensionFund.EVC_Contribution ?? 0
                
            };
                response.Success = true;
            return response;


            }
            catch (Exception ex) {
                _settings.LogRequests($"{ex.Message}|{ex.StackTrace}", "CompleteBalanceRequest", RequestType.Error);
                response.Success = false;
                response.AddError("view-balance", "An error occurred while processing your request.");

            }
            return response;
        }
        /// <summary>
        /// Generates a cryptographically secure OTP of the given length, zero-padded.
        /// </summary>
        private static string GenerateSecureOtp(int length)
        {
            length = Math.Clamp(length, 1, 10);
            var max = (int)Math.Pow(10, length);
            var bytes = new byte[4];
            RandomNumberGenerator.Fill(bytes);
            var value = BitConverter.ToUInt32(bytes, 0) % max;
            return value.ToString($"D{length}");
        }
    }
}

using DAL.Model;
using DAL.ModelView;
using DAL.ModelView.Flex;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace API.Infrastructure.Application.Flex
{
    internal partial class FlexManager
    {
        /// <summary>
        /// Saves a base64 signature to a file in the Signatures folder and updates the CustomerProduct
        /// with the file path and base64 data.
        /// </summary>
        /// <param name="base64Signature">Base64-encoded signature (with or without data URL prefix, e.g. "data:image/png;base64,...").</param>
        /// <param name="customerProduct">The CustomerProduct to update.</param>
        /// <param name="signaturesFolder">Optional. Base folder for signatures. Defaults to "Signatures" under current directory.</param>
        /// <returns>The full file path where the signature was saved, or null if saving failed.</returns>
        public async Task<string?> SaveSignatureAndUpdateCustomerProductAsync(
            string base64Signature,
            CustomerProduct customerProduct,
            string? signaturesFolder = null)
        {
            if (string.IsNullOrWhiteSpace(base64Signature))
            {
                _logger.LogWarning("SaveSignatureAndUpdateCustomerProduct: base64Signature is null or empty.");
                return null;
            }

            try
            {
                // Strip data URL prefix if present (e.g. "data:image/png;base64,")
                var base64Data = base64Signature;
                var extension = ".png";
                if (base64Signature.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                {
                    var base64Index = base64Signature.IndexOf(",", StringComparison.Ordinal);
                    if (base64Index >= 0)
                    {
                        base64Data = base64Signature[(base64Index + 1)..];
                        var mimePart = base64Signature[..base64Index];
                        if (mimePart.Contains("jpeg", StringComparison.OrdinalIgnoreCase) || mimePart.Contains("jpg", StringComparison.OrdinalIgnoreCase))
                            extension = ".jpg";
                        else if (mimePart.Contains("gif", StringComparison.OrdinalIgnoreCase))
                            extension = ".gif";
                    }
                }

                var bytes = Convert.FromBase64String(base64Data);

                var basePath = string.IsNullOrWhiteSpace(signaturesFolder)
                    ? Path.Combine(Directory.GetCurrentDirectory(), "Signatures")
                    : signaturesFolder;

                if (!Directory.Exists(basePath))
                    Directory.CreateDirectory(basePath);

                var fileName = $"{customerProduct.RefNo ?? "sig"}_{DateTime.UtcNow:yyyyMMddHHmmss}{extension}";
                var filePath = Path.Combine(basePath, fileName);

                await File.WriteAllBytesAsync(filePath, bytes);

                customerProduct.Filelocation = filePath;
                customerProduct.Filebytes = base64Signature;
                await _db.SaveChangesAsync();

                _logger.LogInformation("Signature saved to {FilePath} for CustomerProduct {RefNo}", filePath, customerProduct.RefNo);
                return filePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save signature for CustomerProduct {RefNo}", customerProduct.RefNo);
                _isettings.LogRequests(ex.Message, "SaveSignatureAndUpdateCustomerProduct", Interface.RequestType.Error);
                return null;
            }
        }

        public async Task<ResponseDTO> CompleteActivation(CompleteActivation activateDTO)
        {
            var response = new ResponseDTO();
            try
            {

            // Query for the customer product by ProductRef
            var customerProduct = await _db.customerProducts
                .FirstOrDefaultAsync(cp => cp.RefNo == activateDTO.ProductRef);

            if (customerProduct == null)
            {
                response.Success = false;
                response.AddError("validation field", "Product not found.");
                return response;
            }

            // Check ActivationMode and payload validity
            if (customerProduct.ActivationMode == (int)ActivationMode.OTP)
            {
                if (string.IsNullOrEmpty(activateDTO.OTP))
                {
                    response.Success = false;
                    response.AddError("validation field", "OTP is required for OTP activation mode.");
                    return response;
                }

                // Query OTP
                var otp = await _db.OTPs
                    .OrderByDescending(o => o.CreatedAt)
                    .FirstOrDefaultAsync(o => o.CustomerId == customerProduct.CustomerId 
                                             //&& o.ProductRef == activateDTO.ProductRef
                                             && o.Code == activateDTO.OTP 
                                             && !o.IsUsed);

                if (otp == null)
                {
                    response.Success = false;
                    response.AddError("validation field", "Invalid OTP.");
                    return response;
                }

                // Check expiration
                if (otp.ExpiredAt < DateTime.UtcNow)
                {
                    response.Success = false;
                    response.AddError("validation field", " OTP Invalid or has expired.");
                    return response;
                }

                // Mark OTP as used
                otp.IsUsed = true;

                // Update customerProduct
                customerProduct.Validated = true;
                customerProduct.Activated = true;
                customerProduct.Complete = true;

                await _db.SaveChangesAsync();

                response.Success = true;
                response.AddError("validation field", "Activation completed successfully.");
                return response;
            }
            else if (customerProduct.ActivationMode == (int)ActivationMode.Signature)
            {
                if (string.IsNullOrEmpty(activateDTO.Signature))
                {
                    response.Success = false;
                    response.AddError("validation field", "Signature is required for Signature activation mode.");
                    return response;
                }

                var savedPath = await SaveSignatureAndUpdateCustomerProductAsync(activateDTO.Signature, customerProduct);
                if (string.IsNullOrEmpty(savedPath))
                {
                    response.Success = false;
                    response.AddError("validation field", "Failed to save signature.");
                    return response;
                }

                customerProduct.Validated = true;
                customerProduct.Activated = true;
                customerProduct.Complete = true;

                await _db.SaveChangesAsync();

                response.Success = true;
                response.AddError("validation field", "Activation completed successfully.");
                return response;
            }
            else
            {
                response.Success = false;
                response.AddError("validation field", "Unsupported activation mode.");
                return response;
            }


            }
            catch (Exception ex)
            {
                response.Success = false;
                response.AddError("validation field", ex.Message);
                _isettings.LogRequests(ex.Message, "CompleteActivation", Interface.RequestType.Error);
            }
            return response;
        }
        public async Task<ActivateResponseDTO> Activate(ActivateDTO request)
        {
            var response = new ActivateResponseDTO();
            try
            {
                var customerProduct = await _db.customerProducts
                    .FirstOrDefaultAsync(cp => cp.RefNo == request.ProductRef);

                if (customerProduct == null)
                {
                    response.Success = false;
                    response.Message = "Product not found.";
                    return response;
                }

                customerProduct.ActivationMode = (int)request.activationMode;

                if (request.activationMode == ActivationMode.OTP)
                {
                    var otpCode = _isettings.GenerateRadomCode(6);
                    var otp = new OTP
                    {
                        Id = Guid.NewGuid(),
                        CustomerId = customerProduct.CustomerId,
                        Code = otpCode,
                        Message = otpCode,
                         ProductRef=request.ProductRef,
                        notificationType = NotificationType.SMS,
                        CreatedAt = DateTime.UtcNow,
                        ExpiredAt = DateTime.UtcNow.AddMinutes(5),
                        IsUsed = false,
                        ISsent = false,
                        ISSMSSent = false,
                        IsEmailSent = false
                    };
                    _db.OTPs.Add(otp);
                    response.Message = "You'll recieve an SMS to complete the request: expires in 5 minutes";
                }

                await _db.SaveChangesAsync();
               
                
                response.Success = true;
                
            }
            catch (Exception ex)
            {
                response.Message = "Error occurred while activating, please try later";
                response.Success = false;
                response.Message = ex.Message;
                _isettings.LogRequests(ex.Message, "Activate", Interface.RequestType.Error);
            }
            return response;
        }
    }
}

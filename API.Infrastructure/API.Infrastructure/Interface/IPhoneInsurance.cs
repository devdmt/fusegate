using DAL.ModelView;
using DAL.ModelView.Safaricom;
using Microsoft.AspNetCore.Http;

namespace API.Infrastructure.Interface
{
    public interface IPhoneInsurance:ITransientService
    {
       // Task<ResponseDTO> PurchaseInsurance(List<PhoneInsuranceRequest> request);
        Task<OnboardingResponseDTO> PurchaseInsurance(PhoneInsuranceRequest request);
       
        Task<ResponseDTO> SubmitClaim(ClaimRequestDTO request);
        Task<ClaimResponseDetails> GetCustomerClaims(string request); 
        Task<ResponseDTO> ReplaceClaimRequest(ReplaceRequestDeviceDTO request);
    }
}

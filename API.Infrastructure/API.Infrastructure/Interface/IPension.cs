using DAL.ModelView;
using DAL.ModelView.Pension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Infrastructure.Interface
{
    public interface IPension :ITransientService
    {
       
        Task<OnboardResponse> OnboardingRequest(PensionOnboardingDTO onboardingDto, string Ip, string PartnerId, string partnerName,string PartnerCode);
        Task<PensionQuoteResponse> GetQuote(PensionCalculatorDTO request);
        Task<ResponseDTO<List<BeneficiaryDetailsDTO>>>  AddBeneficiaries(PensionBeneficiaryDTO pensionerBeneficiarieAddDTO);
        Task<ResponseDTO<BalanceRequestResponse>> BalanceRequest(BalanceDTORequest request);
        Task<ResponseDTO<CompleteBalanceResponse>> CompleteBalanceRequest(ViewBalanceDTORequest request);
        Task<ResponseDTO> Contribute(contributeDTO contributeDTO,string partnerCode);
        Task<ResponseDTO<TransferDTO>> Transfer(TransferRequestDTO transferDTO,string PartnerCode);
        PensionQuoteResponse CalculateCompoundInterest(
            decimal contributionAmount,
            decimal oneTimeCont,
            decimal annualRate,
            decimal desiredRetirementIncome,
            string DateOfBirth,
            int retirementAge,
            string type = "both",
            string frequency = "Monthly");
    }
}

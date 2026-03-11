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
        Task<OnboardResponse> OnboardingRequest(PensionOnboardingDTO onboardingDto, string Ip, string partnerId);
        Task<PensionQuoteResponse> GetQuote(PensionCalculatorDTO request);
        Task<ResponseDTO<BeneficiaryDetailsDTO>>  AddBeneficiaries(PensionBeneficiaryDTO pensionerBeneficiarieAddDTO);
        Task<ResponseDTO<BalanceRequestResponse>> BalanceRequest(BalanceDTORequest request);
        Task<ResponseDTO<CompleteBalanceResponse>> CompleteBalanceRequest(ViewBalanceDTORequest request);
        Task<ResponseDTO> Contribute(contributeDTO contributeDTO);
        Task<ResponseDTO<TransferDTO>> Transfer(TransferRequestDTO transferDTO,string PartnerCode);
    }
}

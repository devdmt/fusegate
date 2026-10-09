using DAL.Model;
using DAL.ModelView;
using DAL.ModelView.Flex;
using DAL.ModelView.FuneralExpense;
using DAL.ModelView.Pension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Infrastructure.Interface
{
    public interface IflexManager:ITransientService
    {
         Task<TransactionStatusDTO> GetContributionStatus(ContributionStatusRequest request);
        Task<RateResponse> CalculateLastExpense(LastExpenseCalcDTO request);
        Task<List<LastExpenseOptions>> GetOptions();
        Task<MainRateRiderResponse> CalculateRates(RateSDTO rateSDTO,Productenum product);
        Task<OnboardResponse> OnBoarding(CustomerBIODTO customer,string PartnerId);
        Task<ResponseDTO<BeneficiaryResultDto>>     AddBeneficiaryAsync(BeneficiaryCreateDTO dto, Guid userId,string partnercode);
        Task CreateLead(LeadsDTO leads);
        Task<ProductDTO> GetMyProducts(string Partner);
        Task<ActivateResponseDTO> Activate(ActivateDTO request);
        Task<ResponseDTO> CompleteActivation(CompleteActivation activateDTO);
        Task<ResponseDTO> Contribute(ContributeDTO dto,string PartnerCode);
        // Task<FuneralExpenseQuotationResponseDto> GetQuote(FuneralExpenseQuotationDto funeralExpenseDto);
    }

    
};

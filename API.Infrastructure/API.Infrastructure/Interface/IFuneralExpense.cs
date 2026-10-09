using DAL.Model;
using DAL.Model.FuneralExpense;
using DAL.ModelView;
using DAL.ModelView.CreditLife;
using DAL.ModelView.Flex;
using DAL.ModelView.FuneralExpense;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Infrastructure.Interface;

public interface IFuneralExpense : ITransientService
{
    Task<FuneralExpenseOnboardingResponseDto> OnboardingRequest(FuneralExpenseOnboardingDto funeralExpenseOnboardingDto);
    Task<MemberHealthResponseDto> ProcessMedical(MemberHealthDto memberHealthDto);
    Task<List<ProductDTO>> GetProducts(string partnerCode);
    Task<FuneralExpenseQuotationResponseDto> GetQuote(FuneralExpenseQuotationDto funeralExpenseDto);
    Task<PaymentContributionResponseDto> Contribution(PaymentContributionDto paymentContributionDto);
  
    Task<PolicyActivationResponseDto> Activation(PolicyActivationDto policyActivationDto);
    Task<PolicyResponseDto> Status(PolicyStatusDto policyStatusDto);
    Task<List<Terms>> GetMainTerms();
    Task<List<RiderAmount>> GetRiderAmount(FuneralExpenseQuotationDto rate, int frequency);
    Task<string> SendOTP(string memberId);
}

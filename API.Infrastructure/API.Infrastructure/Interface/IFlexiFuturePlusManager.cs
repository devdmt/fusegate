using DAL.Core.Interface;
using DAL.ModelView.FlexiFuture;
using DAL.ModelView.FlexiFuturePlus;

namespace API.Infrastructure.Interface;

public interface IFlexiFuturePlusManager : ITransientService
{
    Task<FlexiFuturePlusResponse<FlexiFutureUiConfigDto>> GetConfigsAsync(CancellationToken cancellationToken = default);
    Task<FlexiFuturePlusResponse<FlexiFutureCoverPremiumsResultDto>> GetCoverPremiumsAsync(
        FlexiFutureCoverPremiumsRequestDto request,
        PartnerContext partner,
        CancellationToken cancellationToken = default);
    Task<FlexiFuturePlusResponse<FlexiFutureQuoteResultDto>> UpsertQuoteAsync(
        FlexiFutureQuoteUpsertDto request,
        PartnerContext partner,
        string? ipAddress,
        string? browser,
        CancellationToken cancellationToken = default);
    Task<FlexiFuturePlusResponse<object>> ShareQuoteAsync(
        FlexiFutureShareQuoteDto request,
        PartnerContext partner,
        string? ipAddress,
        string? browser,
        CancellationToken cancellationToken = default);
    Task<FlexiFuturePlusResponse<List<FlexiFutureQuoteSearchResultDto>>> SearchQuotesAsync(
        FlexiFutureQuoteSearchDto request,
        PartnerContext partner,
        CancellationToken cancellationToken = default);
    Task<FlexiFuturePlusResponse<FlexiFuturePlusOnboardingQuestionsDto>> GetOnboardingQuestionsAsync(
        Guid quoteId,
        PartnerContext partner,
        CancellationToken cancellationToken = default);
    Task<FlexiFuturePlusResponse<FlexiFuturePlusOnboardingStateDto>> SaveConsolidatedOnboardingAsync(
        FlexiFuturePlusOnboardingRequestDto request,
        PartnerContext partner,
        string? ipAddress,
        string? browser,
        CancellationToken cancellationToken = default);
    Task<FlexiFuturePlusResponse<FlexiFuturePlusOnboardingStateDto>> GetOnboardingStateAsync(
        Guid quoteId,
        PartnerContext partner,
        CancellationToken cancellationToken = default);
    Task<FlexiFuturePlusResponse<object>> SendConsentOtpAsync(
        FlexiFuturePlusSendOtpRequestDto request,
        PartnerContext partner,
        CancellationToken cancellationToken = default);
    Task<FlexiFuturePlusResponse<FlexiFutureQuoteResultDto>> UpdateQuoteAsync(
        FlexiFutureQuoteRecalculateDto request,
        PartnerContext partner,
        string? ipAddress,
        string? browser,
        CancellationToken cancellationToken = default);
    Task<FlexiFuturePlusResponse<FlexiFuturePlusPaymentResultDto>> AddPaymentAsync(
        FlexiFuturePlusAddPaymentRequestDto request,
        PartnerContext partner,
        string? ipAddress,
        string? browser,
        CancellationToken cancellationToken = default);
    Task<FlexiFuturePlusResponse<object>> PollMpesaStkResultAsync(
        string stkTransactionId,
        PartnerContext partner,
        CancellationToken cancellationToken = default);
}

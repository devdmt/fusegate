using API.Infrastructure.Interface;
using API.Infrastructure.Otp;
using DAL;
using DAL.ModelView;
using DAL.ModelView.FlexiFuture;
using DAL.ModelView.FlexiFuturePlus;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace API.Infrastructure.Application.FlexiFuturePlus;

public partial class FlexiFuturePlusManager : IFlexiFuturePlusManager
{
    private const int DefaultMaximumFlexiQuoteRetry = 3;

    /// <summary>Partner API origin — matches <see cref="RegistrationChannel.API"/> stamped on quotes/policies.</summary>
    private const int PartnerApiSource = (int)RegistrationChannel.API;

    private readonly AkibappDbContext _akiba;
    private readonly ApplicationDbContext _db;
    private readonly IMemoryCache _cache;
    private readonly ILogger<FlexiFuturePlusManager> _logger;
    private readonly Isettings _settings;
    private readonly IPay _ipay;
    private readonly OtpSettings _otpSettings;

    public FlexiFuturePlusManager(
        AkibappDbContext akiba,
        ApplicationDbContext db,
        IMemoryCache cache,
        ILogger<FlexiFuturePlusManager> logger,
        Isettings settings,
        IPay ipay,
        IOptions<OtpSettings> otpSettings)
    {
        _akiba = akiba;
        _db = db;
        _cache = cache;
        _logger = logger;
        _settings = settings;
        _ipay = ipay;
        _otpSettings = otpSettings?.Value ?? new OtpSettings();
    }

    public async Task<FlexiFuturePlusResponse<FlexiFutureUiConfigDto>> GetConfigsAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var config = await GetUiConfigAsync(cancellationToken);
            return FlexiFuturePlusResponse<FlexiFutureUiConfigDto>.Ok(config, "Configuration retrieved successfully.");
        }
        catch (Exception ex)
        {
            return FlexiFuturePlusResponse<FlexiFutureUiConfigDto>.Fail(
                FlexiFutureErrorHandling.ToClientMessage(ex, _logger, nameof(GetConfigsAsync)));
        }
    }

    public async Task<FlexiFuturePlusResponse<FlexiFutureCoverPremiumsResultDto>> GetCoverPremiumsAsync(
        FlexiFutureCoverPremiumsRequestDto request,
        PartnerContext partner,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _ = partner;
            return await ComputeCoverPremiumsAsync(request, cancellationToken);
        }
        catch (Exception ex)
        {
            return FlexiFuturePlusResponse<FlexiFutureCoverPremiumsResultDto>.Fail(
                FlexiFutureErrorHandling.ToClientMessage(ex, _logger, nameof(GetCoverPremiumsAsync)));
        }
    }

    public Task<FlexiFuturePlusResponse<FlexiFutureQuoteResultDto>> UpsertQuoteAsync(
        FlexiFutureQuoteUpsertDto request,
        PartnerContext partner,
        string? ipAddress,
        string? browser,
        CancellationToken cancellationToken = default)
    {
        request.IpAddress = ipAddress;
        request.Browser = browser;
        return UpsertQuoteInternalAsync(request, partner, cancellationToken);
    }

    public Task<FlexiFuturePlusResponse<object>> ShareQuoteAsync(
        FlexiFutureShareQuoteDto request,
        PartnerContext partner,
        string? ipAddress,
        string? browser,
        CancellationToken cancellationToken = default)
    {
        request.IpAddress = ipAddress;
        request.Browser = browser;
        return ShareQuoteInternalAsync(request, partner, cancellationToken);
    }

    public Task<FlexiFuturePlusResponse<List<FlexiFutureQuoteSearchResultDto>>> SearchQuotesAsync(
        FlexiFutureQuoteSearchDto request,
        PartnerContext partner,
        CancellationToken cancellationToken = default)
        => SearchQuotesInternalAsync(request, partner, cancellationToken);

    public Task<FlexiFuturePlusResponse<FlexiFuturePlusOnboardingQuestionsDto>> GetOnboardingQuestionsAsync(
        Guid quoteId,
        PartnerContext partner,
        CancellationToken cancellationToken = default)
        => GetOnboardingQuestionsInternalAsync(quoteId, partner, cancellationToken);

    public Task<FlexiFuturePlusResponse<FlexiFuturePlusOnboardingStateDto>> SaveConsolidatedOnboardingAsync(
        FlexiFuturePlusOnboardingRequestDto request,
        PartnerContext partner,
        string? ipAddress,
        string? browser,
        CancellationToken cancellationToken = default)
        => SaveConsolidatedOnboardingInternalAsync(request, partner, ipAddress, browser, cancellationToken);

    public Task<FlexiFuturePlusResponse<FlexiFuturePlusOnboardingStateDto>> GetOnboardingStateAsync(
        Guid quoteId,
        PartnerContext partner,
        CancellationToken cancellationToken = default)
        => GetOnboardingStateInternalAsync(quoteId, partner, cancellationToken);

    public Task<FlexiFuturePlusResponse<object>> SendConsentOtpAsync(
        FlexiFuturePlusSendOtpRequestDto request,
        PartnerContext partner,
        CancellationToken cancellationToken = default)
        => SendConsentOtpInternalAsync(request, partner, cancellationToken);

    public Task<FlexiFuturePlusResponse<FlexiFutureQuoteResultDto>> UpdateQuoteAsync(
        FlexiFutureQuoteRecalculateDto request,
        PartnerContext partner,
        string? ipAddress,
        string? browser,
        CancellationToken cancellationToken = default)
        => UpdateQuoteInternalAsync(request, partner, ipAddress, browser, cancellationToken);

    public Task<FlexiFuturePlusResponse<FlexiFuturePlusPaymentResultDto>> AddPaymentAsync(
        FlexiFuturePlusAddPaymentRequestDto request,
        PartnerContext partner,
        string? ipAddress,
        string? browser,
        CancellationToken cancellationToken = default)
        => AddPaymentInternalAsync(request, partner, ipAddress, browser, cancellationToken);

    public Task<FlexiFuturePlusResponse<object>> PollMpesaStkResultAsync(
        string stkTransactionId,
        PartnerContext partner,
        CancellationToken cancellationToken = default)
        => PollMpesaStkResultInternalAsync(stkTransactionId, partner, cancellationToken);
}

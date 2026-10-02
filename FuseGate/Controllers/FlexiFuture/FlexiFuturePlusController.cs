using API.Infrastructure.Auth;
using API.Infrastructure.Interface;
using API.Infrastructure.OpenApi;
using DAL.Core.Interface;
using DAL.ModelView;
using DAL.ModelView.FlexiFuture;
using DAL.ModelView.FlexiFuturePlus;
using EsbJson.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace FuseGate.Controllers.FlexiFuture;

[Route("api/v{version:apiVersion}/FlexiFuturePlus")]
[ApiController]
public class FlexiFuturePlusController : VersionedApiController
{
    private readonly ILogger<FlexiFuturePlusController> _logger;
    private readonly ICurrentUser _currentUser;
    private readonly IPartnerAuthValidator _partnerAuth;
    private readonly IFlexiFuturePlusManager _manager;

    public FlexiFuturePlusController(
        ILogger<FlexiFuturePlusController> logger,
        ICurrentUser currentUser,
        IPartnerAuthValidator partnerAuth,
        IFlexiFuturePlusManager manager)
    {
        _logger = logger;
        _currentUser = currentUser;
        _partnerAuth = partnerAuth;
        _manager = manager;
    }

    [HttpGet("configs")]
    [PartnerCodeHeader]
    [ProducesResponseType(typeof(FlexiFuturePlusResponse<FlexiFutureUiConfigDto>), 200)]
    public async Task<IActionResult> GetConfigs(CancellationToken cancellationToken)
    {
        try
        {
            var auth = await ValidatePartnerAsync(nameof(GetConfigs), cancellationToken);
            if (auth != null) return auth;

            var result = await _manager.GetConfigsAsync(cancellationToken);
            return ToActionResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetConfigs failed");
            return BadRequest(FlexiFuturePlusResponse<FlexiFutureUiConfigDto>.Fail(ex.Message));
        }
    }

    [HttpPost("cover-premiums")]
    [PartnerCodeHeader]
    public async Task<IActionResult> GetCoverPremiums(
        [FromBody] FlexiFutureCoverPremiumsRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(FlexiFuturePlusResponse<FlexiFutureCoverPremiumsResultDto>.Fail("Invalid request."));

            var auth = await ValidatePartnerAsync(nameof(GetCoverPremiums), cancellationToken);
            if (auth != null) return auth;

            var result = await _manager.GetCoverPremiumsAsync(request, CurrentPartner(), cancellationToken);
            return ToActionResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetCoverPremiums failed");
            return BadRequest(FlexiFuturePlusResponse<FlexiFutureCoverPremiumsResultDto>.Fail(ex.Message));
        }
    }

    [HttpPost("quotes")]
    [PartnerCodeHeader]
    public async Task<IActionResult> UpsertQuote(
        [FromBody] FlexiFutureQuoteUpsertDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(FlexiFuturePlusResponse<FlexiFutureQuoteResultDto>.Fail("Invalid request."));

            var auth = await ValidatePartnerAsync(nameof(UpsertQuote), cancellationToken);
            if (auth != null) return auth;

            var result = await _manager.UpsertQuoteAsync(
                request,
                CurrentPartner(),
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                Request.Headers.UserAgent.ToString(),
                cancellationToken);
            return ToActionResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpsertQuote failed");
            return BadRequest(FlexiFuturePlusResponse<FlexiFutureQuoteResultDto>.Fail(ex.Message));
        }
    }

    [HttpPost("quotes/share")]
    [PartnerCodeHeader]
    public async Task<IActionResult> ShareQuote(
        [FromBody] FlexiFutureShareQuoteDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(FlexiFuturePlusResponse<object>.Fail("Invalid request."));

            var auth = await ValidatePartnerAsync(nameof(ShareQuote), cancellationToken);
            if (auth != null) return auth;

            var result = await _manager.ShareQuoteAsync(
                request,
                CurrentPartner(),
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                Request.Headers.UserAgent.ToString(),
                cancellationToken);
            return ToActionResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ShareQuote failed");
            return BadRequest(FlexiFuturePlusResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPost("quotes/search")]
    [PartnerCodeHeader]
    public async Task<IActionResult> SearchQuotes(
        [FromBody] FlexiFutureQuoteSearchDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var auth = await ValidatePartnerAsync(nameof(SearchQuotes), cancellationToken);
            if (auth != null) return auth;

            var result = await _manager.SearchQuotesAsync(request, CurrentPartner(), cancellationToken);
            return ToActionResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SearchQuotes failed");
            return BadRequest(FlexiFuturePlusResponse<List<FlexiFutureQuoteSearchResultDto>>.Fail(ex.Message));
        }
    }

    [HttpGet("onboarding/questions")]
    [PartnerCodeHeader]
    public async Task<IActionResult> GetOnboardingQuestions(
        [FromQuery] Guid quoteId,
        CancellationToken cancellationToken)
    {
        try
        {
            var auth = await ValidatePartnerAsync(nameof(GetOnboardingQuestions), cancellationToken);
            if (auth != null) return auth;

            var result = await _manager.GetOnboardingQuestionsAsync(quoteId, CurrentPartner(), cancellationToken);
            return ToActionResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetOnboardingQuestions failed");
            return BadRequest(FlexiFuturePlusResponse<FlexiFuturePlusOnboardingQuestionsDto>.Fail(ex.Message));
        }
    }

    [HttpPut("onboarding")]
    [PartnerCodeHeader]
    public async Task<IActionResult> SaveOnboarding(
        [FromBody] FlexiFuturePlusOnboardingRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(FlexiFuturePlusResponse<FlexiFuturePlusOnboardingStateDto>.Fail("Invalid request."));

            var auth = await ValidatePartnerAsync(nameof(SaveOnboarding), cancellationToken);
            if (auth != null) return auth;

            var result = await _manager.SaveConsolidatedOnboardingAsync(
                request,
                CurrentPartner(),
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                Request.Headers.UserAgent.ToString(),
                cancellationToken);
            return ToActionResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SaveOnboarding failed");
            return BadRequest(FlexiFuturePlusResponse<FlexiFuturePlusOnboardingStateDto>.Fail(ex.Message));
        }
    }

    [HttpGet("onboarding")]
    [PartnerCodeHeader]
    public async Task<IActionResult> GetOnboarding(
        [FromQuery] Guid quoteId,
        CancellationToken cancellationToken)
    {
        try
        {
            var auth = await ValidatePartnerAsync(nameof(GetOnboarding), cancellationToken);
            if (auth != null) return auth;

            var result = await _manager.GetOnboardingStateAsync(quoteId, CurrentPartner(), cancellationToken);
            return ToActionResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetOnboarding failed");
            return BadRequest(FlexiFuturePlusResponse<FlexiFuturePlusOnboardingStateDto>.Fail(ex.Message));
        }
    }

    [HttpPost("onboarding/send-otp")]
    [PartnerCodeHeader]
    public async Task<IActionResult> SendConsentOtp(
        [FromBody] FlexiFuturePlusSendOtpRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(FlexiFuturePlusResponse<object>.Fail("Invalid request."));

            var auth = await ValidatePartnerAsync(nameof(SendConsentOtp), cancellationToken);
            if (auth != null) return auth;

            var result = await _manager.SendConsentOtpAsync(request, CurrentPartner(), cancellationToken);
            return ToActionResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SendConsentOtp failed");
            return BadRequest(FlexiFuturePlusResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPut("quote")]
    [PartnerCodeHeader]
    public async Task<IActionResult> UpdateQuote(
        [FromBody] FlexiFutureQuoteRecalculateDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(FlexiFuturePlusResponse<FlexiFutureQuoteResultDto>.Fail("Invalid request."));

            var auth = await ValidatePartnerAsync(nameof(UpdateQuote), cancellationToken);
            if (auth != null) return auth;

            var result = await _manager.UpdateQuoteAsync(
                request,
                CurrentPartner(),
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                Request.Headers.UserAgent.ToString(),
                cancellationToken);
            return ToActionResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateQuote failed");
            return BadRequest(FlexiFuturePlusResponse<FlexiFutureQuoteResultDto>.Fail(ex.Message));
        }
    }

    [HttpPost("add-payment")]
    [PartnerCodeHeader]
    public async Task<IActionResult> AddPayment(
        [FromBody] FlexiFuturePlusAddPaymentRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(FlexiFuturePlusResponse<FlexiFuturePlusPaymentResultDto>.Fail("Invalid request."));

            var auth = await ValidatePartnerAsync(nameof(AddPayment), cancellationToken);
            if (auth != null) return auth;

            var result = await _manager.AddPaymentAsync(
                request,
                CurrentPartner(),
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                Request.Headers.UserAgent.ToString(),
                cancellationToken);
            return ToActionResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AddPayment failed");
            return BadRequest(FlexiFuturePlusResponse<FlexiFuturePlusPaymentResultDto>.Fail(ex.Message));
        }
    }

    [HttpPost("payments/mpesa/stk-result/{id}")]
    [PartnerCodeHeader]
    public async Task<IActionResult> PollMpesaStkResult(
        string id,
        CancellationToken cancellationToken)
    {
        try
        {
            var auth = await ValidatePartnerAsync(nameof(PollMpesaStkResult), cancellationToken);
            if (auth != null) return auth;

            var result = await _manager.PollMpesaStkResultAsync(id, CurrentPartner(), cancellationToken);
            return ToActionResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PollMpesaStkResult failed");
            return BadRequest(FlexiFuturePlusResponse<object>.Fail(ex.Message));
        }
    }

    private async Task<IActionResult?> ValidatePartnerAsync(string actionName, CancellationToken cancellationToken)
    {
        var auth = await _partnerAuth.ValidateAsync(
            _currentUser.PartnerCode(),
            GetPartnerCode(),
            actionName,
            cancellationToken);

        if (!auth.Success)
            return BadRequest(FlexiFuturePlusResponse<object>.Fail(auth.ErrorMessage ?? "Unauthorized.", auth.IsInvalidPartnerCode ? 401 : 400));

        _partnerLookup = auth.Partner!;
        return null;
    }

    private PartnerLookupDTO? _partnerLookup;

    private PartnerContext CurrentPartner() =>
        new(_partnerLookup!.PartnerCode, _partnerLookup.Id, _partnerLookup.PartnerName);

    private static IActionResult ToActionResult<T>(FlexiFuturePlusResponse<T> response) where T : class
    {
        if (!response.Success)
            return new ObjectResult(response) { StatusCode = response.StatusCode == 200 ? 400 : response.StatusCode };

        return new ObjectResult(response) { StatusCode = response.StatusCode };
    }

    private static IActionResult ToActionResult(FlexiFuturePlusResponse response)
    {
        if (!response.Success)
            return new ObjectResult(response) { StatusCode = response.StatusCode == 200 ? 400 : response.StatusCode };

        return new ObjectResult(response) { StatusCode = response.StatusCode };
    }
}

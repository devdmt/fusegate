using API.Infrastructure.Interface;
using API.Infrastructure.OpenApi;
using Azure;
using DAL.ModelView;
using DAL.ModelView.CreditLife;
using EsbJson.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EsbJson.API.Controllers.MSure
{
    [Authorize]
    public class CreditLifeController : VersionNeutralApiController
    {
        private readonly ILogger<CreditLifeController> _logger;
        private readonly ICreditLife _msure;
        public CreditLifeController(ICreditLife msure, ILogger<CreditLifeController> logger)
        {
            _msure = msure;
            _logger = logger;
        }

        //[AllowAnonymous]
        [HttpPost("customerOnboarding")]
        public async Task<IActionResult> OnboardingRequest([FromBody] CreditLifeDTO onboardingDto)
        {
            try
            {
                // Check if Model is valid
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                               _logger.LogInformation("Created Onboarding Request successfully Post Request at Controller: {controller}, " +
                    "at action:{action} , at time: {time} , with result: {r}",
                    nameof(CreditLifeController), nameof(OnboardingRequest), DateTime.Now,
                    JsonConvert.SerializeObject(onboardingDto));

                var result = await _msure.OnboardingRequest(onboardingDto);

                _logger.LogInformation("Created Onboarding Request successfully Post Request at Controller: {controller}, " +
                    "at action:{action} , at time: {time} , with result: {r}",
                    nameof(CreditLifeController), nameof(OnboardingRequest), DateTime.Now, JsonConvert.SerializeObject(result));

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error on Post Request at Controller: {controller}, " +
                "at action:{action} , at time: {time}, with Error: {error} and stackTrace : {trace}",
                   nameof(CreditLifeController), nameof(OnboardingRequest), DateTime.Now, ex.Message, ex.StackTrace);

                return BadRequest(ex.Message);
            }

        }
         [HttpPost("GetQuote")]
        [PartnerCodeHeader]
        public async Task<IActionResult> GetQuote([FromBody] QuoteRequestDTO msure)
        {
            var partnerCode = GetPartnerCode();
            var result = await _msure.GetQuote(msure,partnerCode);
            return Ok(result);
        }
        [HttpPost("InsureRequest")]
        [PartnerCodeHeader]
        public async Task<IActionResult> InsureRequest([FromBody] MsureDTO msure)
        {
            var partnerCode = GetPartnerCode();
            if (!string.IsNullOrEmpty(partnerCode))
            {
                msure.partnerCode = partnerCode;
            }
            var result = await _msure.ProcessRequest(msure);
            return Ok(result);
        }

        [HttpPost("GetProduct")]
        [PartnerCodeHeader]
        public async Task<IActionResult> GetProduct()
        {
            var partnerCode = GetPartnerCode();
            if (string.IsNullOrEmpty(partnerCode))
            {
                return BadRequest("PartnerCode header is required");
            }
            var result = await _msure.GetProducts(partnerCode);
            return Ok(result);
        }
    }
}

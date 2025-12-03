using API.Infrastructure.Interface;
using Azure;
using DAL.ModelView;
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
        [HttpPost("onboarding")]
        public async Task<IActionResult> OnboardingRequest([FromBody] CreditLifeDTO onboardingDto)
        {
            try
            {
                // Check if Model is valid
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

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

        [HttpPost("InsureRequest")]
        public async Task<IActionResult> InsureRequest([FromBody] MsureDTO msure)
        {
            var result = await _msure.ProcessRequest(msure);
            return Ok(result);
        }

        [HttpPost("GetProduct/{PartnerCode}")]
        public async Task<IActionResult> GetProduct(string PartnerCode)
        {
            var result = await _msure.GetProducts(PartnerCode);
            return Ok(result);
        }
    }
}

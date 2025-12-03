using DAL.ModelView.Safaricom;
using EsbJson.Controllers;
using Microsoft.AspNetCore.Mvc;
using DAL.ModelView.Pension;
using API.Infrastructure.Interface;
using DAL.ModelView;
using Microsoft.AspNetCore.Authorization;
using DAL.Core.Interface;
using Microsoft.Net.Http.Headers;
using API.Infrastructure.OpenApi;
namespace IPP.EsbJson.API.Controllers.Pension
{
    [Authorize]
    public class PensionController : VersionedApiController
    {
        readonly ILogger<PensionController> _logger;

         readonly IPension _pension;
        readonly ICurrentUser _currentUser;
        public PensionController(ILogger<PensionController> logger, IPension pension,ICurrentUser currentUser)
        {
            _logger = logger;
            _currentUser = currentUser;
            _pension = pension;
        }
            
        [HttpPost("Onboarding")]
        [ProducesResponseType(typeof(OnboardResponse), 200)]
        [ProducesResponseType(typeof(OnboardResponse), 400)]
         [PartnerCodeHeader]
        public async Task<IActionResult> Oboarding([FromBody] PensionOnboardingDTO request)
        {

             var authHeader = Request.Headers[HeaderNames.Authorization].FirstOrDefault();
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState)   ;
            }
            var user = _currentUser.GetUserId;
            var partner = "WEBAPI";
            var result = await _pension.OnboardingRequest(request,
                HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",partner);
            return Ok(result);
            
        }

         [HttpPost("AddBeneficiary")]
        [ProducesResponseType(typeof(PensionOnboardResponseDTO), 200)]
         [PartnerCodeHeader]
        public async Task<IActionResult> AddBeneficiary([FromBody]PensionBeneficiaryDTO repairShopAUth)
        {

            try
            {
                //  var result = await _claim.ClaimUserAuth(repairShopAUth);

                return Ok();

            }
            catch (Exception ex)
            {
                // _isettings.LogRequests(ex.Message, "Authenticate", RequestType.Error);
            }
            return BadRequest("Unable to authenticate you please try again");
        }
       
         [HttpPost("initiate-balance")]
        [ProducesResponseType(typeof(BalanceDTO), 200)]
         [PartnerCodeHeader]
        public async Task<IActionResult> BalanceRequest([FromBody]BalanceDTORequest balance)
        {

            try
            {
                //  var result = await _claim.ClaimUserAuth(repairShopAUth);

                return Ok();

            }
            catch (Exception ex)
            {
                // _isettings.LogRequests(ex.Message, "Authenticate", RequestType.Error);
            }
            return BadRequest("Unable to authenticate you please try again");
        }
         [HttpPost("view-balance")]
        [ProducesResponseType(typeof(BalanceDTO), 200)]
         [PartnerCodeHeader]
        public async Task<IActionResult> ViewBalance([FromBody]ViewBalanceDTORequest repairShopAUth)
        {

            try
            {
                //  var result = await _claim.ClaimUserAuth(repairShopAUth);

                return Ok();

            }
            catch (Exception ex)
            {
                // _isettings.LogRequests(ex.Message, "Authenticate", RequestType.Error);
            }
            return BadRequest("Unable to authenticate you please try again");
        }

        [HttpPost("Contribute")]
        [ProducesResponseType(typeof(ResponseDTO), 200)]
         [PartnerCodeHeader]
        public async Task<IActionResult> Contribute(contributeDTO repairShopAUth)
        {

            try
            {
                //  var result = await _claim.ClaimUserAuth(repairShopAUth);

                return Ok();

            }
            catch (Exception ex)
            {
                // _isettings.LogRequests(ex.Message, "Authenticate", RequestType.Error);
            }
            return BadRequest("Unable to authenticate you please try again");
        }

        [HttpPost("WithdrawalRequest")]
        [ProducesResponseType(typeof(ResponseDTO), 200)]
         [PartnerCodeHeader]
        public async Task<IActionResult> WithdrawalRequest(WithdrawalDTO withdrawal)
        {

            try
            {
                //  var result = await _claim.ClaimUserAuth(repairShopAUth);

                return Ok();

            }
            catch (Exception ex)
            {
                // _isettings.LogRequests(ex.Message, "Authenticate", RequestType.Error);
            }
            return BadRequest("Unable to authenticate you please try again");
        }

         [HttpPost("ActivateProduct")]
        [ProducesResponseType(typeof(ResponseDTO), 200)]
         [PartnerCodeHeader]
        public async Task<IActionResult> ActivateProduct([FromBody]ActivatePensionDTO withdrawal)
        {

            try
            {
                //  var result = await _claim.ClaimUserAuth(repairShopAUth);

                return Ok();

            }
            catch (Exception ex)
            {
                // _isettings.LogRequests(ex.Message, "Authenticate", RequestType.Error);
            }
            return BadRequest("Unable to authenticate you please try again");
        }
    }
}

using API.Infrastructure.Interface;
using API.Infrastructure.OpenApi;
using Azure.Core;
using DAL.Core.Interface;
using DAL.ModelView;
using DAL.ModelView.Pension;
using EsbJson.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
namespace IPP.EsbJson.API.Controllers.Pension
{
    [ApiController]
    public class PensionController : VersionedApiController
    {
        readonly ILogger<PensionController> _logger;        

         readonly IPension _pension;
        readonly ICurrentUser _currentUser;
        readonly Isettings _isettings;
        public PensionController(ILogger<PensionController> logger, IPension pension,ICurrentUser currentUser,Isettings isettings)
        {
            _logger = logger;
            _currentUser = currentUser;
            _pension = pension;
            _isettings = isettings;
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
            var user = _currentUser.PartnerCode;
            var partner = "WEBAPI";
            var result = await _pension.OnboardingRequest(request,
                HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",partner);
            return Ok(result);
            
        }

         [HttpPost("AddBeneficiary")]
        [ProducesResponseType(typeof(BeneficiaryResponseDTO), 200)]
         [PartnerCodeHeader]
        public async Task<IActionResult> AddBeneficiary([FromBody]PensionBeneficiaryDTO request)
        {

            try
            {
                  if(!ModelState.IsValid)
            {
                return BadRequest(ModelState)   ;
            }
            var partner = _currentUser.PartnerCode;
                  var result = await _pension.AddBeneficiaries(request);

                return Ok(result);

            }
            catch (Exception ex)
            {
                // _isettings.LogRequests(ex.Message, "Authenticate", RequestType.Error);
            }
            return BadRequest("Unable to authenticate you please try again");
        }
       
        [HttpPost("initiate-balance")]
        [ProducesResponseType(typeof(ResponseDTO<BalanceRequestResponse>), 200)]
        [ProducesResponseType(typeof(ResponseDTO<BalanceRequestResponse>), 400)]
        [PartnerCodeHeader]
        public async Task<IActionResult> BalanceRequest([FromBody] BalanceDTORequest balance)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var result = await _pension.BalanceRequest(balance);
                if (!result.Success)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BalanceRequest failed");
                return BadRequest(new ResponseDTO<BalanceRequestResponse> { Success = false, ErrorMsg = "Unable to process your request. Please try again." });
            }
        }
         [HttpPost("view-balance")]
        [ProducesResponseType(typeof(CompleteBalanceResponse), 200)]
         [PartnerCodeHeader]
        public async Task<IActionResult> ViewBalance([FromBody]ViewBalanceDTORequest request)
        {

            try
            {
                  var result = await _pension.CompleteBalanceRequest(request);
                if (!result.Success)
                {
                    return BadRequest(result);
                }
                return Ok(result);

            }
            catch (Exception ex)
            {
                // _isettings.LogRequests(ex.Message, "Authenticate", RequestType.Error);
            }
            return BadRequest("Unable to authenticate you please try again");
        }
        [HttpPost("Transfer")]
        [ProducesResponseType(typeof(ResponseDTO), 200)]
         [PartnerCodeHeader]
        public async Task<IActionResult> Transfer(TransferRequestDTO request)
        {

            try
            {
                var partner = _currentUser.PartnerCode();
                var pcode = GetPartnerCode();
                if(partner != pcode)
                {
                    _isettings.LogRequests(string.Format("partner {0} --pcode {1}",partner,pcode),"transfer", RequestType.Info);
                    return BadRequest("Invalid partner code");
                }
               var result = await _pension.Transfer(request, partner);

                return Ok(result);

            }
            catch (Exception ex)
            {
                 
            }
            return BadRequest("Unable to authenticate you please try again");
        }


        [HttpPost("Contribute")]
        [ProducesResponseType(typeof(ResponseDTO), 200)]
         [PartnerCodeHeader]
        public async Task<IActionResult> Contribute(contributeDTO request)
        {

            try
            {
               var result = await _pension.Contribute(request);

                return Ok(result);

            }
            catch (Exception ex)
            {
                 
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
  
        [HttpPost("GetQuote")]
        [ProducesResponseType(typeof(ResponseDTO), 200)]
         [PartnerCodeHeader]
        public async Task<IActionResult> GetQuote([FromBody]PensionCalculatorDTO withdrawal)
        {

            try
            {
               var result = await _pension.GetQuote(withdrawal);

                return Ok(result);

            }
            catch (Exception ex)
            {
                // _isettings.LogRequests(ex.Message, "Authenticate", RequestType.Error);
            }
            return BadRequest("Unable to authenticate you please try again");
        }
    }
}

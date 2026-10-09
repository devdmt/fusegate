using API.Infrastructure.Interface;
using API.Infrastructure.OpenApi;
using Azure.Core;
using DAL;
using DAL.Core.Interface;
using DAL.Model;
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
        readonly ILeads _leads;
        readonly ApplicationDbContext _db;
        public PensionController(ILogger<PensionController> logger,ILeads leads, IPension pension,ICurrentUser currentUser,Isettings isettings, ApplicationDbContext db)
        {
            _logger = logger;
            _currentUser = currentUser;
            _pension = pension;
            _leads = leads;
            _isettings = isettings;
            _db = db;
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
            var partner = _currentUser.PartnerCode();
           
                var pcode = GetPartnerCode();
                if(partner != pcode)
                {
                    _isettings.LogRequests(string.Format("partner {0} --pcode {1}",partner,pcode),"Oboarding", RequestType.Info);
                    return BadRequest("Invalid partner code");
                }

            var partnerLookup = await _db.GetPartnerAsync(pcode);
            if (partnerLookup == null)
            {
                return BadRequest("Partner not found.");
            }

            //var partner = "WEBAPI";
            var result = await _pension.OnboardingRequest(request,
                HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown", partnerLookup.Id.ToString(),
                partnerLookup.PartnerName,partnerLookup.PartnerCode);
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
        //[ProducesResponseType(typeof(ResponseDTO), 200)]
    //     [PartnerCodeHeader]
       // [AllowAnonymous]
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
                 _isettings.LogRequests(ex.Message, "Transfer", RequestType.Error);
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
                var partner = _currentUser.PartnerCode();
                var pcode = GetPartnerCode();
                if(partner != pcode)
                {
                    _isettings.LogRequests(string.Format("partner {0} --pcode {1}",partner,pcode),"transfer", RequestType.Info);
                    return BadRequest("Invalid partner code");
                }
               var result = await _pension.Contribute(request,partner);

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
               // var result = await _claim.ClaimUserAuth(repairShopAUth);

                return Ok();

            }
            catch (Exception ex)
            {
                // _isettings.LogRequests(ex.Message, "Authenticate", RequestType.Error);
            }
            return BadRequest("Unable to authenticate you please try again");
        } 
  [HttpPost("GetQuote2")]
        [ProducesResponseType(typeof(ResponseDTO), 200)]
       // [PartnerCodeHeader]
        [AllowAnonymous]
        public async Task<IActionResult> GetQuote2([FromBody]PensionCalculatorDTO withdrawal)
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
        [HttpPost("GetQuote")]
        [ProducesResponseType(typeof(ResponseDTO), 200)]
        [PartnerCodeHeader]
      
        public async Task<IActionResult> GetQuote([FromBody]PensionCalculatorDTO req)
        {

            try
            {
                //var partner = _currentUser.PartnerCode();
                var partnerCode = GetPartnerCode();
               
                await _leads.AddLeads(new CustomerApiLeads()
                {
                    CreatedOn = DateTime.UtcNow,
                     DateOfBirth=req.DateOfBirth,
                      Id= Guid.NewGuid(),
                       MonthlyContribution=(decimal)req.MonthlyContribution,
                        Names="Pension Calculator",
                         Phone=req.Phonenumber,
                          ProductEnum=Productenum.IPP   ,
                           RetirementAge=req.RetireAge,
                            StartingContribution=(decimal)req.StartingContribution
                }, partnerCode);
                //var result = await _pension.GetQuote(withdrawal);
                // Declare variables from the 'req' parameter to match parameters needed for CalculateCompoundInterest
                decimal contributionAmount = (decimal)req.MonthlyContribution;
                decimal oneTimeCont = (decimal)req.StartingContribution;
                decimal annualRate = 0;
                switch (req.InterestGrowthRate)
                {
                    case interestGrowthRate.guaranteed:
                        annualRate = 5;
                        break;
                    case interestGrowthRate.moderate:
                        annualRate = 8;
                        break;
                    case interestGrowthRate.aggressive:
                        annualRate = 14;
                        break;
                    default:
                        annualRate = 5;
                        break;
                }
                decimal desiredRetirementIncome = (decimal)req.RetireIncome;
                string dateOfBirth = req.DateOfBirth;
                int retirementAge = req.RetireAge; 
                string type = "both"; // or you may optionally allow callers to specify 'type'
                string frequency = req.frequency.ToString().Substring(0, 1).ToUpper() + req.frequency.ToString().Substring(1); // e.g. "Monthly"

                var result = _pension.CalculateCompoundInterest(
                    contributionAmount,
                    oneTimeCont,
                    annualRate,
                    desiredRetirementIncome,
                    dateOfBirth,
                    retirementAge,
                    type,
                    frequency
                );
    


          
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

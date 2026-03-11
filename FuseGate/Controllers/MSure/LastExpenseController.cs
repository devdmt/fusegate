//using API.Infrastructure.Interface;
//using API.Infrastructure.OpenApi;
//using DAL.ModelView.FuneralExpense;
//using EsbJson.Controllers;
//using Microsoft.AspNetCore.Authorization;

//namespace FuseGate.Controllers.MSure;

//[Route("api/[controller]")]
//[ApiController]
//[Authorize]
//public class LastExpenseController : VersionNeutralApiController
//{
//    private readonly IFuneralExpense _funeralExpense;
//    private readonly ILogger<LastExpenseController> _logger;

//    public LastExpenseController(IFuneralExpense funeralExpense, ILogger<LastExpenseController> logger)
//    {
//        _funeralExpense = funeralExpense;
//        _logger = logger;
//    }

//    [HttpPost("GetQuote")]
//    [PartnerCodeHeader]
//    [ProducesResponseType(typeof(FuneralExpenseQuotationResponseDto), StatusCodes.Status200OK)]
//    public async Task<IActionResult> GetQuote([FromBody] FuneralExpenseQuotationDto request)
//    {
//        try
//        {
//            _logger.LogInformation("Adding New Quote {@Model}", request);

//            if (!ModelState.IsValid)
//            {
//                _logger.LogError("Invalid ModelState Error Adding new Quote {@Model}", request);
//                return ValidationProblem(ModelState);
//            }

//            //var partnerCode = GetPartnerCode();

//            //request.PartnerId = partnerCode;

//            var result = await _funeralExpense.GetQuote(request);

//            _logger.LogInformation("Added new Quote {@Model}", result);

//            return Ok(result);
//        }
//        catch(Exception ex)
//        {
//            _logger.LogError("Error Adding new Quote {@Model}", request);

//            return Problem(
//                        title: ex.Message,
//                        detail: ex.Message,
//                        statusCode: StatusCodes.Status500InternalServerError
//                );
//        }
       
        
//    }

//    [HttpPost("OnBoarding")]
//    [PartnerCodeHeader]
//    [ProducesResponseType(typeof(FuneralExpenseOnboardingResponseDto), StatusCodes.Status200OK)]
//    public async Task<IActionResult> OnBoarding([FromBody] FuneralExpenseOnboardingDto request)
//    {
//        try
//        {
//            _logger.LogInformation("Adding New OnBoarding {@Model}", request);

//            if (!ModelState.IsValid)
//            {
//                _logger.LogError("Invalid ModelState Error Adding new OnBoarding {@Model}", request);
//                return ValidationProblem(ModelState);
//            }

//            var result = await _funeralExpense.OnboardingRequest(request);

//            _logger.LogInformation("Added new OnBoarding {@Model}", result);

//            return Ok(result);
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError("Error Adding new OnBoarding {@Model}", request);

//            return Problem(
//                        title: "An unexpected error occurred",
//                        detail: ex.Message,
//                        statusCode: StatusCodes.Status500InternalServerError
//                );
//        }


//    }

//    [HttpPost("Medical")]
//    [PartnerCodeHeader]
//    [ProducesResponseType(typeof(MemberHealthResponseDto), StatusCodes.Status200OK)]
//    public async Task<IActionResult> Medical([FromBody] MemberHealthDto request)
//    {
//        try
//        {
//            _logger.LogInformation("Adding New Medical {@Model}", request);

//            if (!ModelState.IsValid)
//            {
//                _logger.LogError("Invalid ModelState Error Adding new Medical {@Model}", request);
//                return ValidationProblem(ModelState);
//            }

//            var result = await _funeralExpense.ProcessMedical(request);

//            _logger.LogInformation("Added new Medical {@Model}", result);

//            return Ok(result);
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError("Error Adding new Medical {@Model}", request);

//            return Problem(
//                        title: "An unexpected error occurred",
//                        detail: ex.Message,
//                        statusCode: StatusCodes.Status500InternalServerError
//                );
//        }


//    }

//    [HttpPost("Contribution")]
//    [PartnerCodeHeader]
//    [ProducesResponseType(typeof(PaymentContributionResponseDto), StatusCodes.Status200OK)]
//    public async Task<IActionResult> Contribution([FromBody] PaymentContributionDto request)
//    {
//        try
//        {
//            //var partnerCode = GetPartnerCode();
//            _logger.LogInformation("Adding New Contribution {@Model}", request);

//            if (!ModelState.IsValid)
//            {
//                _logger.LogError("Invalid ModelState Error Adding new Contribution {@Model}", request);
//                return ValidationProblem(ModelState);
//            }

//            var result = await _funeralExpense.Contribution(request);

//            _logger.LogInformation("Added new Contribution {@Model}", result);

//            return Ok(result);
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError("Error Adding new Contribution {@Model}", request);

//            return Problem(
//                        title: "An unexpected error occurred",
//                        detail: ex.Message,
//                        statusCode: StatusCodes.Status500InternalServerError
//                );
//        }


//    }

//    [HttpPost("Beneficiary")]
//    [PartnerCodeHeader]
//    [ProducesResponseType(typeof(BeneficiaryResponseDto), StatusCodes.Status200OK)]
//    public async Task<IActionResult> Beneficiary([FromBody] BeneficiaryDto request)
//    {
//        try
//        {
//            //var partnerCode = GetPartnerCode();
//            _logger.LogInformation("Adding New Beneficiary {@Model}", request);

//            if (!ModelState.IsValid)
//            {
//                _logger.LogError("Invalid ModelState Error Adding new Beneficiary {@Model}", request);
//                return ValidationProblem(ModelState);
//            }

//            //var result = await _funeralExpense.Beneficiary(request);

//           // _logger.LogInformation("Added new Beneficiary {@Model}", result);

//           // return Ok(result);
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError("Error Adding new Beneficiary {@Model}", request);

//            return Problem(
//                        title: "An unexpected error occurred",
//                        detail: ex.Message,
//                        statusCode: StatusCodes.Status500InternalServerError
//                );
//        }

//        return BadRequest("error creating beneficiary");
//    }

//    [HttpPost("Activation")]
//    [PartnerCodeHeader]
//    [ProducesResponseType(typeof(PolicyActivationResponseDto), StatusCodes.Status200OK)]
//    public async Task<IActionResult> Activation([FromBody] PolicyActivationDto request)
//    {
//        try
//        {
//            //var partnerCode = GetPartnerCode();
//            _logger.LogInformation("Adding New Activation {@Model}", request);

//            if (!ModelState.IsValid)
//            {
//                _logger.LogError("Invalid ModelState Error Adding new Activation {@Model}", request);
//                return ValidationProblem(ModelState);
//            }

//            var result = await _funeralExpense.Activation(request);

//            _logger.LogInformation("Added new Activation {@Model}", result);

//            return Ok(result);
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError("Error Adding new Activation {@Model}", request);

//            return Problem(
//                        title: "An unexpected error occurred",
//                        detail: ex.Message,
//                        statusCode: StatusCodes.Status500InternalServerError
//                );
//        }


//    }


//    [HttpPost("Status")]
//    [PartnerCodeHeader]
//    [ProducesResponseType(typeof(PolicyResponseDto), StatusCodes.Status200OK)]
//    public async Task<IActionResult> Status([FromBody] PolicyStatusDto request)
//    {
//        try
//        {
//            //var partnerCode = GetPartnerCode();
//            _logger.LogInformation("Adding New PolicyStatus {@Model}", request);

//            if (!ModelState.IsValid)
//            {
//                _logger.LogError("Invalid ModelState Error Adding new PolicyStatus {@Model}", request);
//                return ValidationProblem(ModelState);
//            }

//            var result = await _funeralExpense.Status(request);

//            _logger.LogInformation("Added new PolicyStatus {@Model}", result);

//            return Ok(result);
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError("Error Adding new PolicyStatus {@Model}", request);

//            return Problem(
//                        title: "An unexpected error occurred",
//                        detail: ex.Message,
//                        statusCode: StatusCodes.Status500InternalServerError
//                );
//        }


//    }

//    [HttpPost("Otp/{MemberId}")]
//    [PartnerCodeHeader]
//    public async Task<IActionResult> Otp(string MemberId)
//    {
//        try
//        {
//            //var partnerCode = GetPartnerCode();
//            _logger.LogInformation("Adding New OTP {@Model}", MemberId);

//            if (!ModelState.IsValid)
//            {
//                _logger.LogError("Invalid ModelState Error Adding new OTP {@Model}", MemberId);
//                return ValidationProblem(ModelState);
//            }

//            var result = await _funeralExpense.SendOTP(MemberId);

//            _logger.LogInformation("Added new OTP {@Model}", MemberId);

//            return Ok(new {Otp = result });
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError("Error Adding new OTP {@Model}", MemberId);

//            return Problem(
//                        title: "An unexpected error occurred",
//                        detail: ex.Message,
//                        statusCode: StatusCodes.Status500InternalServerError
//                );
//        }


//    }

//}

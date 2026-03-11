using API.Infrastructure.Application.HealthDeclaration;
using API.Infrastructure.Common;
using API.Infrastructure.Interface;
using API.Infrastructure.OpenApi;
using DAL.Core.Interface;
using DAL.ModelView;
using DAL.ModelView.Flex;
using DAL.ModelView.HealthDeclaration;
using EsbJson.Controllers;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace FuseGate.Controllers.MSure
{
    public class InsureController : VersionedApiController
    {
        readonly IflexManager _iflexManager;
        private readonly IHealthDeclarationService _healthDeclarationService;
    private readonly IValidator<HealthDeclarationRequestDto> _validator;
    private readonly ILogger<InsureController> _logger;
        ICurrentUser _currentUsers;
    public InsureController(
        IHealthDeclarationService healthDeclarationService,
        IValidator<HealthDeclarationRequestDto> validator, ICurrentUser currentUsers,
        ILogger<InsureController> logger,IflexManager iflexManager)
    {
        _healthDeclarationService = healthDeclarationService;
        _validator = validator;
        _logger = logger;
            _iflexManager = iflexManager;
            _currentUsers = currentUsers;
        }
         [HttpPost("GetMyProducts")]
        [PartnerCodeHeader]
        public async Task<IActionResult> GetMyProducts()
        {
            try
            {

                var partnerCode = GetPartnerCode();
                if (partnerCode == null) {

                    return BadRequest("Invalid credentials");
                
                }

                var result = await _iflexManager.GetMyProducts(partnerCode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
         [HttpPost("LastExpense/GetOptions")]
    
        public async Task<IActionResult> GetQuote()
        {
            try
            {
                var partnerCode = GetPartnerCode();
                var result = await _iflexManager.GetOptions();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
  [HttpPost("LastExpense/GetQuote")]
        [PartnerCodeHeader]
        public async Task<IActionResult> GetQuote([FromBody] LastExpenseCalcDTO request)
        {
            try
            {
                var partnerCode = GetPartnerCode();
                var result = await _iflexManager.CalculateLastExpense(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("GetQuote/FlexEducator")]
        [PartnerCodeHeader]
        public async Task<IActionResult> GetQuote([FromBody] RateSDTO rateSDTO)
        {
            try
            {
                var partnerCode = GetPartnerCode();
                var result = await _iflexManager.CalculateRates(rateSDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("Onboarding")]
        [PartnerCodeHeader]
        public async Task<IActionResult> Onboarding([FromBody] CustomerBIODTO customer)
        {
            try
            {
                var partnerCode = GetPartnerCode();
                var claim= User.Claims.FirstOrDefault(c => c.Type == "client_id")?.Value;
                var result = await _iflexManager.OnBoarding(customer, partnerCode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    
         [HttpPost("beneficiaries")]
        
    public async Task<IActionResult> AddBeneficiary([FromBody] BeneficiaryCreateDTO dto)
    {
            
            var partnerCode = _currentUsers.PartnerCode();
          var claim = _currentUsers.GetUserId();
             
            var result = await _iflexManager.AddBeneficiaryAsync(dto,claim,partnerCode);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
        
         [HttpPost("Activate")]
    public async Task<IActionResult> Activate([FromBody] ActivateDTO dto)
    {
            if (ModelState.IsValid)
            {        
                var response = await _iflexManager.Activate(dto);
            return Ok(response);

            }


            return BadRequest("Error processing request");
    }

           [HttpPost("CompleteActivation")]
    public async Task<IActionResult> CompleteActivation([FromBody] CompleteActivation dto)
    {
             if (ModelState.IsValid)
            {        
                var response = await _iflexManager.CompleteActivation(dto);
            return Ok(response);

            }
            return BadRequest("Error processing request");
    }
               [HttpPost("Contribute")]
    public async Task<IActionResult> Contribute([FromBody] ContributeDTO dto)
    {

            return BadRequest("Error processing request");
    }

          [HttpPost("{memberno}/health-declaration")]
    [ProducesResponseType(typeof(ApiResponse<HealthDeclarationResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostHealthDeclaration(
        [FromRoute] string memberno,
        [FromBody] HealthDeclarationRequestDto request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(memberno))
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Validation failed",
                Errors = new List<ValidationError> { new() { Field = "memberno", Error = "member no is required." } }
            });
        }

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => new ValidationError
            {
                Field = ToCamelCasePath(e.PropertyName),
                Error = e.ErrorMessage
            }).ToList();
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Validation failed",
                Errors = errors
            });
        }

        var result = await _healthDeclarationService.UpsertAsync(memberno.Trim(), request, cancellationToken);

        if (!result.Success)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = result.Message,
                Errors = result.Errors
            });
        }

        return Ok(new ApiResponse<HealthDeclarationResponseDto>
        {
            Success = true,
            Message = result.Message,
            Data = result.Data
        });
    }

    private static string ToCamelCasePath(string propertyPath)
    {
        if (string.IsNullOrEmpty(propertyPath)) return propertyPath;
        var parts = propertyPath.Split('.');
        return string.Join(".", parts.Select(p =>
        {
            var bracket = p.IndexOf('[');
            var name = bracket >= 0 ? p.Substring(0, bracket) : p;
            var rest = bracket >= 0 ? p.Substring(bracket) : "";
            if (name.Length == 0) return p;
            return char.ToLowerInvariant(name[0]) + name.Substring(1) + rest;
        }));
    }
    }
}

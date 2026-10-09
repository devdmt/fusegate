using API.Infrastructure.Application.HealthDeclaration;
using API.Infrastructure.Common;
using API.Infrastructure.Interface;
using DAL.ModelView.HealthDeclaration;
using EsbJson.Controllers;
using FluentValidation;
using DAL.ModelView;

namespace EsbJson.API.Controllers;

/// <summary>
/// Captures and updates customer health declarations for insurance onboarding.
/// </summary>
[ApiController]
[Route("api/customers")]
public class HealthDeclarationsController : BaseApiController
{
    private readonly IHealthDeclarationService _healthDeclarationService;
    private readonly IValidator<HealthDeclarationRequestDto> _validator;

    public HealthDeclarationsController(IHealthDeclarationService healthDeclarationService, IValidator<HealthDeclarationRequestDto> validator)
    {
        _healthDeclarationService = healthDeclarationService;
        _validator = validator;
    }

    /// <summary>
    /// Submit or update a health declaration for a customer.
    /// </summary>
    /// <param name="customerId">Customer identifier (from route; takes precedence over body).</param>
    /// <param name="request">Vitals, health declaration flag, and optional list of question answers.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>200 with declaration summary, or 400 with validation errors.</returns>
    /// <remarks>
    /// Example request:
    /// <code>
    /// POST /api/customers/cust-001/health-declaration
    /// {
    ///   "vitals": { "heightCm": 170.0, "weightKg": 68.0 },
    ///   "healthQuestion": {
    ///     "healthDeclaration": true,
    ///     "answers": [
    ///       { "question": "ExistingConditions", "answer": true, "narration": "Hypertension since 2021, on meds." },
    ///       { "question": "DrugAlcoholAbuse", "answer": false, "narration": null }
    ///     ]
    ///   },
    ///   "contextKey": "Application:12345"
    /// }
    /// </code>
    /// Supported question values: PriorDeclinedInsurance, ExistingConditions, DrugAlcoholAbuse, Respiratory, HeartCirculation, ChronicConditions, Wellness, ImmuneViral, Senses.
    /// </remarks>
  
}

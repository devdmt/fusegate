using API.Infrastructure.Common;
using API.Infrastructure.Interface;
using DAL;
using DAL.Model.HealthDeclaration;
using DAL.ModelView;
using DAL.ModelView.HealthDeclaration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Office.Interop.Excel;
namespace API.Infrastructure.Application.HealthDeclaration;

public class HealthDeclarationService : IHealthDeclarationService
{
    private static readonly string[] SupportedQuestionNames =
    {
        nameof(HealthQuestionType.PriorDeclinedInsurance),
        nameof(HealthQuestionType.ExistingConditions),
        nameof(HealthQuestionType.DrugAlcoholAbuse),
        nameof(HealthQuestionType.Respiratory),
        nameof(HealthQuestionType.HeartCirculation),
        nameof(HealthQuestionType.ChronicConditions),
        nameof(HealthQuestionType.Wellness),
        nameof(HealthQuestionType.ImmuneViral),
        nameof(HealthQuestionType.Senses)
    };

    private readonly ApplicationDbContext _db;

    public HealthDeclarationService(ApplicationDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public async Task<ApiResult<HealthDeclarationResponseDto>> UpsertAsync(string memberno,
        HealthDeclarationRequestDto request, CancellationToken cancellationToken = default)
    {
        // Guard: required request parts (validator should have run first)
        var errors = new List<ValidationError>();
        if (request.Vitals == null)
            return ApiResult<HealthDeclarationResponseDto>.ValidationFailed("Validation failed", new List<ValidationError> { new() { Field = "vitals", Error = "Vitals are required." } });
        if (request.HealthQuestion == null)
            return ApiResult<HealthDeclarationResponseDto>.ValidationFailed("Validation failed", new List<ValidationError> { new() { Field = "healthQuestion", Error = "Health question section is required." } });
        var customers= await _db.customers.Where(a=>a.MemberNo == memberno).FirstOrDefaultAsync();
        if (customers == null)
        {
             errors.Add(new ValidationError
            {
                Field = $"healthQuestion.answers question",
                Error = $"Please use a valid member number"
            });
             return ApiResult<HealthDeclarationResponseDto>.ValidationFailed("Validation failed", errors);
        }
        // Reject unknown question strings with 400
        var validationErrors = ValidateQuestionNames(request);
        if (validationErrors.Count > 0)
            return ApiResult<HealthDeclarationResponseDto>.ValidationFailed("Validation failed", validationErrors);

        // BMI = weightKg / (heightM^2), persist with 2 decimals
        var heightM = request.Vitals.HeightCm / 100m;
        var bmi = Math.Round(request.Vitals.WeightKg / (heightM * heightM), 2);
        var now = DateTime.UtcNow;
        var contextKey = string.IsNullOrWhiteSpace(request.ContextKey) ? null : request.ContextKey.Trim();

        CustomerHealthDeclaration declaration = await ResolveDeclarationAsync(customers.Id, request, bmi, now, contextKey, cancellationToken);

        // If healthDeclaration == false: save declaration only, do NOT insert answers.
        // If healthDeclaration == true: replace answers for that declaration (delete existing, insert new); dedupe by Question, last wins.
        if (request.HealthQuestion.HealthDeclaration && request.HealthQuestion.Answers != null && request.HealthQuestion.Answers.Count > 0)
        {
            var deduped = request.HealthQuestion.Answers
                .Where(a => Enum.TryParse<HealthQuestionType>(a.Question, ignoreCase: false, out _))
                .GroupBy(a => a.Question, StringComparer.OrdinalIgnoreCase)
                .Select(g => g.Last())
                .ToList();

            foreach (var a in deduped)
            {
                var question = Enum.Parse<HealthQuestionType>(a.Question, ignoreCase: false);
                // For answer == true: use narration (trimmed); for answer == false: normalize to NULL (discard text)
                var narration = a.Answer
                    ? (string.IsNullOrWhiteSpace(a.Narration) ? null : a.Narration!.Trim())
                    : null;

                _db.CustomerHealthDeclarationAnswers.Add(new CustomerHealthDeclarationAnswer
                {
                    Id = Guid.NewGuid(),
                    DeclarationId = declaration.Id,
                    Question = question,
                    Answer = a.Answer,
                    Narration = narration,
                    CreatedOn = now
                });
            }
        }

        await _db.SaveChangesAsync(cancellationToken);

        // Build response: triggeredConditions = items where answer is true, with question and narration
        var triggeredConditions = request.HealthQuestion.HealthDeclaration && request.HealthQuestion.Answers != null
            ? request.HealthQuestion.Answers
                .Where(x => x.Answer)
                .Select(x => new TriggeredConditionDto
                {
                    Question = x.Question,
                    Narration = string.IsNullOrWhiteSpace(x.Narration) ? null : x.Narration!.Trim()
                })
                .ToList()
            : new List<TriggeredConditionDto>();

        var dto = new HealthDeclarationResponseDto
        {
            CustomerId = declaration.CustomerId,
            ContextKey = declaration.ContextKey,
            DeclarationId = declaration.Id,
            HealthDeclaration = declaration.HealthDeclaration,
            HeightCm = declaration.HeightCm,
            WeightKg = declaration.WeightKg,
            Bmi = declaration.Bmi,
            TriggeredConditions = triggeredConditions,
            CreatedOn = declaration.CreatedOn,
            UpdatedOn = declaration.UpdatedOn
        };

        return ApiResult<HealthDeclarationResponseDto>.Ok(dto);
    }

    /// <summary>
    /// Validates that all question strings are one of the supported enum names. Returns a list of validation errors (empty if valid).
    /// </summary>
    private static List<ValidationError> ValidateQuestionNames(HealthDeclarationRequestDto request)
    {
        var errors = new List<ValidationError>();
        if (!request.HealthQuestion!.HealthDeclaration || request.HealthQuestion.Answers == null)
            return errors;

        var supported = new HashSet<string>(SupportedQuestionNames, StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < request.HealthQuestion.Answers.Count; i++)
        {
            var a = request.HealthQuestion.Answers[i];
            if (string.IsNullOrEmpty(a.Question)) continue;
            if (supported.Contains(a.Question)) continue;
            errors.Add(new ValidationError
            {
                Field = $"healthQuestion.answers[{i}].question",
                Error = $"Unknown question: '{a.Question}'. Supported: {string.Join(", ", SupportedQuestionNames)}."
            });
        }
        return errors;
    }

    /// <summary>
    /// Resolves the declaration entity: by (CustomerId, ContextKey) when ContextKey is not null;
    /// when ContextKey is null, idempotency by updating the latest record for that customer (else insert new).
    /// Ensures existing answers are removed when updating so they can be replaced.
    /// </summary>
    private async Task<CustomerHealthDeclaration> ResolveDeclarationAsync(
        string customerId,
        HealthDeclarationRequestDto request,
        decimal bmi,
        DateTime now,
        string? contextKey,
        CancellationToken cancellationToken)
    {
        if (contextKey != null)
        {
            var existing = await _db.CustomerHealthDeclarations
                .Include(x => x.Answers)
                .FirstOrDefaultAsync(x => x.CustomerId == customerId && x.ContextKey == contextKey, cancellationToken);

            if (existing != null)
            {
                existing.HeightCm = request.Vitals!.HeightCm;
                existing.WeightKg = request.Vitals.WeightKg;
                existing.Bmi = bmi;
                existing.HealthDeclaration = request.HealthQuestion!.HealthDeclaration;
                existing.UpdatedOn = now;
                _db.CustomerHealthDeclarationAnswers.RemoveRange(existing.Answers);
                return existing;
            }

            var newDecl = new CustomerHealthDeclaration
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                ContextKey = contextKey,
                HeightCm = request.Vitals!.HeightCm,
                WeightKg = request.Vitals.WeightKg,
                Bmi = bmi,
                HealthDeclaration = request.HealthQuestion!.HealthDeclaration,
                CreatedOn = now
            };
            _db.CustomerHealthDeclarations.Add(newDecl);
            return newDecl;
        }

        // ContextKey is null: fallback idempotency = update latest record per customer
        var latest = await _db.CustomerHealthDeclarations
            .Include(x => x.Answers)
            .Where(x => x.CustomerId == customerId && x.ContextKey == null)
            .OrderByDescending(x => x.CreatedOn)
            .FirstOrDefaultAsync(cancellationToken);

        if (latest != null)
        {
            latest.HeightCm = request.Vitals!.HeightCm;
            latest.WeightKg = request.Vitals.WeightKg;
            latest.Bmi = bmi;
            latest.HealthDeclaration = request.HealthQuestion!.HealthDeclaration;
            latest.UpdatedOn = now;
            _db.CustomerHealthDeclarationAnswers.RemoveRange(latest.Answers);
            return latest;
        }

        var newDeclNullCtx = new CustomerHealthDeclaration
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            ContextKey = null,
            HeightCm = request.Vitals!.HeightCm,
            WeightKg = request.Vitals.WeightKg,
            Bmi = bmi,
            HealthDeclaration = request.HealthQuestion!.HealthDeclaration,
            CreatedOn = now
        };
        _db.CustomerHealthDeclarations.Add(newDeclNullCtx);
        return newDeclNullCtx;
    }
}

using DAL.Model.HealthDeclaration;
using DAL.ModelView.HealthDeclaration;
using FluentValidation;

namespace API.Infrastructure.Application.HealthDeclaration;

public class HealthDeclarationRequestValidator : AbstractValidator<HealthDeclarationRequestDto>
{
    private static readonly HashSet<string> SupportedQuestions = new(StringComparer.OrdinalIgnoreCase)
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

    public HealthDeclarationRequestValidator()
    {
        RuleFor(x => x.Vitals).NotNull().WithMessage("Vitals are required.");
        RuleFor(x => x.Vitals)
            .ChildRules(v =>
            {
                v.RuleFor(x => x.HeightCm).GreaterThan(0).When(x => x != null).WithMessage("Height must be greater than 0.");
                v.RuleFor(x => x.HeightCm).LessThanOrEqualTo(300).When(x => x != null).WithMessage("Height must not exceed 300 cm.");
                v.RuleFor(x => x.WeightKg).GreaterThan(0).When(x => x != null).WithMessage("Weight must be greater than 0.");
                v.RuleFor(x => x.WeightKg).LessThanOrEqualTo(500).When(x => x != null).WithMessage("Weight must not exceed 500 kg.");
            }).When(x => x.Vitals != null);

        RuleFor(x => x.HealthQuestion).NotNull().WithMessage("Health question section is required.");
        RuleFor(x => x.HealthQuestion.HealthDeclaration).NotNull().When(x => x.HealthQuestion != null).WithMessage("Health declaration (yes/no) is required.");

        When(x => x.HealthQuestion != null && x.HealthQuestion.HealthDeclaration == true, () =>
        {
            RuleFor(x => x.HealthQuestion.Answers)
                .NotNull().WithMessage("Answers are required when health declaration is Yes.");
            RuleFor(x => x.HealthQuestion.Answers)
                .Must(a => a != null && a.Count > 0).WithMessage("At least one answer is required when health declaration is Yes.")
                .When(x => x.HealthQuestion?.HealthDeclaration == true);
            RuleForEach(x => x.HealthQuestion.Answers).SetValidator(new HealthAnswerValidator()).When(x => x.HealthQuestion?.Answers != null);
            RuleFor(x => x.HealthQuestion.Answers)
                .Must(a => a != null && a.Any(x => x.Answer)).WithMessage("At least one condition must be Yes when health declaration is Yes.")
                .When(x => x.HealthQuestion?.HealthDeclaration == true && x.HealthQuestion?.Answers != null);
        });

        // When healthDeclaration is false, answers can be null/empty; no extra rules needed.
    }
}

public class HealthAnswerValidator : AbstractValidator<HealthAnswerDto>
{
    private static readonly HashSet<string> SupportedQuestions = new(StringComparer.OrdinalIgnoreCase)
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

    public HealthAnswerValidator()
    {
        RuleFor(x => x.Question).NotEmpty().WithMessage("Question is required.");
        RuleFor(x => x.Question)
            .Must(q => SupportedQuestions.Contains(q!)).WithMessage("Unknown question. Supported: PriorDeclinedInsurance, ExistingConditions, DrugAlcoholAbuse, Respiratory, HeartCirculation, ChronicConditions, Wellness, ImmuneViral, Senses.")
            .When(x => !string.IsNullOrEmpty(x.Question));

        RuleFor(x => x.Narration)
            .NotEmpty().WithMessage("Narration is required when answer is Yes.")
            .When(x => x.Answer == true);
        RuleFor(x => x.Narration)
            .Must(n => string.IsNullOrWhiteSpace(n)).WithMessage("Narration must be empty when answer is No.")
            .When(x => x.Answer == false && x.Narration != null);
    }
}

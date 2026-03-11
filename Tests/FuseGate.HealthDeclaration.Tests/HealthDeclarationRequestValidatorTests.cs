using API.Infrastructure.Application.HealthDeclaration;
using DAL.ModelView.HealthDeclaration;
using FluentValidation.TestHelper;
using Xunit;

namespace FuseGate.HealthDeclaration.Tests;

public class HealthDeclarationRequestValidatorTests
{
    private readonly HealthDeclarationRequestValidator _validator = new();

    [Fact]
    public void Declaration_false_accepts_empty_answers()
    {
        var request = new HealthDeclarationRequestDto
        {
            Vitals = new VitalsDto { HeightCm = 170, WeightKg = 68 },
            HealthQuestion = new HealthQuestionDto { HealthDeclaration = false, Answers = null },
            ContextKey = null
        };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Declaration_false_accepts_empty_list_answers()
    {
        var request = new HealthDeclarationRequestDto
        {
            Vitals = new VitalsDto { HeightCm = 170, WeightKg = 68 },
            HealthQuestion = new HealthQuestionDto { HealthDeclaration = false, Answers = new List<HealthAnswerDto>() },
            ContextKey = null
        };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Declaration_true_rejects_empty_answers()
    {
        var request = new HealthDeclarationRequestDto
        {
            Vitals = new VitalsDto { HeightCm = 170, WeightKg = 68 },
            HealthQuestion = new HealthQuestionDto { HealthDeclaration = true, Answers = null },
            ContextKey = null
        };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.HealthQuestion!.Answers);
    }

    [Fact]
    public void Declaration_true_rejects_all_false_answers()
    {
        var request = new HealthDeclarationRequestDto
        {
            Vitals = new VitalsDto { HeightCm = 170, WeightKg = 68 },
            HealthQuestion = new HealthQuestionDto
            {
                HealthDeclaration = true,
                Answers = new List<HealthAnswerDto>
                {
                    new() { Question = "ExistingConditions", Answer = false, Narration = null },
                    new() { Question = "DrugAlcoholAbuse", Answer = false, Narration = null }
                }
            },
            ContextKey = null
        };
        var result = _validator.TestValidate(request);
        result.ShouldHaveAnyValidationError();
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("At least one condition must be Yes"));
    }

    [Fact]
    public void Declaration_true_rejects_missing_narration_when_answer_true()
    {
        var request = new HealthDeclarationRequestDto
        {
            Vitals = new VitalsDto { HeightCm = 170, WeightKg = 68 },
            HealthQuestion = new HealthQuestionDto
            {
                HealthDeclaration = true,
                Answers = new List<HealthAnswerDto>
                {
                    new() { Question = "ExistingConditions", Answer = true, Narration = null }
                }
            },
            ContextKey = null
        };
        var result = _validator.TestValidate(request);
        result.ShouldHaveAnyValidationError();
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Narration"));
    }

    [Fact]
    public void Declaration_true_accepts_valid_answers_with_narration()
    {
        var request = new HealthDeclarationRequestDto
        {
            Vitals = new VitalsDto { HeightCm = 170, WeightKg = 68 },
            HealthQuestion = new HealthQuestionDto
            {
                HealthDeclaration = true,
                Answers = new List<HealthAnswerDto>
                {
                    new() { Question = "ExistingConditions", Answer = true, Narration = "Hypertension since 2021." },
                    new() { Question = "DrugAlcoholAbuse", Answer = false, Narration = null }
                }
            },
            ContextKey = "Application:12345"
        };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Rejects_unknown_question_string()
    {
        var request = new HealthDeclarationRequestDto
        {
            Vitals = new VitalsDto { HeightCm = 170, WeightKg = 68 },
            HealthQuestion = new HealthQuestionDto
            {
                HealthDeclaration = true,
                Answers = new List<HealthAnswerDto>
                {
                    new() { Question = "InvalidQuestion", Answer = true, Narration = "Some text." }
                }
            },
            ContextKey = null
        };
        var result = _validator.TestValidate(request);
        result.ShouldHaveAnyValidationError();
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Unknown question") || e.ErrorMessage.Contains("Supported:"));
    }

    [Fact]
    public void Rejects_vitals_height_out_of_range()
    {
        var request = new HealthDeclarationRequestDto
        {
            Vitals = new VitalsDto { HeightCm = 0, WeightKg = 68 },
            HealthQuestion = new HealthQuestionDto { HealthDeclaration = false, Answers = null },
            ContextKey = null
        };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor("Vitals.HeightCm");
    }

    [Fact]
    public void Rejects_vitals_weight_over_500()
    {
        var request = new HealthDeclarationRequestDto
        {
            Vitals = new VitalsDto { HeightCm = 170, WeightKg = 501 },
            HealthQuestion = new HealthQuestionDto { HealthDeclaration = false, Answers = null },
            ContextKey = null
        };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor("Vitals.WeightKg");
    }
}

using API.Infrastructure.Application.HealthDeclaration;
using DAL;
using DAL.Model.HealthDeclaration;
using DAL.ModelView.HealthDeclaration;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FuseGate.HealthDeclaration.Tests;

public class HealthDeclarationServiceTests
{
    private static ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task BMI_calculation_is_correct()
    {
        await using var db = CreateDbContext();
        var service = new HealthDeclarationService(db);
        var request = new HealthDeclarationRequestDto
        {
            Vitals = new VitalsDto { HeightCm = 170, WeightKg = 68 },
            HealthQuestion = new HealthQuestionDto { HealthDeclaration = false, Answers = null },
            ContextKey = "App:1"
        };
        var result = await service.UpsertAsync("cust-1", request);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        // BMI = 68 / (1.7 * 1.7) = 23.529...
        Assert.True(result.Data.Bmi > 23.5m && result.Data.Bmi < 23.6m);
    }

    [Fact]
    public async Task Dedup_question_handling_last_wins()
    {
        await using var db = CreateDbContext();
        var service = new HealthDeclarationService(db);
        var request = new HealthDeclarationRequestDto
        {
            Vitals = new VitalsDto { HeightCm = 170, WeightKg = 68 },
            HealthQuestion = new HealthQuestionDto
            {
                HealthDeclaration = true,
                Answers = new List<HealthAnswerDto>
                {
                    new() { Question = "ExistingConditions", Answer = true, Narration = "First" },
                    new() { Question = "ExistingConditions", Answer = true, Narration = "Second wins" }
                }
            },
            ContextKey = "App:Dedup"
        };
        var result = await service.UpsertAsync("cust-dedup", request);
        Assert.True(result.Success);
        Assert.Single(result.Data!.TriggeredConditions);
        Assert.Equal("Second wins", result.Data.TriggeredConditions[0].Narration);
    }

    [Fact]
    public async Task Upsert_with_contextKey_updates_existing()
    {
        await using var db = CreateDbContext();
        var service = new HealthDeclarationService(db);
        var request1 = new HealthDeclarationRequestDto
        {
            Vitals = new VitalsDto { HeightCm = 170, WeightKg = 68 },
            HealthQuestion = new HealthQuestionDto { HealthDeclaration = false, Answers = null },
            ContextKey = "Application:12345"
        };
        var result1 = await service.UpsertAsync("cust-ctx", request1);
        Assert.True(result1.Success);
        var id1 = result1.Data!.DeclarationId;

        var request2 = new HealthDeclarationRequestDto
        {
            Vitals = new VitalsDto { HeightCm = 180, WeightKg = 80 },
            HealthQuestion = new HealthQuestionDto { HealthDeclaration = false, Answers = null },
            ContextKey = "Application:12345"
        };
        var result2 = await service.UpsertAsync("cust-ctx", request2);
        Assert.True(result2.Success);
        Assert.Equal(id1, result2.Data!.DeclarationId);
        Assert.Equal(180, result2.Data.HeightCm);
        Assert.Equal(80, result2.Data.WeightKg);
        Assert.NotNull(result2.Data.UpdatedOn);
    }

    [Fact]
    public async Task Declaration_false_saves_no_answers()
    {
        await using var db = CreateDbContext();
        var service = new HealthDeclarationService(db);
        var request = new HealthDeclarationRequestDto
        {
            Vitals = new VitalsDto { HeightCm = 170, WeightKg = 68 },
            HealthQuestion = new HealthQuestionDto { HealthDeclaration = false, Answers = null },
            ContextKey = "App:NoAnswers"
        };
        var result = await service.UpsertAsync("cust-no-answers", request);
        Assert.True(result.Success);
        Assert.Empty(result.Data!.TriggeredConditions);
        var declaration = await db.CustomerHealthDeclarations.Include(x => x.Answers).FirstAsync(x => x.Id == result.Data.DeclarationId);
        Assert.Empty(declaration.Answers);
    }

    [Fact]
    public async Task Unknown_question_returns_validation_error()
    {
        await using var db = CreateDbContext();
        var service = new HealthDeclarationService(db);
        var request = new HealthDeclarationRequestDto
        {
            Vitals = new VitalsDto { HeightCm = 170, WeightKg = 68 },
            HealthQuestion = new HealthQuestionDto
            {
                HealthDeclaration = true,
                Answers = new List<HealthAnswerDto>
                {
                    new() { Question = "InvalidQuestion", Answer = true, Narration = "Text" }
                }
            },
            ContextKey = null
        };
        var result = await service.UpsertAsync("cust-1", request);
        Assert.False(result.Success);
        Assert.NotNull(result.Errors);
        Assert.Contains(result.Errors, e => e.Error.Contains("Unknown question") || e.Error.Contains("Supported:"));
    }
}

namespace DAL.ModelView.HealthDeclaration;

public class HealthDeclarationRequestDto
{
    public VitalsDto Vitals { get; set; } = null!;
    public HealthQuestionDto HealthQuestion { get; set; } = null!;
    public string? ContextKey { get; set; }
}

public class VitalsDto
{
    public decimal HeightCm { get; set; }
    public decimal WeightKg { get; set; }
}

public class HealthQuestionDto
{
    public bool HealthDeclaration { get; set; }
    public List<HealthAnswerDto>? Answers { get; set; }
}

public class HealthAnswerDto
{
    public string Question { get; set; } = null!; // enum name, e.g. "ExistingConditions"
    public bool Answer { get; set; }
    public string? Narration { get; set; }
}

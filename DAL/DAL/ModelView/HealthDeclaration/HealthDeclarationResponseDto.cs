namespace DAL.ModelView.HealthDeclaration;

public class HealthDeclarationResponseDto
{
    public string CustomerId { get; set; } = null!;
    public string? ContextKey { get; set; }
    public Guid DeclarationId { get; set; }
    public bool HealthDeclaration { get; set; }
    public decimal HeightCm { get; set; }
    public decimal WeightKg { get; set; }
    public decimal Bmi { get; set; }
    public List<TriggeredConditionDto> TriggeredConditions { get; set; } = new();
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
}

public class TriggeredConditionDto
{
    public string Question { get; set; } = null!;
    public string? Narration { get; set; }
}

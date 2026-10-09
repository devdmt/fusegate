namespace DAL.Model.HealthDeclaration;

public class CustomerHealthDeclarationAnswer
{
    public Guid Id { get; set; }
    public Guid DeclarationId { get; set; }
    public HealthQuestionType Question { get; set; }
    public bool Answer { get; set; }
    public string? Narration { get; set; }
    public DateTime CreatedOn { get; set; }

    public virtual CustomerHealthDeclaration Declaration { get; set; } = null!;
}

namespace DAL.Model.HealthDeclaration;

public class CustomerHealthDeclaration
{
    public Guid Id { get; set; }
    public string CustomerId { get; set; } = null!;
    public string? ContextKey { get; set; }
    public decimal HeightCm { get; set; }
    public decimal WeightKg { get; set; }
    public decimal Bmi { get; set; }
    public bool HealthDeclaration { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }

    public virtual ICollection<CustomerHealthDeclarationAnswer> Answers { get; set; } = new List<CustomerHealthDeclarationAnswer>();
}

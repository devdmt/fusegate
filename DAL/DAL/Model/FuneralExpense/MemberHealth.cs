using System.ComponentModel.DataAnnotations;

namespace DAL.Model.FuneralExpense;

public class MemberHealth
{
    [Key]
    public string Id { get; set; }
    [Required]
    public string MemberId { get; set; }
    [Required]
    public string QuoteId { get; set; }
    [Required]
    public decimal Height { get; set; }
    [Required]
    public decimal Weight { get; set; }
    [Required]
    public bool PriorDeclinedInsurance { get; set; }
    [Required]
    public bool ExistingConditions { get; set; }
    [Required]
    public bool DrugOrAlcoholAbuse { get; set; }
    [Required]
    public bool Respiratory { get; set; }
    [Required]
    public bool HeartOrCirculation { get; set; }
    [Required]
    public bool ChronicConditions { get; set; }
    [Required]
    public bool Wellness { get; set; }
    [Required]
    public bool ImmuneOrViral { get; set; }
    [Required]
    public bool Senses { get; set; }
}

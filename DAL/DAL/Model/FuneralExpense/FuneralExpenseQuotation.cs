using DAL.ModelView.Flex;
using System.ComponentModel.DataAnnotations;

namespace DAL.Model.LastExpense;

public class FuneralExpenseQuotation
{
    [Key]
    public string? QuoteId { get; set; }
    [Required]
    public Productenum PolicyType { get; set; }
    [Required]
    public string? PartnerId { get; set; }
    [Required]
    public string? CustomerPhone { get; set; }
    [Required]
    public double SumAssured { get; set; }
    [Required]
    public double CoverPremium { get; set; }
    [Required]
    public double CompensationLevy { get; set; }
    [Required]
    public double PolicyFee { get; set; }
    [Required]  
    public double  TotalPremium { get; set; }
    [Required]
    public DateTime DateOfBirth { get; set; }

    [Required]
    public int PolicyTerm { get; set; }
    //[Required]
    //public int TermId { get; set; }

    [Required]
    public Frequency? PaymentFrequency { get; set; }
    public decimal? Maturity { get; set; } = 0;
    public decimal? NaturalDeath { get; set; } = 0;
    public decimal? AccidentialDeath { get; set; } = 0;
    public decimal? CriticalIllness { get; set; } = 0;
    public decimal? PTDNatural { get; set; } = 0;
    public decimal? PTDAccidental { get; set; } = 0;
    
    public string? AgentId { get; set; }
  
    public string? CallbackUrl { get; set; }
}

public class FuneralExpenseBenefit
{
    public decimal? Maturity { get; set; } = 0;
    public decimal? NaturalDeath { get; set; } = 0;
    public decimal? AccidentialDeath { get; set; } = 0;
    public decimal? CriticalIllness { get; set; } = 0;
    public decimal? PTDNatural { get; set; } = 0;
    public decimal? PTDAccidental { get; set; } = 0;
}
    
public class OTPOnboarding
{
    [Key]
    public string Id { get; set; }
    // public virtual Customer? Customer { get; set; } = default!;
    public string? CustomerPhoneNumber { get; set; } = default!;
    public string? MemberId { get; set; } = default!;
    public string? Code { get; set; } = default!;
    public string? Message { get; set; } = default!;
    public string? EmailPlaceHolder { get; set; } = default!;
    public bool? IsUsed { get; set; } = false!;
    public bool? ISsent { get; set; } = false!;

    public DateTime? CreatedAt { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime? ExpiredAt { get; set; }
    public DateTime? UsedAt { get; set; }
    public string? ErrorMessage { get; set; }


}

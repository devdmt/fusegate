using DAL.Model;
using DAL.ModelView.Flex;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ModelView.FuneralExpense;

public class FuneralExpenseQuotationDto
{
    [Required]
    public Productenum PolicyType { get; set; }
    [Required]
    public string PartnerId { get; set; }
    [Required]
    public string CustomerPhone { get; set; }
    [Required]
    public double    SumAssured { get; set; }
    [Required]
    public int? DeathBenefits { get; set; }
    [Required]
    public DateTime DateOfBirth { get; set; }
    [Required]
    public int Age { get; set; }

    [Required]
    public int PolicyTerm { get; set; } 
    [Required]
    public int TermId { get; set; }

    [Required]
    public Frequency PaymentFrequency { get; set; }
    [Required]
    public string AgentId { get; set; }
    [Required]
    public string CallbackUrl { get; set; }
    public List<RiderType>? Riders { get; set; } = new List<RiderType>();
}


public class FuneralExpenseQuotationResponseDto
{
    public string? QuoteId { get; set; }
    public double? SumAssured { get; set; }
    public double? CoverPremium { get; set; }
    public double? CompensationLevy { get; set; }
    public double? PolicyFee { get; set; }
    public double? TotalPremium { get; set; }
    public double? Maturity { get; set; } = 0;
    public double? NaturalDeath { get; set; } = 0;
    public double? AccidentialDeath { get; set; } = 0;
    public double? CriticalIllness { get; set; } = 0;
    public double? PTDNatural { get; set; } = 0;
    public double? PTDAccidental { get; set; } = 0;
    public List<RiderAmount> Riders { get; set; } = new();
    public string? Message { get; set; }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ModelView.FuneralExpense;

public class FuneralExpenseOnboardingDto
{
    [Required]
    public string QuoteId { get; set; }
    [Required]
    public string FullName { get; set; }
    [Required]
    public string IdNumber { get; set; }

    /// <summary>
    /// e.g. Male, Female, Other
    /// </summary>
    [Required]
    public Gender Gender { get; set; }
    [Required]
    public string Nationality { get; set; }

    /// <summary>
    /// Country of residence
    /// </summary>
    [Required]
    public string Residency { get; set; }

    [Required]
    public string Email { get; set; }

    [Required]
    public string Occupation { get; set; }

    /// <summary>
    /// e.g. "50,000 - 100,000"
    /// </summary>
    [Required]
    public string MonthlyIncomeRange { get; set; }

    [Required]
    public string ProductId { get; set; }

    [Required]
    public string PartnerId { get; set; }

    
    public string? AgentId { get; set; }

    public string? CallbackUrl { get; set; }
}

public class FuneralExpenseOnboardingResponseDto
{
    public string? MemberId { get; set; }
    public string? QuoteId { get; set; }
    public decimal SumAssured { get; set; }
    public string? Message { get; set; }
}

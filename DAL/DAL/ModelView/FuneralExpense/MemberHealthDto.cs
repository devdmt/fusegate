using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ModelView.FuneralExpense;

public class MemberHealthDto
{
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

public class MemberHealthResponseDto
{
    public string? TransactionId { get; set; }
    public string? QuoteId { get; set; }
    public string? Message { get; set; }
}

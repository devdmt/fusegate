using DAL.ModelView;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DAL.Model.FuneralExpense;

public class FuneralExpenseOnboarding
{
    [Key]
    public string? MemberId { get; set; }
    [Required]
    public string? QuoteId { get; set; }
    [Required]
    public string? FullName { get; set; }
    [Required]
    public string? IdNumber { get; set; }

    [Required]
    public Gender Gender { get; set; }
    [Required]
    public string? Nationality { get; set; }

   
    [Required]
    public string? Residency { get; set; }

    [Required]
    public string? Email { get; set; }

    [Required]
    public string? Occupation { get; set; }
    public PolicyStatus PolicyStatus { get; set; }

    
    [Required]
    public string? MonthlyIncomeRange { get; set; }

    [Required]
    public string? ProductId { get; set; }

    [Required]
    public string? PartnerId { get; set; }

    [Required]
    public string? AgentId { get; set; }

    public string? SignatureBase64 { get; set; }

    public string? CallbackUrl { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PolicyStatus
{
    [EnumMember(Value = "Pending")]
    Pending,
    [EnumMember(Value = "PaidUp")]
    PaidUp, 
    [EnumMember(Value = "Active")]
    Active,
    [EnumMember(Value = "Lapsed")]
    Lapsed

}

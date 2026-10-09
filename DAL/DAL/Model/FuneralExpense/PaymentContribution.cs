using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model.FuneralExpense;

public class PaymentContribution
{
    [Key]
    public string Id { get; set; }
    [Required]
    public string MemberId { get; set; }

    [Required]
    public string ProductId { get; set; }

    [Required]
    public List<MemberPayment> Payments { get; set; } // e.g. MPESA, CARD, BANK

    [Url]
    public string? CallbackUrl { get; set; }
}

public class MemberPayment
{
    [Key]
    public string Id { get; set; }
    [Required]
    public string PaymentContributionId { get; set; }
    [Required]
    public PaymentMode PaymentMode { get; set; }
    [Required]
    public string PaymentReference { get; set; }
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }
    [Required]
    [Phone]
    public string MpesaNumber { get; set; }

}

public enum PaymentMode
{
    Mpesa,
    Card,
    BankTransfer
}

using DAL.Model.FuneralExpense;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ModelView.FuneralExpense;

public class PaymentContributionDto
{
    [Required]
    public string MemberId { get; set; }

    [Required]
    public string ProductId { get; set; }

    [Required]
    public List<MemberPaymentDto> Payments { get; set; } // e.g. MPESA, CARD, BANK

    [Url]
    public string? CallbackUrl { get; set; }
}

public class MemberPaymentDto
{
   
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

public class PaymentContributionResponseDto
{
    public string? TransactioId { get; set; }
    public string? Message { get; set; }
}

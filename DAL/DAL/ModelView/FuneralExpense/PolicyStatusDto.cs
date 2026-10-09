using DAL.Model.FuneralExpense;
using System.ComponentModel.DataAnnotations;

namespace DAL.ModelView.FuneralExpense;

public class PolicyStatusDto
{
    [Required]
    public string MemberId { get; set; } = default!;

    [Required]
    public string PolicyType { get; set; } = default!;

    [Required]
    public string ProductId { get; set; } = default!;

    [Required]
    public string PartnerId { get; set; } = default!;
}

public class PolicyResponseDto
{
    public string MemberId { get; set; } = default!;
    public PolicyStatus PolicyStatus { get; set; }
    public string Message { get; set; } = default!;
}

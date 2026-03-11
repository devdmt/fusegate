using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ModelView.FuneralExpense;

public class PolicyActivationDto
{
    [Required]
    public string MemberId { get; set; }
    [Required]
    public string SignatureBase64 { get; set; }
}

public class PolicyActivationResponseDto
{
    public string? MemberId { get; set; }
    public string? Otp { get; set; }
    public string? Message { get; set; }
}

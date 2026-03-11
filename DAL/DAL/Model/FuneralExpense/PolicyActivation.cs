using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model.FuneralExpense;

public class PolicyActivation
{
    [Key]
    public string Id { get; set; }

    [Required]
    public string MemberId { get; set; }
    [Required]
    public string SignatureBase64 { get; set; }
}

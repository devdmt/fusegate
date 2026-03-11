using DAL.Model.FuneralExpense;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ModelView.FuneralExpense;

public class BeneficiaryDto
{
    [Required]
    public string MemberId { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public Gender Gender { get; set; }

    [Required]
    public DateTime DateOfBirth { get; set; }

    [Required]
    public List<GuardianDto> Guardians { get; set; }
}

public class GuardianDto
{
   
    [Required]
    public string Name { get; set; }

    [Required]
    public string Relationship { get; set; }

    [Required]
    public Gender Gender { get; set; }

    [Required]
    [Phone]
    public string Phone { get; set; }

    [Required]
    public string IdNumber { get; set; }

    [Required]
    [Range(0.01, 100)]
    public decimal SharePercentage { get; set; }
}

public class BeneficiaryResponseDto
{
    public string BeneficiaryId { get; set; }
    public string Message { get; set; }
}


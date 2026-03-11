using DAL.Model;
using DAL.ModelView.Pension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ModelView
{
    public class CustomerBIODTO
    {

  
    [Required(ErrorMessage = "Customer Name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "OtherName required.")]
        public string OtherNames { get; set; } // Optional derived field
    public string? DateOfBirth { get; set; }
    public Productenum ProductTypes { get; set; }
    public string? IDNumber { get; set; }
    public IDType IdType { get; set; } = IDType.NationalID;
    public Gender? Gender { get; set; }
    // Contact
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    // Demographics
    public string? Nationality { get; set; }
    public string? Residency { get; set; }
    public string? Occupation { get; set; }
    public string? EmploymentTerms { get; set; }
    // Tax & Compliance
    public string? TaxIdNumber { get; set; }
    public string? Pin { get; set; }
    public string? USAddress { get; set; }
    // Business / Employer Info
    public string? EmployerName { get; set; }
    public string? BusinessName { get; set; }
    public string? NatureOfBusiness { get; set; }
    public string? RoleInBusiness { get; set; }
    public string? Role { get; set; }
    // Income
    public string? SourceOfIncome { get; set; }
    public string? AdditionalSourceOfIncome { get; set; }
    public int? AverageIncome { get; set; }

    // Group / Membership
  //  public bool? IsGroupMember { get; set; }

    // Documents & Signatures
  //  public string? Signature { get; set; }
    }
}

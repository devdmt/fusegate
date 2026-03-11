using DAL.ModelView;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class PensionerBalanceRequest
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public long ProductId { get; set; }
        public string? MemberCode { get; set; }
        [MaxLength(50)]
        public string? PartnerCode { get; set; }
        [MaxLength(10)]
        public string OtpCode { get; set; }
        public DateTime GeneratedOn { get; set; }
        public DateTime ExpireOn { get; set; }
        public bool IsUsed { get; set; } = false;
        public DateTime? UsedAt { get; set; }
        public DateTime? AuthorisedOn { get; set; }
        [MaxLength(50)]
        public string RequestRef { get; set; }
        [MaxLength(50)]
        public string PartnerId { get; set; }
        [MaxLength(50)]
        public string AgentCode { get; set; }
        public bool RequestCompleted { get; set; } = false;
        public  bool RequestFailed { get; set; } = false;
        public bool RequestAuthorised { get; set; } = false;
    }

        public class PensionerBeneficiaries
    {
        public long Id { get; set; }
      
        public Guid CustomerId { get; set; }
         public string? Surname { get; set; }
        public string? Firstname { get; set; }
        public string? OtherNames { get; set; }
        public string? EmailAddress { get; set; }
        public Gender? Gender { get; set; }
        public string? Phone { get; set; }
        public int? Percentage { get; set; }
        public string? Relationship { get; set; }
        public string? IdNumber { get; set; }
        public string? Date_of_birth { get; set; }
        public bool Confirmed { get; set; } = false;
        public bool IsDeleted { get; set; }
        public bool IsLocked { get; set; }
        public string? PartnerBeneficiaryCode { get; set; }
       
    }
    public class PensionerGurdian
    {
        public long Id { get; set; }
       
        public Guid? CustomerId { get; set; }
        public virtual Beneficiaries Beneficiary { get; set; } = default!;
        public long BeneficiaryId { get; set; }
        public string? Surname { get; set; }
        public string? OtherNames { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? DOB { get; set; }
        public Gender? Gender { get; set; }
        public string? IdNumber { get; set; }
        public string? IdType { get; set; }
        public string? Relationship { get; set; }
        public string? GuardianCode { get; set; } 
    }
}



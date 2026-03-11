using System;
using DAL.ModelView;

namespace DAL.Model
{
    public class Guardian:Auditable
    {
        public long Id { get; set; }
        public Guid CustomerId { get; set; }
        public long BeneficiaryId { get; set; }
        public string? FirstName { get; set; }
        public string? Surname { get; set; }
        public string? OtherNames { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? DateOfBirth { get; set; }

        public Gender? Gender { get; set; }

        public string? IdNumber { get; set; }
        public string? IdType { get; set; }
        public string? Relationship { get; set; }
        public string? Status { get; set; }

        public string? PartnerResponseDesc { get; set; }
        public string? PartnerResponseCode { get; set; }

        public bool? Approved { get; set; }
    }
}

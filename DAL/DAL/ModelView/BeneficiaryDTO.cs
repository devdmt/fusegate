namespace DAL.ModelView
{
    public class BeneficiaryCreateDTO
    {
         public string Memberno { get; set; } = null!;
        public string? BeneficiaryCode { get; set; }
        public string? Firstname { get; set; }
        public string? OtherNames { get; set; }
        
        public string? Surname { get; set; }
        public string? EmailAddress { get; set; }

        public Gender? Gender { get; set; }

        public string? Phone { get; set; }
        public int? Percentage { get; set; }
        public string? Relationship { get; set; }
        public string? IdNumber { get; set; }
        public string? DateOfBirth { get; set; }
         public GuardianCreateDTO? Guardian { get; set; }
    }
    public class BeneficiaryDTO 
    {
      
        public string Memberno { get; set; } = null!;
        public string? BeneficiaryCode { get; set; }
        public string? Firstname { get; set; }
        public string? OtherNames { get; set; }
        public string? EmailAddress { get; set; }

        public Gender? Gender { get; set; }

        public string? Phone { get; set; }
        public int? Percentage { get; set; }
        public string? Relationship { get; set; }
        public string? IdNumber { get; set; }
        public string? DateOfBirth { get; set; }
        public string? Status { get; set; }
        public GuardianDTO? Guardian { get; set; }
        public string? PartnerResponseDesc { get; set; }
        public string? PartnerResponseCode { get; set; }
        public string? Approved { get; set; }

        public bool Confirmed { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsLocked { get; set; }

        public string? Surname { get; set; }
    }
}

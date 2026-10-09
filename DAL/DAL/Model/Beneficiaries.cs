using System;
using DAL.ModelView;

namespace DAL.Model
{
    public class Beneficiaries:Auditable
    {
        public long Id { get; set; }
        public string CustomerId { get; set; } = null!;
        public string? MemberNo { get; set; }
        public string? PartnerBeneficiaryCode { get; set; }
        public string? Firstname { get; set; }
        public string? OtherNames { get; set; }
        public string? EmailAddress { get; set; }
        public string? PartnerCode { get; set; }    
        public Gender? Gender { get; set; }

        public string? Phone { get; set; }
        public decimal? Percentage { get; set; }
        public string? Relationship { get; set; }
        public string? IdNumber { get; set; }
        public string? Date_of_birthRaw  { get; set; }
        public string? Date_of_birth { get; set; }
        public int? Age  { get; set; }
        public string? Status { get; set; }
        public Guardian? Guardian { get; set; }
        public string? PartnerResponseDesc { get; set; }
        public string? PartnerResponseCode { get; set; }
        public string? Approved { get; set; }

        public bool Confirmed { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsLocked { get; set; }

        public string? Surname { get; set; }
    }

      public class KLGuardian
    {
    public string? Name {get;set;}
    public string? Email {get;set;}
    public string? Phonenumber {get;set;}
    public string? IdNumber {get;set;}
    public string? DOB {get;set;}  
    public string? customerCode {get;set;}
    public string? RelationShip {get;set;}

    }
    public class KLBeneficiary
    {
        
    public string? Name {get;set;}
    public string? Email {get;set;}
    public string? Phonenumber {get;set;}
    public string? IdNumber {get;set;}
    public string? DOB {get;set;}    
    public string? customerCode {get;set;}
    public string? Proportion {get;set;}
    public string? RelationShip {get;set;}
}
}



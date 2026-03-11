using System;

namespace DAL.ModelView.Pension
{
    /// <summary>
    /// Response for balance request initiation. Does not include OTP (sent via SMS/email elsewhere).
    /// </summary>
    public class BalanceRequestResponse
    {
        public Guid RequestId { get; set; }
        public string MaskedPhoneNumber { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
    public class CompleteBalanceResponse
    {
              
        public decimal Employeefunds { get; set; }
        public decimal Employerfunds { get; set; }
        public decimal Totalfunds { get; set; }
        public decimal EVC_Contribution { get; set; }
        public string? DeclaredInterest { get; set; }

    }
     public class STKContributionDTO
    {
        public string PensionerId { get; set; }
        public double Amount { get; set; }
        public string Phonenumber { get; set; } 
        public bool Salary { get; set; }
        public string? TrnCode { get; set; }
        public string? MemberNo { get; set; }
        public CustomerType AccountType { get; set; } = CustomerType.Individual;
        public bool ProcessBatch { get; set; } = false;
    }
   
}

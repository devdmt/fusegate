namespace DAL.ModelView
{
    public class BeneficiaryResultDto
    {
        public string BeneficiaryCode { get; set; }
        public string CustomerId { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public decimal Percentage { get; set; }
        public int? Age { get; set; }
        public bool GuardianSaved { get; set; }
    }
}
        
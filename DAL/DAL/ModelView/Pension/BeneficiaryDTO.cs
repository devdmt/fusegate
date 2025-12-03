using System;


namespace DAL.ModelView.Pension
{
    public class BeneficiaryDTO
    {
        public string CustomerId { get; set; }
        public string RequestId { get; set; }
        public string BeneficiaryName { get; set; }
        public string BeneficiaryDOB { get; set; }
        public string BeneficiaryGender { get; set; }
        public string Relationship { get; set; }
        public string Contact { get; set; }
       
    }
    public class BeneficiaryAcknowledge
    {
        public string RequestId { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMsg { get; set; }
        public bool accepted { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ModelView.Pension
{
    public class PostContributionDTO
    {
        public string CustomerId { get; set; }
        public string TransactionId { get; set; }  
        public string PaymentRecieptNumber { get; set; }
        public double Amount { get; set; }
        public string PaymentMode { get; set; }
        public string PaymentDateTime { get; set; } 
        public string callBackUrl { get; set; }
    }
    public class AcknowledgeContributionDTO
    {
        public string OriginalTransactionId { get; set; }
        public string TransactionId { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMsg { get; set; }
        public string Status { get; set; }
    }

    public class ContributionResultDTO
    {
        public string OriginalTransactionId { get; set; }
        public string TransactionId { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMsg { get; set; }
        public string Status { get; set; }
        public string CompletedDateTime { get; set; }
        public string CurrentCharge { get; set; }
        public double TotalContribution { get; set; }   


    }
}

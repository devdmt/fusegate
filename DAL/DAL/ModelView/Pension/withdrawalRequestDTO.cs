using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ModelView.Pension
{
    public class withdrawalRequestDTO
    {
        public string CustomerId { get; set; }
        public double Amount { get; set; }
        public string TransactionId { get; set; }
        public string callBackUrl { get; set; }
        public string  RequestDate { get; set; }
    }

    public class AcknowledgewithdrawalDTO
    {
        public string OriginalTransactionId { get; set; }
        public string TransactionId { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMsg { get; set; }
    }

    public class WithdrawalResultDTO
    {
        public string OriginalTransactionId { get; set; }
        public string TransactionId { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMsg { get; set; }
        public double ApprovedAmount { get; set; }
        public string Status { get; set; }
        public string CompletedDateTime { get; set; }
        public string CurrentCharge { get; set; }
        public double ContributionBalance { get; set; }
    }
}

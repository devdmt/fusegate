using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ModelView
{
    public class MpesaTokenDTO
    {
        public string? access_token { get; set; }
        public string? expires_in { get; set; }
    }

    public class STkPushRequestDTO
    {
        public string BusinessShortCode { get; set; }
        public string Password { get; set; }
        public string Timestamp { get; set; }
        public string TransactionType { get; set; }
        public string Amount { get; set; }
        public string PartyA { get; set; }
        public string PartyB { get; set; }
        public string PhoneNumber { get; set; }
        public string CallBackURL { get; set; }
        public string AccountReference { get; set; }
        public string TransactionDesc { get; set; }
    }
    public class STKResult
    {

        public string? ResultCode { get; set; }
        public string? ResultDesc { get; set; }
        public string? Amount { get; set; }
        public string? MpesaReceiptNumber { get; set; }
        public string? AccountReference { get; set; }
        public bool? Success { get; set; } = false;
        public bool? Finalized { get; set; } = false;
    }
    public class AcknowledgementDTO
    {

        public string? MerchantID { get; set; }
        public string? CheckoutRequestID { get; set; }
        public string? ResponseCode { get; set; }
        public string? ResponseDescription { get; set; }
        public string? CustomerMessage { get; set; }
        public string? RequestId { get; set; }
        public string? ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
        public bool Success { get; set; } = false;
    }
    public class PaymentRequestDTO
    {
        public long? productId { get; set; }
        public long? customerId { get; set; }
        public string? phoneNumber { get; set; }
        public string? trnCode { get; set; }
        public double? Amount { get; set; }

    }

    public class STKResponseErrorDTO
    {
        public string requestId { get; set; }
        public string errorCode { get; set; }
        public string errorMessage { get; set; }
    }
    public class STKResponseDTO
    {
        public string MerchantRequestID { get; set; }
        public string CheckoutRequestID { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseDescription { get; set; }
        public string CustomerMessage { get; set; }
    }
}

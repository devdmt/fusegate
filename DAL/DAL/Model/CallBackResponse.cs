using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class CallBackResponse
    {
        public Guid Id { get; set; }
        public string PartnerCode { get; set; }
        public string PartnerId { get; set; }
        public Productenum Productenum { get; set; }
        public string RequestId { get; set; }
        public string CorrelationId { get; set; }
        public string RequestType { get; set; }
        public string Request { get; set; }
        public string? Response { get; set; }
        public bool Responded { get; set; }
        public bool ProcessResponse { get; set; }
        public bool CallbackProcessed { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? RespondedAT { get; set; }
        public string CallbackUrl { get; set; }
         public string? Callbackerror { get; set; }
        public string ? Status { get; set; }
        public string StatusMessage { get; set; } = string.Empty;

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class OTP
    {
            public Guid Id { get; set; }
            public string OTPCode { get; set; }
            public bool IsActive { get; set; }
            public DateTime DateInitiated { get; set; }
            public string? Channel { get; set; }
            public string? ModuleAccessed { get; set; }
            public DateTime ExpiresOn { get; set; }
            public bool Sent { get; set; }
            public string? PRSPResponse { get; set; }
            public bool Validated { get; set; } = false;
            public string? DeviceId { get; set; }
            public DateTime? ValidatedOn { get; set; }
            public string? SMSResponseJson { get; set; }
        
    }
}

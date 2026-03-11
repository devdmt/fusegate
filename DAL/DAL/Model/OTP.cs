using DAL.ModelView.Pension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class OTP
    {
        public Guid Id { get; set; }
  
        public string CustomerId { get; set; }
          public string? ProductRef { get; set; }
        public string Code { get; set; } = default!;
        public string Phonenumber { get; set; }
        public string? Message { get; set; } = default!;
        public string? EmailPlaceHolder { get; set; } = default!;
        public bool? ISSMSSent { get; set; } = false!;
        public bool? IsEmailSent { get; set; } = false!; 
        public bool? isEmailPicked { get; set; } = false!;
        public bool IsUsed { get; set; } = false!;
        public bool ISsent { get; set; }= false!;
        public bool? LinkGenerated { get; set; }= false!;
        public DateTime? CreatedAt { get; set; }
        public DateTime? SentAt { get; set; }
        public DateTime? ExpiredAt { get; set; }
        public DateTime? UsedAt { get; set; }
        public string? UsedBy { get; set; }
        public NotificationType notificationType { get; set; }
        public string? Response { get; set; } = default!;
        public string? ErrorMessage { get; set; }
        public string? LinkCode { get; set; }
        public string? SentError { get; set; }
        public int? SendTrial { get; set; } = 0;
        public string? EmailDisplayName { get; set; }
        public int? EmailTemplateType { get; set; } = 0;
   
    }



    public enum NotificationType
    {
        Email,
        SMS, ALL
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace DAL.Model
{
    public class Contribution
    {
        public Guid Id { get; set; }
        [MaxLength(50)]
        public string? CustomerId { get; set; }

        [MaxLength(50)]
        public string? CustomerProductId { get; set; }

        public int Product { get; set; }

        [MaxLength(50)]
        public string? RefNo { get; set; }

        public decimal Amount { get; set; }

        [MaxLength(50)]
        public string? ProductRef { get; set; }

        [MaxLength(50)]
        public string? MemberNo { get; set; }

        [MaxLength(50)]
        public string? PhoneNumber { get; set; }

        [MaxLength(100)]
        public string? PaymentReference { get; set; }

        public bool Completed { get; set; }

        public int Processed { get; set; }

        public bool Acknowledged { get; set; }

        [MaxLength(50)]
        public string? PartnerId { get; set; }

        [MaxLength(100)]
        public string? PaymentGatewayRef { get; set; }

        public DateTime? CompletedOn { get; set; }

        public DateTime? AcknowledgedOn { get; set; }

        [MaxLength(500)]
        public string? FailedReason { get; set; }

        [MaxLength(50)]
        public string? ErrorCode { get; set; }

        public int PaymentMode { get; set; }

        [MaxLength(500)]
        public string? Narration { get; set; }

        public PaymentStatus? PaymentStatus { get; set; }
    }
}

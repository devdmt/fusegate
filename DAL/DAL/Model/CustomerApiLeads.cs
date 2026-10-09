using System;
using System.ComponentModel.DataAnnotations;

namespace DAL.Model
{
    public class CustomerApiLeads
    {
        public Guid Id { get; set; }

        [MaxLength(50)]
        public string? IdNumber { get; set; }

        [MaxLength(30)]
        public string Phone { get; set; } = null!;

        public string? DateOfBirth { get; set; }

        /// <summary>Stored as integer; aligns with <see cref="Productenum"/> values.</summary>
        public Productenum ProductEnum { get; set; }

        public decimal StartingContribution { get; set; }

        public decimal MonthlyContribution { get; set; }
        public string PartnerCode { get; set; }
        public string? PartnerName { get; set; }
        public int RetirementAge { get; set; }

        [MaxLength(300)]
        public string? Names { get; set; } = null!;

        public DateTime? CreatedOn { get; set; }
    }
}

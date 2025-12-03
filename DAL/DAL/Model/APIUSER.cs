using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models.Interfaces;
namespace DAL.Model
{
    public class APIUSER :  IAuditableEntity
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        
        public string? Configuration { get; set; }
        public string? PartnerId { get; set; }
        [MaxLength(1000)]
        public string ConsumerKey { get; set; }
        [MaxLength(500)]
        public string ConsumerSecret { get; set; }
        [MaxLength(100)]
        public string Salt { get; set; }
        public bool? IsEnabled { get; set; }
        //public bool? IsLockedOut => this.LockoutEnabled && this.LockoutEnd >= DateTimeOffset.UtcNow;
        [MaxLength(50)]
        public string? IpAddress { get; set; }
        [MaxLength(50)]
        public string? HostUrl { get; set; }
        [MaxLength(50)]
        public string? HostPort { get; set; }
        public string? PasswordHash { get; set; }
        public string? SecurityStamp { get; set; }
        public string? ConcurrencyStamp { get; set; }
        public string? PhoneNumber { get; set; }
        [MaxLength(50)]
        public string? CreatedBy { get; set; }
        [MaxLength(50)]
        public string? UpdatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public AccountType accountType { get; set; }
    }
      public enum AccountType
    {
        SandBox = 1,
        Production = 2
    }
    public class PartnerAdminUser : IdentityUser, IAuditableEntity
    {
        public string FullName { get; set; }
        [MaxLength(50)]
        public string? FirstName { get; set; }
        [MaxLength(50)]
        public string? LastName { get; set; }
        [MaxLength(50)]
        public string? Configuration { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsLockedOut => this.LockoutEnabled && this.LockoutEnd >= DateTimeOffset.UtcNow;
        [MaxLength(50)]
        public string? IpAddress { get; set; }
        [MaxLength(50)]
        public string? HostUrl { get; set; }
        [MaxLength(50)]
        public string? HostPort { get; set; }
        [MaxLength(50)]
        public string? CreatedBy { get; set; }
        [MaxLength(50)]
        public string? UpdatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public LoginType loginType { get; set; } = LoginType.Admin;
    }
}

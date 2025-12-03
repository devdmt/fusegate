using DAL.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class Auditable : IAuditableEntity
    {
        public string? CreatedBy { get; set; }
        public string? CreatedName { get; set; }
        public string? UpdatedBy { get; set; }
        public string? UpdatedName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
        public string? DeletedById { get; set; }
        public string? DeletedByUser { get; set; }
    }
}

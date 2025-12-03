

using Microsoft.AspNetCore.Identity;

namespace DAL.Model
{
    public class APIRole: IdentityRole
    {
       
        public string? Description { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}

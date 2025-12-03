

namespace DAL.Model
{
    public class Partners: Auditable
    {
        public int Id { get; set; }
        public string PartnerCode { get; set; }
        public string PartnerName { get; set; }
        public string PartnerDescription { get; set;}
        public string? PartnerType { get; set;}
        public bool Active { get; set; } = true;

    }
   
}

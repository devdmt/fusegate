using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ModelView
{
    public class PartnerDTO
    {
      
        public string PartnerName { get; set; }
        public string PartnerDescription { get; set; }
        public string PartnerType { get; set; }
    }

    /// <summary>
    /// Slim partner lookup result for shared GetPartnerAsync usage.
    /// </summary>
    public class PartnerLookupDTO
    {
        public int Id { get; set; }
        public string PartnerCode { get; set; }
        public string PartnerName { get; set; }
    }
}

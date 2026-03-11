using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class EndpointsSettings
    {
        public string? baseUrl { get; set; }
        public string? SchemeMembers { get; set; }
        public string? Contributions { get; set; }
        public string? Beneficiary { get; set; }
        public string? Guardian { get; set; }
        public string? Withdrawal { get; set; }
        public string? Banks { get; set; }
        public string? Branch { get; set; }
        public string? Validation { get; set; }
        public string? CallbackUrl { get; set; }
    }
}

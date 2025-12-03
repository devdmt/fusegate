using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Policy;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class PartnersProducts
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }        
        
        public virtual Partners Partner { get; set; }
        public string? PartnerId { get; set; }
        public string? Image { get; set; }
        public bool Active { get; set; } =false;
        public DateTime? CreatedOn { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class Products
    {
        public int Id { get; set; }
        public  string productName { get; set; }
        public Productenum productenum { get; set; }
        [MaxLength(10)]
        public string Prefix { get; set; }
        public string Emailtemplate { get; set; }
        
    }

     [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Productenum
    {
        [EnumMember(Value = "creditlife")]
        creditlife=1,
        [EnumMember(Value = "lastexpense")]
        lastexpense=2,
       [EnumMember(Value = "nssf")]
        nssf=2,
       [EnumMember(Value = "prmf")]
        prmf=2,
        [EnumMember(Value = "ipp")]
        ipp=2,
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class MsureRequests
    {
        public string Id { get; set; }
        public virtual Partners Partners { get; set; }
        public int PartnersId { get; set; }
        public virtual PartnersProducts Products { get; set; }
        public int? ProductsId { get; set;}
        public string? customerId { get; set; }
        public benefitOption? benefitOption { get; set; }
        public string? transactionId { get; set; }
        public string? optinTime { get; set; }
        public string? Customername { get; set; }
        public string? Gender { get; set; }
        public double? premium { get; set; }
        public string? status { get; set; }
        public DateTime CreatedOn { get; set; } 
        public bool Processed { get; set; }
       
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum benefitOption
    {
        [EnumMember(Value = "educare")]
        educare = 1,
        [EnumMember(Value = "educareflex")]
        educareflex = 2,
        [EnumMember(Value = "educareplus")]
        educareplus = 3,
    }
}

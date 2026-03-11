using DAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DAL.ModelView
{
    public class MsureDTO
    {
        public string? partnerCode { get; set;}
        public string? productId { get; set; }
        public string? customerName { get; set; }
        public Gender? gender { get; set; }
        public string? customerId { get; set; }
        public benefitOption? benefitOption { get; set; }
        public string? transactionId { get; set; }
        public string? optinTime { get; set; }
        public double? premium { get; set; }
        public string? status { get; set; }

    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Gender
    {
        [EnumMember(Value = "male")]
        male,
        [EnumMember(Value = "female")]
        female,[EnumMember(Value = "other")]
        other

    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum IDType
    {
        [EnumMember(Value = "NationalID")]
        NationalID,
        [EnumMember(Value = "Passport")]
        Passport,
        [EnumMember(Value = "MilitaryId")]
        MilitaryId,
        [EnumMember(Value = "AllienId")]
        AllienId,
        [EnumMember(Value = "driverlicence")] 
        DriverLicense

    }
     

    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
    }
}

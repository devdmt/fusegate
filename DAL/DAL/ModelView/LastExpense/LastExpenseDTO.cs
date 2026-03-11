using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DAL.ModelView.LastExpense
{
    
          public class LastExpenseDTOCalcrequest
    {
        public OptionType optionType { get; set; }
        public long? CustomerId { get; set; }
        public long? productId { get; set; }
        public bool group { get; set; } = false;


    }
     [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OptionType
    {
        [EnumMember(Value = "option1")]
        option1 = 1,
        [EnumMember(Value = "option2")]
        option2 = 2,
        [EnumMember(Value = "option3")]
        option3 = 3,
            [EnumMember(Value = "option4")]
        option4 = 4,
            [EnumMember(Value = "option5")]
        option5 = 5,
            [EnumMember(Value = "option6")]
        option6 = 6

    }

     public class RateResponse
    {
        public bool Success { get; set; } = false;
        public bool Processed { get; set; } = false;
        public string? Errormsg { get; set; } = "";
        public double CoverPremium { get; set; } = 0;
        public double PolicyFee { get; set; } = 0;
        public double CompensationLevy { get; set; } = 0;
        public double TotalPremium { get; set; } = 0;
        public double Discount { get; set; } = 0;

    }
}

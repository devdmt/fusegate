using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DAL.ModelView.Pension
{
    public class PensionerADDDTO
    {
        public string CustomerCode { get; set; }    
        public string Name { get; set; }
        public string Email { get; set; }
        public string DOB { get; set; }
        public EmploymentStatus employmentStatus { get; set; }  
        public string PhoneNumber { get; set; }
        public string IDNumber { get; set; }
        public List<EmployerAddDTO> employer { get; set; }
        public string Signature { get; set; }
     
    }

    public class EmployerAddDTO
    {
        public string Employername { get; set; }
        public string HRemail { get; set; }
        public string Pensionprovider { get; set; }
        public string PensionproviderId { get; set; }
        public string AdditionalInformation { get; set; }
        public bool Currentlyfunding { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EmploymentStatus
    {
        [EnumMember(Value = "EMPLOYED")] EMPLOYED,
        [EnumMember(Value = "BUSINESS")] BUSINESS
    }

    public class CreatePensionResponseDTO
    {
        public string CustomerId { get; set;}
        public string ResponseCode { get; set;}
        public string ResponseMsg { get; set;}
        public string CompletedDateTime { get; set;}
        public string Status { get; set;}   
    }
}

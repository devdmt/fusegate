using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ModelView.Flex
{
    public class ActivateDTO
    {
        public string MemberNo { get; set; }
        public string ProductRef { get; set; }

        public ActivationMode activationMode { get; set; }
    }

    public enum ActivationMode
      {   OTP = 0,
        Signature = 1,
        
    }
    public class ActivateResponseDTO
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }

    public class ContributeDTO
    {
       public string ProductRef { get; set; } 
        public string? MemberNO { get; set; }
        public string? Amount { get; set; } 
        public string? phoneNumber { get; set; }
    }
    public class CompleteActivation
    {
       public string ProductRef { get; set; } 
        public string? OTP { get; set; }
        public string? Signature { get; set; }
    }
}

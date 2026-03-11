using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ModelView.CreditLife
{
     public class QuoteRequestDTO
    {
        public string? productId { set; get; }
        public float sumAssured { set; get; }
        //public string? partnerCode { set; get; }
        public int Loanterm { set; get; }
    }

    public class QuoteResponseDTO
    {
       public bool success {get;set;}=false;
       public bool processed {get;set;}=false;
       public string? errormsg {get;set;}
        public decimal coverPremium { get; set; } = 0;
        public double policyFee { get; set; } = 0;
        public double compensationLevy { get; set; } = 0;
       public decimal totalPremium { get; set; } = 0;
        public double discount { get; set; } = 0;
    }
}

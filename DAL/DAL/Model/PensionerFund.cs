using DAL.Common.Contract;
using DAL.Models.Interfaces;
using DAL.ModelView.Pension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class PensionerFund 
    {

        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }

        public bool Approved { get; set; }
        public DateTime? ApprovedOn { get; set; }
  
        public int? PensionsProviderId { get; set; }
        public ProductTypes ProductTypes { get; set; }  = ProductTypes.IPP;
        public decimal? Employerfunds { get; set; } = 0;
        public decimal? EmployerRegistered { get; set; } = 0;
        public decimal? EmployerUnregistered { get; set; } = 0;
        public decimal? Employeefunds { get; set; } = 0;
        public decimal? Employeeregistered { get; set; } = 0;
        public decimal? EmployeeUnregistered { get; set; } = 0;
        public decimal? Totalfunds { get; set; } = 0;
        public decimal? DeclaredInterest { get; set; } = 0;
        public decimal? EVC_Contribution { get; set; } = 0;
        public decimal? Guaranteed_Interest { get; set; } = 0;
        public decimal? Cumulativefund { get; set; } = 0;
         

    }
}

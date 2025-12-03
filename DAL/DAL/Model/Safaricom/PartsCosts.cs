using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model.Safaricom
{
    public class PartsCosts
    {
        public int Id {  get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double PartCosts { get; set; } = 0;
        public double LaborCost { get; set; } = 0;
        public bool Active {  get; set; }
        public int ReplacementLimit { get; set; } = 0;
        public DateTime CreatedOn {  get; set; } = DateTime.Now;
       
    } 

   
}

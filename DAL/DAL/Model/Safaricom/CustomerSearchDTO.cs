using DAL.ModelView.Safaricom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model.Safaricom
{
    public class CustomerSearchDTO
    {
        public CustomerSearchDTO() {
            Phones = new List<PhoneDTO>();
        }
        public int Id { get; set; }
        public string? CustomerName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? IdNumber { get; set; }
        public string? CustomerAddress { get; set; }
        public string? SecondaryContactName { get; set; }
        public string? SecondaryContact { get; set; }
        public List<PhoneDTO>? Phones { get; set; }
    }
    public class ClaimsDTO
    {
        public long Id { get; set; }
        public string Phonename { get; set; }
        //public int shopType { get; set; }
        public string? CustomerName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ClaimRefNumber { get; set; }
        public string? RequestId { get; set; }
        public string? IDNumber { get; set; }
        public string? IMEINO { get; set; }
        public ClaimStatus claimStatus { get; set; }    
        public ClaimType? ClaimType { get; set; }
        public string? DamagePart { get; set; }
        public int? Partid { get; set; } = 0;
        public double PartCost { get; set; } = 0;
        public double? ReplacementCost { get; set; }
        public string? IncidentDate { get; set; }
        public string? ClaimDate { get; set; }
        public double LabourCost { get; set; } = 0;
        public string? Narration { get; set; }      
        public string? Response { get; set; }    
        public bool? Dispatched { get; set; }   
        public string? DispatchedOn { get; set; }

    }
    public class PhoneDTO
    {
        public string? Id {  get; set; }
        public string? PhoneModel { get; set; }
        public double? PhoneCost {  get; set; } = 0;    
        public string? PhoneName { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public string? IMEINumber { get; set; }
        public string? IMEINumber1 { get; set; }
        public bool Active { get; set; } = true;

    }



}

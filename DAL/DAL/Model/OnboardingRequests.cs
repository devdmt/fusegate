using DAL.ModelView;
using System.ComponentModel.DataAnnotations;

namespace DAL.Model
{
   
    public class Customers
    {
        public string Id { get; set; }
      //  public required string TransactionId { get; set; }
        public required string PartnerId { get; set; }      
        public int ProductId { get; set; }
        public string CustomerName { get; set; }
        public string? DateOfBirth { get; set; }
        public string? IDNumber { get; set; } //Reg No
        public Gender? Gender { get; set; }
        public string? Email { get; set; }  
        public string? PhoneNumber { get; set; }  
        public string? Occupation { get; set; }  
        public string? Residency { get; set; }  
        public string? Nationality { get; set; }  
        public string? RequestDate { get; set; }
        public Boolean? Processed { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? Status { get; set; }

    }
}
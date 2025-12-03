using System.Numerics;

namespace DAL.Model
{
    public class PortalActions:Auditable
    {
        public long Id { get; set; }
        public string? ActionName {  get; set; }
        public string? ActionDescription { get; set;}
        public string? Shopname {  get; set; }
        public string? ShopLocation {  get; set; }
        public string? ShopName { get; set; }
        public string? ShopType { get; set;}
        public string? ClaimType {  get; set; }
        public string? IncidenceDate {  get; set; }
        public string? CustomerName {  get; set; }
        public string? CustomerIdNumber {  get; set; }
        public string? PhoneModel {  get; set; }
        public string? Reference { get; set;}
        public string? RequestId {  get; set; }
        public DateTime CreatedOn { get; set; }
        public string? CreatedBy {  get; set; }


    }
}

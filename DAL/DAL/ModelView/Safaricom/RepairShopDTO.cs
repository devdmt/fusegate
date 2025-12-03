
namespace DAL.ModelView.Safaricom
{
    public class RepairShopDTO
    {
        public int Id { get; set; }
        public string ShopName { get; set; }
        public string Phonenumber { get; set; }
        public string Location { get; set; }
        public int shopType {  get; set; }  
        public string? Email { get; set; } 
        public string? loginType { get; set; }
        public string? ContactName { get; set; }
        public string? ShopOwner { get; set; }
        public string? PartnerCode { get; set; }
        public string? ProductCode { get; set; }
    }

    public record ClaimAuth(string username,string password="");
    public record Partner_Auth(string PartnerKey="", string PartnerSecret="");
    public record AuthResponse(string Token = "", int Expiry =0);
}
    
using DAL.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class RepairShops : Auditable
    {
        public int Id { get; set; }
        public string ShopName { get; set; }
        public string Phonenumber { get; set; }
        public string? ShopLocation { get; set; }
        public string? County { get; set; }
        public string? Subcountry { get; set; }
        public string? Ward { get; set; }
        public string? Town { get; set; }
        public string? Address { get; set; }
        public string? SecondaryPhone { get; set; }
        public string? Email { get; set; }
        public string? ContactName { get; set; }
        public string? ShopOwner { get; set; }
        public bool iSActive { get; set; }
        public LoginType loginType { get; set; } = LoginType.RepairShop;
        public ShopType shopType { get; set; }= ShopType.RepairShop;

    }


    public  enum ShopType
    {
        RepairShop=1,Store=2
    }
    
    public enum LoginType
    {
        RepairShop=1, Store = 2,Admin=3
    }
   }

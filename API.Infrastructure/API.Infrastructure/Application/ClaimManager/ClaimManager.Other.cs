using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace API.Infrastructure.Application.ClaimManager
{
    internal partial class ClaimManager
    {
        public string MaskEmailPhone(string contact, OTPChannelDTO oTPChannelDTO)
        {
            string maskedData = "";
            if (oTPChannelDTO == OTPChannelDTO.email)
            {

                maskedData = string.Format("{0}****{1}", contact[0], contact.Substring(contact.IndexOf('@') - 1));

            }
            else if (oTPChannelDTO == OTPChannelDTO.phone)
            {
                maskedData = contact.Replace(contact.Substring(2,3),"***");


            }
            return maskedData;
        }
    }
    public enum OTPChannelDTO
    {
        email,phone
    }
}

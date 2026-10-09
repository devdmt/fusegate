using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    
public class CreditwaveSMSResponse {
   
    public bool success { get; set; }=false;
    public CreditwaveSMSData? data { get; set; }
    public string message { get; set; }="Failed to send SMS";
}
public class CreditwaveSMSData
{
    public string tracking_id { get; set; }
    public string delivery_status { get; set; }
}
public class CreditwaveTokenResponse
                    {
                        public bool success { get; set; }
                        public CreditwaveTokenData data { get; set; }
                        public string message { get; set; }
                    }

                    public class CreditwaveTokenData
                    {
                        public string access_token { get; set; }
                        public string token_type { get; set; }
                        public int expires_in { get; set; }
                    }

  public class CreditwaveSMS
{
    public string BaseURL { get; set; }
    public string TokenendPoint { get; set; }
    public string SMSendPoint { get; set; }
    public string EmailendPoint { get; set; }
    public string client_Id { get; set; }
    public string client_secret { get; set; }
    public string sender_Id { get; set; }
    public string from_email { get; set; }
    public string Email { get; set; }
    public bool SendSMSWithCredwave { get; set; }
    public bool SendEmailWithCredwave { get; set; } = false!;
}
     public class CreditwaveSMSRequest
        {
            public string message { get; set; }
            public string msisdn { get; set; }
            public bool is_scheduled { get; set; }
            public string sender_id { get; set; }
            public string? scheduled_time { get; set; }
        }
}

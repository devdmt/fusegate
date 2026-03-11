using DAL.Model;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DAL.ModelView;
namespace API.Infrastructure.Application.Pay
{
    internal partial class Pay
    {
           public async Task<MpesaTokenDTO> GetMpesaTokenAsync(string paybillId, string? consumerkey, string? consumersecret)
        {
            var mpesaToken = new MpesaTokenDTO();
            try
            {
                var token = await _akiba.mpesaToken.Where(p => p.paybillid == paybillId).FirstOrDefaultAsync();
                if (token != null)
                {
                    double diff = (DateTime.Now - token.createdon).TotalSeconds;
                    if (Convert.ToInt32(token.Expires_in) > diff + 10)
                    {
                        mpesaToken.access_token = token.Access_token;
                        mpesaToken.expires_in = token.Expires_in.ToString();
                        return mpesaToken;
                    }
                }

            }
            catch (Exception ex)
            {
                _setting.LogRequests(ex.Message, "MpesaTransactionManager -GetMpesaTokenAsync",Interface.RequestType.Error);
            }
            //check token


            mpesaToken = await AkibaQueryToken(paybillId, consumerkey, consumersecret);


            return mpesaToken;
        }
        public async Task<MpesaTokenDTO> AkibaQueryToken(string paybillId, string? consumerkey, string? consumersecret)
        {
            var token = new MpesaTokenDTO();

            var mpesaToken = new MpesaTokenDTO();
            string key = "", secret = "";
            if (string.IsNullOrEmpty(consumerkey) == true && string.IsNullOrEmpty(consumersecret))
            {
                var paybillsettings = await _akiba.mpesaSettings.Where(a => a.PaybillId == paybillId).FirstOrDefaultAsync();

                key = _encservice.DecryptText(paybillsettings.ConsumerKey, paybillsettings.SaltKey);
                secret = _encservice.DecryptText(paybillsettings.ConsumerSecret, paybillsettings.SaltKey);
            }
            else
            {
                key = consumerkey; secret = consumersecret;
            }

            if (key != "" && secret != "")
            {
                string basiccode = key + ":" + secret;
                string authcode = Convert.ToBase64String(Encoding.UTF8.GetBytes(basiccode));
                var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                //temporaliry auth code comment after testing 

                //authcode = "cFJZcjZ6anEwaThMMXp6d1FETUxwWkIzeVBDa2hNc2M6UmYyMkJmWm9nMHFRR2xWOQ==";


                /// comment the above
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", "Basic " + authcode + "");
                var url = (string)await _akiba.Connection.ExecuteScalarAsync("SELECT Value from " +
                    " endpointssettings where endPointType =" +
                    (int)EndPointType.Mpesa_Token + ";");
                if (url == null)
                {

                }

                try
                {
                    var results = await client.GetAsync(url);
                    var responseBody = await results.Content.ReadAsStringAsync();
                    mpesaToken = JsonSerializer.Deserialize<MpesaTokenDTO>(responseBody);

                    MpesaToken settings = new MpesaToken();
                    settings.Expires_in = Convert.ToInt16(mpesaToken.expires_in);
                    //settings.Id = Guid.NewGuid().ToString();
                    settings.createdon = DateTime.Now;
                    settings.Access_token = mpesaToken.access_token;
                    settings.paybillid = paybillId;
                    _akiba.mpesaToken.Add(settings);
                    await _akiba.SaveChangesAsync();

                }
                catch (Exception ex)
                {
                     _setting.LogRequests(ex.Message, "MpesaTransactionManager -QueryToken",Interface.RequestType.Error);
                    //throw;
                }


            }
            return mpesaToken;
        }
    }
}

using API.Infrastructure.Interface;
using DAL;
using DAL.Model;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
namespace API.Infrastructure.Application.Communication
{
    public class Communication:IComunication
    {

        public readonly Isettings _settings;
        public readonly ApplicationDbContext _db;
        private readonly ILogger<Communication> _logger;
        private readonly CreditwaveSMS _creditwave;
        public Communication(Isettings settings, ApplicationDbContext db, ILogger<Communication> logger,
            IOptions<CreditwaveSMS> creditwave)
        {
            _settings = settings;
            _db = db;
            _logger = logger;
            _creditwave = creditwave.Value;
        }
        public async Task ProcessSMS()
        {
            try
            {
                
                // Get OTPs that haven't been sent yet

                //var pendingOTPs = await _db.Connection.QueryAsync<OTP>("SELECT * FROM OTPs " +
                //    "WHERE Isnull(ISsent,'0')='0' " +
                //    " AND (notificationType = 1 or notificationType = 2) and SendTrial<5");

                var pendingOTPs = await _db.Connection.QueryAsync<OTP>("exec getOTPs");
                //var pendingOTPs = await _db.Connection.QueryAsync<OTP>("getOTPs 1");
               //  _communication.LogToFile("Processing OTP count: " +pendingOTPs.Count().ToString(), "SendSMSOTPs", LogType.Info);
                //_communication.LogToFile("test", "test", LogType.Info);
                foreach (var otp in pendingOTPs)
                {
                  
                    try
                    {
                       // Get customers phone number
                      // var customers = new Customers(); 

                       
                        
                         

                        if (!string.IsNullOrEmpty(otp.Phonenumber))
                        {
                            string phonenumb= otp.Phonenumber.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
                           // Create SMS message
                            
                    string countrycode = "254";// customers?.CountryCode?.Replace("+", "") ?? "254";
                                
                            ///Send SMS
                             //var smsResponse = await _communication.SendSMS(smsParams);
var smsRequest = new CreditwaveSMSRequest
{
    message = otp.Message,
    msisdn = countrycode + phonenumb.Substring(phonenumb.Length-9),
    is_scheduled = false,
    sender_id = _creditwave.sender_Id,
};
                            
                                var smsResponse = await SendCreditwaveSMSAsync(smsRequest);
                                otp.Response = smsResponse.message;
                            otp.SentError = smsResponse.data?.delivery_status == "MessageWaiting" ? null : smsResponse.message;

                            

                            // Update OTP record
                            otp.ISSMSSent = true;
                            otp.ISsent = true;
                            otp.ErrorMessage= "for testing";
                            otp.SendTrial += 1;
                            otp.SentAt = DateTime.Now;
                            _db.Update(otp);
                            await _db.SaveChangesAsync();

                            _logger.LogInformation("OTP to {phone} responded with {error}",
                                otp.Phonenumber, otp.SentError);
                        }
                        else
                        {
                           // Mark as sent with error if no phone number
                             otp.ISsent = false;
                             otp.ISSMSSent = false;
                            otp.SentAt = DateTime.Now;
                            otp.ErrorMessage = "No phone number found for customers";
                           
                             _db.Update(otp);
                            await _db.SaveChangesAsync();

                            //_logger.LogWarning("No phone number found for customers {customersId}", otp.customersId);
                        }
                    }
                    catch (Exception ex)        
                    {
                      
                        _settings.LogRequests(ex.Message, "SendSMSOTPs", RequestType.Error);
                        _settings.LogRequests(ex?.StackTrace ?? "", "SendSMSOTPs", RequestType.Error);
                        // Update OTP record with error
                        otp.ISsent = false;
                             otp.ISSMSSent = false;
                            otp.SentAt = DateTime.Now;
                            otp.ErrorMessage = ex.Message;
                           

                        await _db.SaveChangesAsync();

                    }
                }
            }
            catch (Exception ex)
            {
                _settings.LogRequests(ex.Message, "SendSMSOTPs", RequestType.Error);
                        // Update OTP record with error    
                _logger.LogError(ex, "Error processing pending OTPs");
               // throw;
            }
        }

         public async Task<CreditwaveSMSResponse> SendCreditwaveSMSAsync(CreditwaveSMSRequest request)
        {
            var url = _creditwave.BaseURL + "/" + _creditwave.SMSendPoint; // Replace with actual endpoint if needed
            var response = new CreditwaveSMSResponse();
            var payload = new
            {
                message = request.message,
                msisdn = request.msisdn,
                is_scheduled = request.is_scheduled,
                sender_id = request.sender_id,
                scheduled_time = request.scheduled_time
            };

            using (var httpClient = new System.Net.Http.HttpClient())
            {
                try
                {
                    // Get token and add to Authorization header
                    var token = await GetCreditwaveTokenAsync();
                    if (!string.IsNullOrEmpty(token))
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    }

                    var jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
                    
                    var content = new System.Net.Http.StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");

                    var result = await httpClient.PostAsync(url, content);
                    var responseContent = await result.Content.ReadAsStringAsync();
                   
                     _settings.LogRequests(responseContent,  "SendCreditwaveSMSAsync",  RequestType.Info);
                    response = Newtonsoft.Json.JsonConvert.DeserializeObject<CreditwaveSMSResponse>(responseContent) ?? new CreditwaveSMSResponse();
                }
                catch (Exception ex)
                {
                    _settings.LogRequests(ex.Message,  "SendCreditwaveSMSAsync",  RequestType.Error);
                    Console.WriteLine("Error sending test SMS: " + ex.Message);
                    return response;
                }
            }
            return response;
        }

                public async Task<string?> GetCreditwaveTokenAsync()
        {
            var url = _creditwave.BaseURL + "/" + _creditwave.TokenendPoint;
            var payload = new
            {
                client_id = _creditwave.client_Id,
                client_secret = _creditwave.client_secret
            };

            using (var httpClient = new System.Net.Http.HttpClient())
            {
                try
                {
                
                    var jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
                    var content = new System.Net.Http.StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");
                    //var content = new System.Net.Http.StringContent(jsonPayload, System.Text.Encoding.UTF8, "multipart/form-data");

                    var result = await httpClient.PostAsync(url, content);
                    var responseContent = await result.Content.ReadAsStringAsync();
                    // Define a class to map the token response
                    

                    // Assuming the response is like: { "access_token": "..." }
                    var token = Newtonsoft.Json.JsonConvert.DeserializeObject<CreditwaveTokenResponse>(responseContent);
                    return token?.data?.access_token;
                }
                catch (Exception ex)
                {
                    _settings.LogRequests(ex.Message, "GetCreditwaveToken",  RequestType.Error);
           
                    Console.WriteLine("Error getting Creditwave token: " + ex.Message);
                    return null;
                }
            }
        }
    }
}

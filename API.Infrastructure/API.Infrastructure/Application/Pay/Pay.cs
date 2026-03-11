using API.Infrastructure.Interface;
using Azure;
using DAL;
using DAL.Model;
using DAL.ModelView;
using DAL.ModelView.Pension;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace API.Infrastructure.Application.Pay
{
    internal partial class Pay:IPay
    {

        readonly ApplicationDbContext _db;
        readonly IEncryptionService _encservice;
        readonly Isettings _setting;
         readonly EndpointsSettings _endpoints;
        readonly AkibappDbContext _akiba;
        public Pay(ApplicationDbContext db, IEncryptionService encservice, Isettings setting, AkibappDbContext akibapp)
        {
            _db = db;
            _encservice = encservice;
            _setting = setting;
            _akiba = akibapp;
        }
        public async Task<ResponseDTO> ProcessSTK(STKContributionDTO request)
        {
            var response = new ResponseDTO();  
            try
            {
                string callbackurl = (string)await _akiba.Connection.ExecuteScalarAsync("select Value from endpointsSettings where endPointType =" +
             (int)EndPointType.Mpesa_Pension_STK_CallbackUrl + "");
                string requesturl = (string)await _akiba.Connection.ExecuteScalarAsync("select Value from endpointsSettings where endPointType =" +
                   (int)EndPointType.Mpesa_STK_RequestUrl + "");
                string ts = DateTime.Now.ToString("yyyyMMddhhmmss");

                string shortcode = "", Passkey = "", trntype = "CustomerPayBillOnline", key = "", secret = "";

                //get the paybill
                var paybill = await _akiba.mpesaSettings.Where(a => a.Active == true && a.Deleted != true 
                && a.paybillType == PaybillType.Paybill).FirstOrDefaultAsync();



                if (paybill != null)
                {
                    shortcode = _encservice.DecryptText(paybill.PaybillId, paybill.SaltKey);
                    Passkey = _encservice.DecryptText(paybill.PassKey, paybill.SaltKey);

                    if (paybill.paybillType == PaybillType.Paybill)
                    {
                        trntype = "CustomerPayBillOnline";
                    }
                    else if (paybill.paybillType == PaybillType.Tillnumber)
                    {
                        trntype = "CustomerBuyGoodsOnline";
                    }

                    key = _encservice.DecryptText(paybill.ConsumerKey, paybill.SaltKey);
                    secret = _encservice.DecryptText(paybill.ConsumerSecret, paybill.SaltKey);




                    var token = await GetMpesaTokenAsync(shortcode, key, secret);
                    if (token.access_token == null)
                    {
                        for(int i = 0; i < 3; i++)
                    {
                           token = await GetMpesaTokenAsync(shortcode, key, secret);
                            if(token.access_token != null)
                            {
                                continue;
                            }
                    }

                    }
                    
                    string pwd = Convert.ToBase64String(Encoding.UTF8.GetBytes(shortcode + Passkey + ts));

                    var stkpush = new STkPushRequestDTO()
                    {
                        Amount =Math.Round(request.Amount,0).ToString(),
                        AccountReference = request.MemberNo.ToString(),
                        PhoneNumber = "254" + request.Phonenumber.Substring(request.Phonenumber.Length - 9),
                        TransactionDesc = request.TrnCode,
                        PartyA = "254" + request.Phonenumber.Substring(request.Phonenumber.Length - 9),
                        Timestamp = ts,
                        Password = pwd,
                        PartyB = shortcode,
                        BusinessShortCode = shortcode,
                        TransactionType = trntype,
                        CallBackURL = callbackurl ?? _endpoints.CallbackUrl
                    };
                    var requestresponse = await RequestMpesaExpress(stkpush, token.access_token, requesturl);

                    string merchantId = requestresponse.Success ? requestresponse.MerchantID : "";
                    string CheckoutRequestID = requestresponse.Success ? requestresponse.CheckoutRequestID : requestresponse.RequestId.ToString();
                    string CustomerMessage = requestresponse.Success ? requestresponse.CustomerMessage : "";
                    string stkquery = "";
                    string updatesale = "";

                    stkquery = "INSERT INTO [dbo].[sTKPushMpesaTransactions]([CustomerId],[RecipientPhoneNumber],[AccountReference]," +
                        "[MerchantRequestID],[CheckoutRequestID],ResponseCode," +
                    "[CreatedOn],[Processed],[Finalized],ContributionTrnId,CustomerType,ProcessBatch) VALUES('" + request.PensionerId + "','" + request.Phonenumber + "'," +
                        "'" + request.MemberNo + "','" + merchantId + "','" + CheckoutRequestID + "','" + requestresponse.ResponseCode + "',getdate(),'0','0','" +
                        request.TrnCode + "','"+ (int) request.AccountType  +"','"+ request.ProcessBatch +"'); select @@IDENTITY as id";

                    //await _db.Connection.ExecuteAsync(stkquery);

                    var id = await _akiba.Connection.ExecuteScalarAsync<decimal>(stkquery);
                    response.ErrorMsg = requestresponse.Success ?
                        "An Mpesa STK push has been sent to " + request.Phonenumber + " please ask the customer to put in the pin" : requestresponse.ResponseDescription;
                    response.Success = requestresponse.Success;
                    
                    response.ProductRef = id.ToString();
                    //response.StatusCode = id.ToString();
                }
            }
            catch (Exception ex) 
            {
            
            }
            return response;
        }
    }
}

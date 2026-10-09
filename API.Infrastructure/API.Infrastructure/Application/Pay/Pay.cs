using API.Infrastructure.Interface;

using DAL;
using DAL.Model;
using DAL.ModelView;
using DAL.ModelView.Pension;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
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
        public Pay(ApplicationDbContext db, IEncryptionService encservice, Isettings setting, AkibappDbContext akibapp,IOptions<EndpointsSettings> options)
        {
            _db = db;
            _encservice = encservice;
            _setting = setting;
            _akiba = akibapp;
            _endpoints= options.Value;
        }

        public async Task<ResponseDTO> ProcessPensionSTKResult(string? body, MpesaSTKResult sTKResult)
        {
            var response = new ResponseDTO();
            try
            {
                string updatequery = "";
                int status = 0;
                string query1 = "SELECT isnull(Id,0) as trnId,ContributionTrnId as productId,ProcessBatch FROM sTKPushMpesaTransactions where MerchantRequestID= '" + sTKResult.Body.stkCallback.MerchantRequestID
                 + "' and CheckoutRequestID ='" + sTKResult.Body.stkCallback.CheckoutRequestID + "' and Processed ='0' ";

                _setting.LogRequests(query1,"ProcessPensionSTKResult",RequestType.Info);
                ////var trn = await _context.sKPushMpesaTransactions.Where(a => a.MerchantRequestID ==
                ////sTKResult.Body.stkCallback.MerchantRequestID && a.CheckoutRequestID == sTKResult.Body.stkCallback.CheckoutRequestID && a.Processed == false).FirstOrDefaultAsync();
                var trnId = await _akiba.Connection.QueryFirstOrDefaultAsync<STKRst>(query1);
                if (trnId != null && (int)trnId.trnId > 0)
                {
                    if (sTKResult.Body.stkCallback.ResultCode.ToString() == "0")
                    {
                        status = 1; 
                    }
                    string stkupdatequery = "";
                    if (sTKResult.Body.stkCallback.ResultCode != 0)
                    {
                        stkupdatequery = "UPDATE sTKPushMpesaTransactions SET Finalized='1',ResponseCode='" + sTKResult.Body.stkCallback.ResultCode + "',Processed='0', Body ='" + body + "'" +
                        ",ResultCode='" + sTKResult.Body.stkCallback.ResultCode.ToString() + "'," +
                       "ResultDesc='" + sTKResult.Body.stkCallback.ResultDesc.ToString() + "',ResultOn=GETDATE() WHERE Id = '" + trnId.trnId + "';";
                         _akiba.Connection.Execute(stkupdatequery);
                      
                            string updateproducts = "update Contributions set Approved='0',PaymentAcknowledged='0',PaymentStatus=" + (int)PaymentStatus.Rejected + "" +
                            ",RejectedReason='" + sTKResult.Body.stkCallback.ResultDesc.ToString() + "' where id='" + trnId.productId + "'";
                        _akiba.Connection.Execute(updateproducts);

                          var callback= await _db.callBackResponse.Where(a => a.CorrelationId == trnId.trnId.ToString()).FirstOrDefaultAsync();
                          
                         var callbackresponse = new ContributionCallbackResponseDTO {
                                ErrorMsg = sTKResult.Body.stkCallback.ResultDesc??"Your transaction failed,please try again",
                                Success = false,
                                ProductRef= callback.CorrelationId,
                                  RequestId = callback.RequestId,
                                   TransactionId =""

                            };
                        
                          string updateCallback = "update CallBackResponse set ProcessResponse='1'," +
                                " Response='"+ JsonSerializer.Serialize(callbackresponse) +"'" +
                                " where id='"+ callback.Id +"'";
                            _db.Connection.Execute(updateCallback);

                    }
                    else
                    {
                       
                       
                            var contribution= await _akiba.Connection.QueryFirstOrDefaultAsync<PensionContributions>("SELECT" +
                                " [Id],[CustomerId],[GroupId],[Year],[Month],[MonthName]," +
                                "ISNULL([Ee_Contribution],0) as [Ee_Contribution],ISNULL([Er_Contribution],0) " +
                                "as Er_Contribution,ISNULL([Ee_Registered],0) as [Ee_Registered]," +
                                "ISNULL([Er_Registered],0) as Er_Registered,ISNULL([Ee_UnRegistered],0) " +
                                "as Ee_UnRegistered,ISNULL([Er_UnRegistered],0) as Er_UnRegistered," +
                                "ISNULL([Ee_CorpTax],0) as Ee_CorpTax,ISNULL([Total_Contribution],0) as" +
                                " Total_Contribution,[Naration],[Reference],[Created],[CreatedBy]," +
                                "[LastModified],[LastModifiedBy],[DeletedBy],[CreatedFromIP]," +
                                "[CreatedFromBrowser],ISNULL([EE_Opening_Balance_Registered],0) " +
                                "as EE_Opening_Balance_Registered,ISNULL([EE_Opening_Balance_UnRegistered],0) " +
                                "as EE_Opening_Balance_UnRegistered,[PensionFundId],[ApprovedBy]" +
                                ",[ApprovedDate],[Bank],[BankReference],[Branch],[PaymentAcknowledged]," +
                                "[PaymentStatus],[RejectedBy],[RejectedDate],[RejectedReason]," +
                                "ISNULL([Er_CorpTax],0) as Er_CorpTax,[FullName],[Idnumber]," +
                                "ISNULL([EVC_Contribution],0) as EVC_Contribution," +
                                "[BatchReference],[Approved],ISNULL(TaxFeeDeferred,'0')" +
                                " as TaxFeeDeferred,[schemeCode],[PaymentMode],[AccountType]," +
                                "[ContributionType] FROM [dbo].[Contributions] where" +
                                " Convert(nvarchar(50),Id)='"+ trnId.productId +"' and" +
                                " isnull(PaymentAcknowledged,'0') ='0' and isnull(Approved,'0')='0'");
                            if (contribution != null)
                            {
                                stkupdatequery = "UPDATE sTKPushMpesaTransactions SET Finalized='1',Processed='1',Amount='" + sTKResult.Body.stkCallback.CallbackMetadata.Item[0].Value + "', Body ='" + body + "',MpesaReceiptNumber='" + sTKResult.Body.stkCallback.CallbackMetadata.Item[1].Value.ToString() + "'" +
                               ",TransactionDate='" + sTKResult.Body.stkCallback.CallbackMetadata.Item[3].Value.ToString() + "',ResultCode='" + sTKResult.Body.stkCallback.ResultCode.ToString() + "'," +
                               "ResultDesc='" + sTKResult.Body.stkCallback.ResultDesc.ToString() + "',ResultOn=GETDATE() WHERE Id = '" + trnId.trnId + "';";

                                 _akiba.Connection.Execute(stkupdatequery);
                                string updateproducts = "update Contributions set Approved='1',PaymentAcknowledged='1',Reference='" + sTKResult.Body.stkCallback.CallbackMetadata.Item[1].Value.ToString() + "' " +
                                    ",PaymentStatus=" + (int)PaymentStatus.Approved + " where id='" + contribution.Id + "'";
                                _akiba.Connection.Execute(updateproducts);

                                string updatePensionerFundquery = "update [dbo].[PensionerFund] set [Employerfunds] =isnull([Employerfunds],0)+ " + Math.Round(Convert.ToDecimal(contribution.Er_Contribution), 2) + "," +
                    "EVC_Contribution=isnull(EVC_Contribution,0)+  " + Math.Round(Convert.ToDecimal(contribution.EVC_Contribution), 2) +",EmployerRegistered =isnull(EmployerRegistered,0)+" + Math.Round(Convert.ToDecimal(contribution.Er_Registered), 2) + "," +
                    " [EmployerUnregistered] =isnull([EmployerUnregistered],0)+" + Math.Round(Convert.ToDecimal(contribution.Ee_UnRegistered), 2) + ",Employeefunds =isnull(Employeefunds ,0)+ " + Math.Round(Convert.ToDecimal(contribution.Ee_Contribution), 2)+"," +
                    "EmployeeRegistered=isnull(EmployeeRegistered ,0)+ " + Math.Round(Convert.ToDecimal(contribution.Ee_Registered), 2) + ",[EmployeeUnregistered] =isnull([EmployeeUnregistered],0)+" + Math.Round(Convert.ToDecimal(contribution.Ee_UnRegistered), 2) + ", " +
                    "[Totalfunds] =isnull([Totalfunds] ,0)+ " + contribution.Total_Contribution + " where  [Id] ='" + contribution.PensionFundId + "' ";
               

                                await _akiba.Connection.ExecuteAsync(updatePensionerFundquery);
                            var callback= await _db.callBackResponse.Where(a => a.CorrelationId == trnId.trnId.ToString()).FirstOrDefaultAsync();
                            var callbackresponse = new ContributionCallbackResponseDTO {
                                ErrorMsg = "Your contribution of Ksh " + contribution.Total_Contribution + " has been received and approved. Thank you for your continued support.",
                                Success = true,
                                 ProductRef= callback.CorrelationId,
                                  RequestId = callback.RequestId,
                                   TransactionId = sTKResult.Body.stkCallback.CallbackMetadata.Item[1].Value.ToString()

                            };


                            string updateCallback = "update CallBackResponse set ProcessResponse='1'," +
                                " Response='"+ JsonSerializer.Serialize(callbackresponse) +"'" +
                                " where id='"+ callback.Id +"'";
                            _db.Connection.Execute(updateCallback);
                            // SendSMS(contribution.CustomerId.ToString());


                        }
                    }



                   


                }
            }
            catch (Exception ex)
            {
                _setting.LogRequests(ex.Message,"ProcessPensionSTKResult",RequestType.Error);
                _setting.LogRequests(ex.StackTrace,"ProcessPensionSTKResult",RequestType.Error);
            }
            return response;
        }
        public async Task<ResponseDTO> ProcessInsureSTKResult(string? body, MpesaSTKResult sTKResult)
        {
            var response = new ResponseDTO();
            try
            {
                string updatequery = "";
                int status = 0;
                string query1 = "SELECT isnull(Id,0) as trnId,ContributionTrnId as productId,ProcessBatch FROM sTKPushMpesaTransactions where MerchantRequestID= '" + sTKResult.Body.stkCallback.MerchantRequestID
                 + "' and CheckoutRequestID ='" + sTKResult.Body.stkCallback.CheckoutRequestID + "' and Processed ='0' ";

                _setting.LogRequests(query1,"ProcessPensionSTKResult",RequestType.Info);
                ////var trn = await _context.sKPushMpesaTransactions.Where(a => a.MerchantRequestID ==
                ////sTKResult.Body.stkCallback.MerchantRequestID && a.CheckoutRequestID == sTKResult.Body.stkCallback.CheckoutRequestID && a.Processed == false).FirstOrDefaultAsync();
                var trnId = await _db.Connection.QueryFirstOrDefaultAsync<STKRst>(query1);
                if (trnId != null && (int)trnId.trnId > 0)
                {
                    if (sTKResult.Body.stkCallback.ResultCode.ToString() == "0")
                    {
                        status = 1; 
                    }
                    string stkupdatequery = "";
                    if (sTKResult.Body.stkCallback.ResultCode != 0)
                    {
                        stkupdatequery = "UPDATE sTKPushMpesaTransactions SET Finalized='1',ResponseCode='" + sTKResult.Body.stkCallback.ResultCode + "',Processed='0', Body ='" + body + "'" +
                        ",ResultCode='" + sTKResult.Body.stkCallback.ResultCode.ToString() + "'," +
                       "ResultDesc='" + sTKResult.Body.stkCallback.ResultDesc.ToString() + "',ResultOn=GETDATE() WHERE Id = '" + trnId.trnId + "';";
                         _db.Connection.Execute(stkupdatequery);
                      
                            string updateproducts = "update Contribution set Acknowledged='0',PaymentStatus=" + (int)PaymentStatus.Rejected + "" +
                            ",FailedReason='" + sTKResult.Body.stkCallback.ResultDesc.ToString() + "' where id='" + trnId.productId + "'";
                        _db.Connection.Execute(updateproducts);

                          var callback= await _db.callBackResponse.Where(a => a.CorrelationId == trnId.trnId.ToString()).FirstOrDefaultAsync();
                          
                         var callbackresponse = new ContributionCallbackResponseDTO {
                                ErrorMsg = sTKResult.Body.stkCallback.ResultDesc??"Your transaction failed,please try again",
                                Success = false,
                                ProductRef= callback.CorrelationId,
                                  RequestId = callback.RequestId,
                                   TransactionId =""

                            };
                        
                          string updateCallback = "update CallBackResponse set ProcessResponse='1'," +
                                " Response='"+ JsonSerializer.Serialize(callbackresponse) +"'" +
                                " where id='"+ callback.Id +"'";
                            _db.Connection.Execute(updateCallback);

                    }
                    else
                    {
                       
                       
                            var contribution= await _db.Connection.QueryFirstOrDefaultAsync<Contribution>("select [Id],[CustomerId]" +
                                ",[CustomerProductId],[Product],[RefNo],[Amount],[ProductRef],[MemberNo],[PhoneNumber],[PaymentReference],[Completed]" +
                                ",[Processed],[Acknowledged],[PartnerId],[PaymentGatewayRef],[CompletedOn],[AcknowledgedOn],[FailedReason],[ErrorCode]" +
                                ",[PaymentMode],[Narration],[PaymentStatus]" +
                                "FROM [dbo].[Contribution] where" +
                                " Convert(nvarchar(50),Id)='"+ trnId.productId +"'");
                            if (contribution != null)
                            {
                                stkupdatequery = "UPDATE sTKPushMpesaTransactions SET Finalized='1',Processed='1',Amount='" + sTKResult.Body.stkCallback.CallbackMetadata.Item[0].Value + "', Body ='" + body + "',MpesaReceiptNumber='" + sTKResult.Body.stkCallback.CallbackMetadata.Item[1].Value.ToString() + "'" +
                               ",TransactionDate='" + sTKResult.Body.stkCallback.CallbackMetadata.Item[3].Value.ToString() + "',ResultCode='" + sTKResult.Body.stkCallback.ResultCode.ToString() + "'," +
                               "ResultDesc='" + sTKResult.Body.stkCallback.ResultDesc.ToString() + "',ResultOn=GETDATE() WHERE Id = '" + trnId.trnId + "';";

                                 _db.Connection.Execute(stkupdatequery);
                                string updateproducts = "update Contribution set Processed='1',Acknowledged='1',Completed='1',CompletedOn=GETDATE()" +
                                ",PaymentGatewayRef='" + sTKResult.Body.stkCallback.CallbackMetadata.Item[1].Value.ToString() + "' " +
                                    ",PaymentStatus=" + (int)PaymentStatus.Approved + " where id='" + contribution.Id + "'";
                                _db.Connection.Execute(updateproducts);

                            var callback= await _db.callBackResponse.Where(a => a.CorrelationId == trnId.trnId.ToString()).FirstOrDefaultAsync();
                            var callbackresponse = new ContributionCallbackResponseDTO {
                                ErrorMsg = "Your contribution of Ksh " + contribution.Amount + " has been received and approved. Thank you for your continued support.",
                                Success = true,
                                 ProductRef= callback.CorrelationId,
                                  RequestId = callback.RequestId,
                                   TransactionId = sTKResult.Body.stkCallback.CallbackMetadata.Item[1].Value.ToString()

                            };


                            string updateCallback = "update CallBackResponse set ProcessResponse='1'," +
                                " Response='"+ JsonSerializer.Serialize(callbackresponse) +"'" +
                                " where id='"+ callback.Id +"'";
                            _db.Connection.Execute(updateCallback);
                            // SendSMS(contribution.CustomerId.ToString());


                        }
                    }



                   


                }
            }
            catch (Exception ex)
            {
                _setting.LogRequests(ex.Message,"ProcessPensionSTKResult",RequestType.Error);
                _setting.LogRequests(ex.StackTrace,"ProcessPensionSTKResult",RequestType.Error);
            }
            return response;
        }

        //   public void SendSMS(string customerId)
        //{
        //    string phonenumber = _db.Connection.ExecuteScalar<string>("select PhoneNumber from Customers where Id=@Id",
        //        new { Id = customerId });
        //    if (!string.IsNullOrEmpty(phonenumber))
        //    {
        //        string message = "Your contributions have been approved. Thank you for your continued support.";
        //        if (phonenumber.StartsWith("+"))
        //        {
        //            phonenumber = phonenumber.Remove(0, 1);

        //        }
        //        if (phonenumber.StartsWith("0"))
        //        {
        //            phonenumber = "254" + phonenumber.Substring(phonenumber.Length - 9);
        //        }

        //        var sendSMS = new SendSMSParams
        //        {
        //            Phonenumber = phonenumber,
        //            Message = message
        //        };
        //        _isend.SendSMS(sendSMS);
        //    }
        //    else
        //    {
        //        throw new Exception("Phone number not found for the customer.");
        //    }
        //}

        public async Task<ResponseDTO> ProcessSTK_Insure(STKContributionDTO request,EndPointType endPointType= EndPointType.Mpesa_Pension_STK_CallbackUrl)
        {
            var response = new ResponseDTO();  
            try
            {
                string callbackurl = (string)await _db.Connection.ExecuteScalarAsync("select Value from endpointsSettings where endPointType =" +
             (int)endPointType + "");
                string requesturl = (string)await _db.Connection.ExecuteScalarAsync("select Value from endpointsSettings where endPointType =" +
                   (int)EndPointType.Mpesa_STK_RequestUrl + "");
                string ts = DateTime.Now.ToString("yyyyMMddhhmmss");

                string shortcode = "", Passkey = "", trntype = "CustomerPayBillOnline", key = "", secret = "";

                //get the paybill
                var paybill = await _db.mpesaSettings.Where(a => a.Active == true && a.Deleted != true 
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

                      Console.WriteLine("encrypted paybill");


                    var token = await GetMpesaTokenAsync(shortcode, key, secret);
                    if (token.access_token == null)
                    {
                        for(int i = 0; i < 3; i++)
                    {
                           token = await GetMpesaTokenAsync(shortcode, key, secret);
                            if(token.access_token != null)
                            {
                                i = 5;
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
                      Console.WriteLine("sTKPushMpesaTransactions");
                    stkquery = "INSERT INTO [dbo].[sTKPushMpesaTransactions]([CustomerId],[RecipientPhoneNumber],[AccountReference]," +
                        "[MerchantRequestID],[CheckoutRequestID],ResponseCode," +
                    "[CreatedOn],[Processed],[Finalized],ContributionTrnId,CustomerType,ProcessBatch) VALUES('" + request.PensionerId + "','" + request.Phonenumber + "'," +
                        "'" + request.MemberNo + "','" + merchantId + "','" + CheckoutRequestID + "','" + requestresponse.ResponseCode + "',getdate(),'0','0','" +
                        request.TrnCode + "','"+ (int) request.AccountType  +"','"+ request.ProcessBatch +"'); select @@IDENTITY as id";

                    //await _db.Connection.ExecuteAsync(stkquery);

                    var id = await _db.Connection.ExecuteScalarAsync<decimal>(stkquery);
                    response.ErrorMsg = requestresponse.Success ?
                        "An Mpesa STK push has been sent to " + request.Phonenumber + " please ask the customer to put in the pin" : requestresponse.ResponseDescription;
                    response.Success = requestresponse.Success;
                    
                    response.ProductRef = id.ToString();
                    //response.StatusCode = id.ToString();
                }
            }
            catch (Exception ex) 
            {
            _setting.LogRequests(ex.Message,"ProcessSTK_Insure",RequestType.Error);
            _setting.LogRequests(ex.StackTrace,"ProcessSTK_Insure",RequestType.Error);
            }
            return response;
        }
        public async Task<ResponseDTO> ProcessSTK(STKContributionDTO request,EndPointType endPointType= EndPointType.Mpesa_Pension_STK_CallbackUrl)
        {
            var response = new ResponseDTO();  
            try
            {
                string callbackurl = (string)await _db.Connection.ExecuteScalarAsync("select Value from endpointsSettings where endPointType =" +
             (int)endPointType + "");
                string requesturl = (string)await _db.Connection.ExecuteScalarAsync("select Value from endpointsSettings where endPointType =" +
                   (int)EndPointType.Mpesa_STK_RequestUrl + "");
                string ts = DateTime.Now.ToString("yyyyMMddhhmmss");

                string shortcode = "", Passkey = "", trntype = "CustomerPayBillOnline", key = "", secret = "";

                //get the paybill
                var paybill = await _db.mpesaSettings.Where(a => a.Active == true && a.Deleted != true 
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
                                i = 5;
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
            _setting.LogRequests(ex.Message,"ProcessSTK",RequestType.Error);
            }
            return response;
        }

        public async Task<ResponseDTO> ProcessFlexiSTKResult(string? body, MpesaSTKResult sTKResult)
        {
            var response = new ResponseDTO();
            try
            {
                var callback = sTKResult?.Body?.stkCallback;
                if (callback == null
                    || string.IsNullOrWhiteSpace(callback.MerchantRequestID)
                    || string.IsNullOrWhiteSpace(callback.CheckoutRequestID))
                {
                    response.ErrorMsg = "Invalid callback payload.";
                    return response;
                }

                const string lookup = @"SELECT isnull(Id,0) as trnId, ContributionTrnId as productId, CustomerType, ProcessBatch
                    FROM sTKPushMpesaTransactions
                    WHERE MerchantRequestID = @MerchantRequestID
                      AND CheckoutRequestID = @CheckoutRequestID
                      AND Processed = '0'";

                var trn = await _akiba.Connection.QueryFirstOrDefaultAsync<STKRst>(lookup, new
                {
                    MerchantRequestID = callback.MerchantRequestID,
                    CheckoutRequestID = callback.CheckoutRequestID
                });

                if (trn == null || trn.trnId <= 0 || string.IsNullOrWhiteSpace(trn.productId))
                {
                    response.ErrorMsg = "No matching unprocessed transaction.";
                    return response;
                }

                var succeeded = callback.ResultCode == 0;
                var receipt = callback.CallbackMetadata?.Item?
                    .FirstOrDefault(i => string.Equals(i.Name, "MpesaReceiptNumber", StringComparison.OrdinalIgnoreCase))?
                    .Value?.ToString();
                var amount = callback.CallbackMetadata?.Item?
                    .FirstOrDefault(i => string.Equals(i.Name, "Amount", StringComparison.OrdinalIgnoreCase))?
                    .Value?.ToString();
                var transactionDate = callback.CallbackMetadata?.Item?
                    .FirstOrDefault(i => string.Equals(i.Name, "TransactionDate", StringComparison.OrdinalIgnoreCase))?
                    .Value?.ToString();

                await _akiba.Connection.ExecuteAsync(
                    @"UPDATE sTKPushMpesaTransactions
                         SET Finalized = '1',
                             Processed = '1',
                             Body = @Body,
                             Amount = @Amount,
                             MpesaReceiptNumber = @Receipt,
                             TransactionDate = @TransactionDate,
                             ResultCode = @ResultCode,
                             ResultDesc = @ResultDesc,
                             ResultOn = getdate()
                       WHERE Id = @TrnId",
                    new
                    {
                        Body = body,
                        Amount = amount,
                        Receipt = receipt,
                        TransactionDate = transactionDate,
                        ResultCode = callback.ResultCode?.ToString(),
                        ResultDesc = callback.ResultDesc,
                        TrnId = trn.trnId
                    });

                var affected = succeeded
                    ? await _akiba.Connection.ExecuteAsync(
                        @"UPDATE FlexiContributions
                             SET Approved = '1',
                                 PaymentAcknowledged = '1',
                                 Reference = @Receipt,
                                 ThirdpartyRef = @Receipt,
                                 PaymentStatus = @PaymentStatus,
                                 ApprovedOn = SYSDATETIMEOFFSET(),
                                 ApprovedDate = GETDATE(),
                                 ApprovedBy = 'MPESA',
                                 EffectiveDate = ISNULL(EffectiveDate, GETDATE()),
                                 LastModified = SYSDATETIMEOFFSET(),
                                 LastModifiedBy = 'MPESA'
                           WHERE CONVERT(nvarchar(50), Id) = @ProductId
                             AND ISNULL(PaymentAcknowledged, '0') = '0'
                             AND ISNULL(Approved, '0') = '0'",
                        new
                        {
                            Receipt = receipt,
                            PaymentStatus = (int)PaymentStatus.Approved,
                            ProductId = trn.productId
                        })
                    : await _akiba.Connection.ExecuteAsync(
                        @"UPDATE FlexiContributions
                             SET Approved = '0',
                                 PaymentAcknowledged = '0',
                                 PaymentStatus = @PaymentStatus,
                                 RejectedReason = @RejectedReason,
                                 RejectedBy = 'MPESA',
                                 RejectedDate = GETDATE(),
                                 LastModified = SYSDATETIMEOFFSET(),
                                 LastModifiedBy = 'MPESA'
                           WHERE CONVERT(nvarchar(50), Id) = @ProductId
                             AND ISNULL(PaymentAcknowledged, '0') = '0'",
                        new
                        {
                            PaymentStatus = (int)PaymentStatus.Rejected,
                            RejectedReason = callback.ResultDesc,
                            ProductId = trn.productId
                        });

                if (succeeded && affected > 0)
                {
                    await _akiba.Connection.ExecuteAsync(
                        @"UPDATE f
                             SET f.TotalFunds = ISNULL(f.TotalFunds, 0) + ISNULL(c.Total_Contribution, 0),
                                 f.LastModified = SYSDATETIMEOFFSET(),
                                 f.LastModifiedBy = 'MPESA'
                          FROM dbo.FlexiFund f
                          INNER JOIN dbo.FlexiContributions c
                            ON (c.FlexiFundId IS NOT NULL AND f.Id = c.FlexiFundId)
                            OR (c.FlexiFundId IS NULL
                                AND f.PolicyId = CONVERT(nvarchar(50), c.FlexiFuturePolicyId))
                         WHERE CONVERT(nvarchar(50), c.Id) = @ProductId",
                        new { ProductId = trn.productId });

                    var callbackRow = await _db.callBackResponse
                        .Where(a => a.CorrelationId == trn.trnId.ToString())
                        .FirstOrDefaultAsync();

                    if (callbackRow != null)
                    {
                        var payload = new ContributionCallbackResponseDTO
                        {
                            Success = true,
                            ErrorMsg = "Payment completed successfully.",
                            RequestId = callbackRow.RequestId,
                            TransactionId = receipt ?? string.Empty,
                            ProductRef = trn.productId
                        };

                        await _db.Connection.ExecuteAsync(
                            "UPDATE CallBackResponse SET ProcessResponse='1', Response=@Response WHERE Id=@Id",
                            new
                            {
                                Response = JsonSerializer.Serialize(payload),
                                Id = callbackRow.Id
                            });
                    }
                }

                response.Success = succeeded;
                response.ErrorMsg = succeeded
                    ? "Payment reconciled successfully."
                    : callback.ResultDesc ?? "Payment was not completed.";
            }
            catch (Exception ex)
            {
                _setting.LogRequests(ex.Message, "ProcessFlexiSTKResult", RequestType.Error);
                response.Success = false;
                response.ErrorMsg = "Failed to process the M-Pesa callback.";
            }

            return response;
        }
    }
}

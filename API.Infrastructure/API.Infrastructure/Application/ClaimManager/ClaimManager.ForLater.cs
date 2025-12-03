using API.Infrastructure.Interface;
using DAL.Model.Safaricom;
using DAL.ModelView;
using DAL.ModelView.Safaricom;
using Dapper;
using Mapster;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace API.Infrastructure.Application.ClaimManager
{
    internal partial class ClaimManager
    {
         public async Task<ResponseDTO> SaveForLaterClaim(ClaimRequestDTOSaveForLater request)
        {
            var response = new ResponseDTO();
            try
            {
                bool passedForProcessing = false;
                string ClaimRefNumber = "";
                string damagedPartselect = "";
                bool saveForLater = true;
                string abstractfile = "", PhoneUpload = "", ImeiUpload = "";
                string ImeiNumber = "", ImeiNumber1 = "", ImeiNumber2 = "";
                string customerId = "";
                string requestId = Guid.NewGuid().ToString();
                var phonedetails = new PhoneDetails();
                string partid = "";
                string addcommand = "addupdateClaimSaveForLater";
                var partnerdetails = await _db.Connection.QueryFirstOrDefaultAsync<PartnerDetails>("select Id ,PartnerName from  [dbo].[Partners] where [PartnerCode]='" + request.PartnerCode + "'");
                DateTime IncidenceDate = new DateTime();
                int claimstatus = (int)ClaimStatus.pending;
                //if (!string.IsNullOrEmpty(request.IncidentDate))
                //{
                //    DateTime.TryParse(request.IncidentDate, out IncidenceDate);
                //    if (IncidenceDate > DateTime.Now)
                //    {
                //        response.ErrorMsg = "Invalid Incident date";
                //        response.Success = false;
                //        passedForProcessing = false;
                //        claimstatus = (int)ClaimStatus.declined;
                //        //return response;
                //    }
                //    if (IncidenceDate.ToString() == "01/01/0001 00:00:00")
                //    {
                //        response.ErrorMsg = "Invalid Incident date";
                //        response.Success = false;
                //        passedForProcessing = false;
                //        claimstatus = (int)ClaimStatus.declined;

                //        // return response;
                //    }


                //}

              
                    


                    ClaimRefNumber = "S/" + partnerdetails.PartnerName.Substring(0, 3).ToUpper() + "/" + new Random().Next(100000, 999999);

                    //if (request.IMEINO.Count > 0)
                    //{
                    //    ImeiNumber = request.IMEINO[0];
                    //    ImeiNumber1 = request.IMEINO.Count > 1 ? request.IMEINO[1] : "";
                    //    ImeiNumber2 = request.IMEINO.Count > 2 ? request.IMEINO[2] : "";
                    //}
                    customerId = (string)await _db.Connection.ExecuteScalarAsync<string>("select Id as customerId from [dbo].[phoneInsuranceCustomers] where IdNumber='" + request.IDNumber + "'");
                    if (string.IsNullOrEmpty(customerId))
                    {
                        response.Success = false;
                        response.ErrorMsg = "Customer does not exists";
                        passedForProcessing = false;
                        claimstatus = (int)ClaimStatus.declined;
                        // return response;

                    }
                    int claimcount = 0;
                    phonedetails = await _db.Connection.QueryFirstOrDefaultAsync<PhoneDetails>("select [IMEINumber],[PhoneCost],[IMEINumber1],[IMEINumber1],[IMEINumber2]" +
                         " from [dbo].[PhoneInsuranceRequest] where Id='" + request.PhoneId + "'");
                if(phonedetails == null){
                     response.Success = false;
                        response.ErrorMsg = "Please select the customer device";
                        passedForProcessing = false;
                        claimstatus = (int)ClaimStatus.declined;
                    return response;
                }

                    claimcount = (int)await _db.Connection.ExecuteScalarAsync("select count(0) as claimcount from [dbo].[claimRequests] " +
                       "where PhoneId='" + request.PhoneId + "' and ClaimType=" + (int)ClaimType.theft + "");
                    if (claimcount > 0)
                    {
                        response.Success = false;
                        response.ErrorMsg = "This phone  has exceeded the claim limits";
                        passedForProcessing = false;
                        claimstatus = (int)ClaimStatus.declined;
                        return response;
                    }
                    //check if claim is 50% of the phone cost

                    int claimlifecount = (int)await _db.Connection.ExecuteScalarAsync("select count(0) as claimcount from [dbo].[CreditLifeUpload] " +
                       "where IdNumber='" + request.IDNumber + "' and isnull(Processed,'0') <>'1'");
                    if (claimlifecount > 0)
                    {
                        response.Success = false;
                        response.ErrorMsg = "This phone  has exceeded the claim limits";
                        passedForProcessing = false;
                        claimstatus = (int)ClaimStatus.declined;
                        return response;
                    }
                    if(request.ClaimType != null)
                {
                     if (request.ClaimType == ClaimType.theft)
                    {
                        if (request.AbstractAttachment != null)
                        {
                            abstractfile = await UploadFileAsync(request.AbstractAttachment, "AbstractAttachment" + request.CustomerName);
                            if (string.IsNullOrEmpty(abstractfile))
                            {
                                response.Success = false;
                                response.ErrorMsg = "There is no abstract attachment";
                                passedForProcessing = false;
                                claimstatus = (int)ClaimStatus.declined;
                                return response;
                            }
                        }
                        else
                        {
                            response.Success = false;
                            response.ErrorMsg = "There is no abstract attachment";
                            passedForProcessing = false;
                            claimstatus = (int)ClaimStatus.declined;
                        }
                    }
                    if (request.ClaimType == ClaimType.damage)
                    {

                        var phonecostdetails = await _db.Connection.QueryFirstOrDefaultAsync<PhoneCostDetails>("select  sum(a.[ReplacementCost]) as Amt, b.[PhoneCost] from" +
                            " [dbo].[claimRequests] a,[dbo].[PhoneInsuranceRequest] b where PhoneId='" + request.PhoneId + "' " +
                            "and a.PhoneId=b.Id and a.IDNumber='" + request.IDNumber + "' and a.Approved='1' group by a.PhoneId,b.[PhoneCost]");
                        if (phonecostdetails != null)
                        {
                            var percentage = (phonecostdetails.Amt + request.ReplacementCost) / phonecostdetails.PhoneCost;
                            if (percentage > .5)
                            {
                                response.Success = false;
                                response.ErrorMsg = "Please visit the nearest Safaricom Shop for replacement";
                                passedForProcessing = false;
                                claimstatus = (int)ClaimStatus.declined;
                            }
                        }
                        if (request.phoneUpload != null)
                        {
                            PhoneUpload = await UploadFileAsync(request.phoneUpload, "phoneUpload" + request.CustomerName);
                        }
                        else
                        {
                            response.Success = false;
                            response.ErrorMsg = "There is no phone attachment";
                            claimstatus = (int)ClaimStatus.declined;
                            passedForProcessing = false;
                        }
                        if (request.ImeiUpload != null)
                        {
                            ImeiUpload = await UploadFileAsync(request.ImeiUpload, "ImeiUpload" + request.CustomerName);
                        }
                    }
                     if (request.ClaimType == ClaimType.damage)
                    {
                        if(request.Partid != null)
                        {
                            if (request.Partid.Count > 0)
                        {
                            partid = string.Join(",", request.Partid);
                            damagedPartselect = await _db.Connection.ExecuteScalarAsync<string>("SELECT STRING_AGG([Name],' | ') as damagedPartselect FROM [PartCosts]  where Id in('" + string.Join("','", request.Partid) + "')");
                            // damagedParts = string.Join("|", damagedPartselect);
                        }
                        }
                        
                    }
                }
                   
                    var validateclaim = await ValidateClaim(new ValidateClaimDTO() { Idnumber = request.IDNumber, PhoneId = request.PhoneId });

                   
                   
                

                var param = new DynamicParameters();
                param.Add("Id", request.Id??0);
                param.Add("PartnerID", partnerdetails.Id);
                param.Add("PhoneId", request.PhoneId);
                param.Add("ClaimRefNumber", ClaimRefNumber);
                param.Add("IDNumber", request.IDNumber);
                param.Add("Narration", request.Narration);
                param.Add("ClaimType", request.ClaimType);
                param.Add("CustomerName", request.CustomerName);
                param.Add("DamagePart", damagedPartselect);
                param.Add("ReplacementCost", request.ReplacementCost);
                param.Add("IncidentDate", request.IncidentDate);
                param.Add("Abstract", abstractfile);
                param.Add("PartnerCode", request.PartnerCode);
                param.Add("RequestId", requestId);
                param.Add("UserId", request.UserId);
                param.Add("shopId", request.ShopId);
                param.Add("shoptype", request.shopType);
                // param.Add("medicalReportUpload", request.MedicalAttachment);
                param.Add("policeAbstractUpload", abstractfile);
                param.Add("imagePhoneUpload", PhoneUpload);
                param.Add("imageIMEIUpload", ImeiUpload);
                param.Add("PhoneInsuranceCustomerId", customerId);
                param.Add("claimStatus", claimstatus);
                param.Add("PartId", partid);
                param.Add("IMEINumber", phonedetails.IMEINumber);
                param.Add("IMEINumber1", phonedetails.IMEINumber1);
                param.Add("IMEINumber2", phonedetails.IMEINumber2);
                param.Add("LabourCost", request.LabourCost == null ? 0 : request.LabourCost);
                param.Add("PartCost", request.PartCost == null ? 0 : request.PartCost);
                param.Add("alternativeContact", request.AlternativeContact);
                param.Add("ErrorMessage", response.ErrorMsg);
                param.Add("policeAbstractUploadBase64",JsonConvert.SerializeObject(request.AbstractAttachment)??"");
                param.Add("imagePhoneUploadbase64", JsonConvert.SerializeObject(request.phoneUpload)??"");
                param.Add("imageIMEIUploadbase64", JsonConvert.SerializeObject(request.ImeiUpload)??"");
                
                param.Add("SourceOfClaim", request.sourceOfClaim);

                var Id = await _db.Connection.QueryFirstAsync<long>(addcommand, param, commandType: System.Data.CommandType.StoredProcedure);
                response.Success =  true ;
                response.ErrorMsg = "Your request has been received, please wait for an approval" ;
                response.ResponseId=Id.ToString();
            }
            catch (Exception ex)
            {
                _isettings.LogRequests(ex.Message, "SaveForLaterClaim", RequestType.Error);
            }
            return response;
        }
      public async Task<List<ClaimRequestDTOSaveForLater>> QuerySaveForLaterClaim(ClaimSearchSaveForLaterDTO request)
        {
            var response = new List<ClaimRequestDTOSaveForLater>();
            try
            {
                string scenario = "";
                if (!string.IsNullOrEmpty(request.request))
                {
                    scenario = " and (a.ClaimRefNumber like '%" + request.request + "%' or " +
                    "a.IDNumber like '%" + request.request + "%' or a.IMEINumber like '%" + request.request + "%')";
                }
               string claimsquery = "SELECT a.[Id],a.[PartnerID],a.[ProductID],a.[CustomerName],a.[ClaimRefNumber],a.[IDNumber],a.[Narration],a.[ClaimType]," +
                    "a.[DamagePart],a.[ReplacementCost],a.[IncidentDate],a.[ClaimDate],a.[Abstract],a.[CreatedOn],a.[PartnerCode],a.[RequestId],a.[TrnId],a.[UserId]," +
                    "a.[medicalReportUpload],a.[policeAbstractUpload],a.[imagePhoneUpload],a.[imageIMEIUpload],a.[ResponseId],a.[PhoneInsuranceCustomerId],a.[claimStatus]," +
                    "a.[PartId] as Partsid,a.[IMEINumber],a.[IMEINumber1],a.[IMEINumber2],a.[LabourCost],a.[PartCost],a.[Comments],a.[PhoneId],a.[ShopId],a.[ShopType],a.[Dispatched]," +
                    "a.[DispatchedOn],a.[NotificationNumber],a.[DispatchedCode],a.[DispatchedShopId],a.[AlternativeContact],a.[ErrorMessage],a.[policeAbstractUploadBase64]," +
                    "a.[imagePhoneUploadbase64],a.[imageIMEIUploadbase64],a.[DispatchedId],a.[SourceOfClaim]  FROM [dbo].[SaveForLaterclaim]  a," +
                    " [dbo].[PhoneInsuranceRequest] b  " +
                    "where a.[PhoneId] = b.Id and a.UserId='"+ request.UserId + "'  and ShopId="+ request.shopId + " "+ scenario +"";

                claimsquery = claimsquery + " order by a.[Id] desc";
 var claims= await _db.Connection.QueryAsync<ClaimRequestDTOSaveForLater>(claimsquery);
                response= claims.Adapt<List<ClaimRequestDTOSaveForLater>>();
            } catch (Exception ex) 
            { 
            _isettings.LogRequests(ex.Message,"QuerySaveForLaterClaim",RequestType.Error);
            }
            return response;
        }
         public async Task<ClaimRequestDTOSaveForLaterDetails> QuerySaveForLaterClaim(string Id)
        {
            var response = new ClaimRequestDTOSaveForLaterDetails();
            try
            {
                string scenario = "";
                
               string claimsquery = "SELECT a.[Id],a.[PartnerID],a.[ProductID],a.[CustomerName],a.[ClaimRefNumber],a.[IDNumber],a.[Narration],a.[ClaimType]," +
                    "a.[DamagePart],a.[ReplacementCost],a.[IncidentDate],a.[ClaimDate],a.[Abstract],a.[CreatedOn],a.[PartnerCode],a.[RequestId],a.[TrnId],a.[UserId]," +
                    "a.[medicalReportUpload],a.[ResponseId],a.[PhoneInsuranceCustomerId],a.[claimStatus]," +
                    "a.[PartId] as Partsid,a.[IMEINumber],a.[IMEINumber1],a.[IMEINumber2],a.[LabourCost],a.[PartCost],a.[Comments],a.[PhoneId],a.[ShopId],a.[ShopType],a.[Dispatched]," +
                    "a.[DispatchedOn],a.[NotificationNumber],a.[DispatchedCode],a.[DispatchedShopId],a.[AlternativeContact],a.[ErrorMessage],a.[policeAbstractUploadBase64] as abstractAttachment," +
                    "a.[imagePhoneUploadbase64] as phoneUpload,a.[imageIMEIUploadbase64] as imeiUpload,a.[DispatchedId],a.[SourceOfClaim]  FROM [dbo].[SaveForLaterclaim]  a," +
                    " [dbo].[PhoneInsuranceRequest] b  " +
                    "where a.[PhoneId] = b.Id and a.Id='"+ Id + "' ";

                claimsquery = claimsquery + " order by a.[Id] desc";
 var claims= await _db.Connection.QueryFirstOrDefaultAsync<ClaimRequestDTOSaveForLaterDetails>(claimsquery);
                response = claims.Adapt<ClaimRequestDTOSaveForLaterDetails>();
            } catch (Exception ex) 
            { 
            _isettings.LogRequests(ex.Message,"QuerySaveForLaterClaim",RequestType.Error);
            }
            return response;
        }
    }
}

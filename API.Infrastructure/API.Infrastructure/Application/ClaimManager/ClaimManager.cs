using API.Infrastructure.Interface;
using DAL;
using DAL.ModelView.Safaricom;
using DAL.ModelView;
using Dapper;
using DAL.Model;
using Mapster;
using DAL.Model.Safaricom;
using Microsoft.Office.Interop.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.EMMA;
using Azure.Core;
using Azure;
using Serilog;
using System.Text;

namespace API.Infrastructure.Application.ClaimManager
{
    internal partial class ClaimManager : IClaimPortal
    {
        readonly ApplicationDbContext _db;
        readonly Isettings _isettings;
        readonly ILogger _logger;
        public ClaimManager(ApplicationDbContext db, Isettings isettings, ILogger logger)
        {
            _db = db;
            _isettings = isettings;
            _logger = logger;
        }
        public async Task<ResponseDTO> ApproveCreditLife(CreditLifeApproveUpload approveUpload)
        {
            var response = new ResponseDTO();
            try
            {
                string procname = "ApproveCreditLifeUpload";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@userId", approveUpload.UserId);
                parameters.Add("@summaryId", approveUpload.Id);
                await _db.Connection.ExecuteAsync(procname, parameters, commandType: System.Data.CommandType.StoredProcedure);
                response.Success = true;

            }
            catch (Exception ex)
            {
                _isettings.LogRequests(ex.Message, "UploadCreditLife", RequestType.Error);
                response.Success = false;
            }
            return response;
        }

        public async Task<ResponseDTO> ValidateDispatchCode(ValidateDispatchDTO code)
        {
            var response = new ResponseDTO();
            try
            {
                string procname = "select count(0) from [claimRequests] where lower(DispatchedCode)=lower('" + code.Code + "')" +
                    " and DispatchedShopId='" + code.ShopId + "' ";

                int codecount = (int)await _db.Connection.ExecuteScalarAsync(procname);
                if (codecount > 0)
                {
                    string query = "update [claimRequests] set  DispatchedOn=getdate(),Dispatched='1', " +
                  " DispatchedShopId='" + code.ShopId + "' where Id='" + code.ClaimId + "'";

                    await _db.Connection.ExecuteAsync(query);
                    var notificationdto = new ActionsApprovalDTO()
                {
                    actionstatus = "code",
                    actiontype = ApprovalType.approve,
                    dispatchcode = code.Code,
                    narration = "",
                     RequestId=(long)Convert.ToInt32(code.ClaimId),
                      requestType= ApproveRequestType.collection

                };
                await AddApprovalNotification(notificationdto);

                    response.Success = true;
                    response.ErrorMsg = "";
                    return response;
                }
                else
                {
                      response.Success = false;
                response.ErrorMsg = "Invalid dispatch code ";
                }

                //var notificationdto = new ActionsApprovalDTO()
                //{
                //    actionstatus = "code",
                //    actiontype = ApprovalType.approve,
                //    dispatchcode = code.Code,
                //    narration = "",
                //     RequestId=(long)Convert.ToInt32(code.ClaimId),
                //      requestType= ApproveRequestType.collection

                //};
                //await AddApprovalNotification(notificationdto);

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMsg = "error while making the request ";
            }
            return response;
        }
        public async Task<ResponseDTO> ResendDispatchCode(Approvedispatch approve)
        {
            var response = new ResponseDTO();
            try
            {
                int notificationtype = (int)NotificationType.DispatchedDamage;
                string phonenumber = (string)await _db.Connection.ExecuteScalarAsync("(select AlternativeContact " +
                     " from claimRequests  where Id='" + approve.ClaimId + "')");
                string Code =  GenerateCode(5);
                string procname = "update [claimRequests] set  Dispatched='0',DispatchedCode='" + Code + "'," +
                    "DispatchedId='" + approve.UserId + "'," +
                    "DispatchedShopId='" + approve.ShopId + "' where Id='" + approve.ClaimId + "'";
                
                await _db.Connection.ExecuteAsync(procname);

                response.ErrorMsg = "Code sent to "+ MaskEmailPhone(phonenumber, OTPChannelDTO.phone);
                response.Success = true;
                if(approve.claimType== ClaimType.theft)
                {
                    notificationtype = (int)NotificationType.DispatchedTheft;
                }
                string createotpquery = " INSERT INTO [OTPValidation]([shopUserId],[OTP],[Sent],[DateCreated],ExpiresIn,PensionerId,PhoneNumber,NotificationType) " +
                    "values " +
                     " (" + approve.UserId + ",'" + Code + "',0,GETDATE(),DATEADD(hour,24,GETDATE()),0,'"+ phonenumber+"',"+ notificationtype +"); ";

                await _db.Connection.ExecuteAsync(createotpquery);

                var notificationdto = new ActionsApprovalDTO()
                {
                    actionstatus = "code",
                    actiontype = ApprovalType.approve,
                    dispatchcode = Code,
                    narration = "",
                     RequestId=(long)Convert.ToInt32(approve.ClaimId),
                      requestType= ApproveRequestType.dispatch

                };
                await AddApprovalNotification(notificationdto);

                //var addrequest = new Notificationadd()
                //{

                //    notificationType =(NotificationType) notificationtype,
                //    RequestId = approve.ClaimId,
                //    ShopId = approve.ShopId,
                //    UserId = approve.UserId,
                //    Code = Code
                //};
                //AddNotification(addrequest);

            }
            catch (Exception ex)
            {
                _isettings.LogRequests(ex.Message, "Approvedispatch", RequestType.Error);
            }
            return response;
        }
        public async Task<ResponseDTO> AppproveDispatch(Approvedispatch approve)
        {
            var response = new ResponseDTO();
            try
            {
                  int notificationtype = (int)NotificationType.DispatchedDamage;
                  string phonenumber = (string)await _db.Connection.ExecuteScalarAsync("(select b.PhoneNumber " +
                     " from claimRequests a, [phoneInsuranceCustomers] b   where a.PhoneInsuranceCustomerId= b.Id and a.Id='" + approve.ClaimId + "')");

                string Code =  GenerateCode(5);
                string procname = "update [claimRequests] set  DispatchedOn=getdate(),Dispatched='0',DispatchedCode='" + Code + "'," +
                    "DispatchedId='" + approve.UserId + "'," +
                    "DispatchedShopId='" + approve.ShopId + "' where Id='" + approve.ClaimId + "'";

                await _db.Connection.ExecuteAsync(procname);
                  response.ErrorMsg = "Dispatch Code sent to "+ MaskEmailPhone(phonenumber, OTPChannelDTO.phone);
                response.Success = true;
                if(approve.claimType== ClaimType.theft)
                {
                    notificationtype = (int)NotificationType.DispatchedTheft;
                }
                string createotpquery = " INSERT INTO [OTPValidation]([shopUserId],[OTP],[Sent],[DateCreated],ExpiresIn,PensionerId,PhoneNumber,NotificationType) values " +
                     " (" + approve.UserId + ",'" + Code + "',0,GETDATE(),DATEADD(hour,24,GETDATE()),0,'"+ phonenumber +"',"+notificationtype +"); ";

                await _db.Connection.ExecuteAsync(createotpquery);
                var notificationdto = new ActionsApprovalDTO()
                {
                    actionstatus = "code",
                    actiontype = ApprovalType.approve,
                    dispatchcode = Code,
                    narration = "",
                     RequestId=(long)Convert.ToInt32(approve.ClaimId),
                      requestType= ApproveRequestType.dispatch

                };
                await AddApprovalNotification(notificationdto);



            }
            catch (Exception ex)
            {
                _isettings.LogRequests(ex.Message, "Approvedispatch", RequestType.Error);
            }
            return response;
        }
        public async Task<ResponseDTO> SendNotification(SendNotificationRequest notification)
        {
            var response = new ResponseDTO();
            try
            {
                //  string message = await _db.Connection.ExecuteScalarAsync<string>("select top 1 [Message] from  [NotificationsType] where notificationType=" + (int)NotificationType.PhoneReady + "");

                var addrequest = new Notificationadd()
                {

                    notificationType = NotificationType.PhoneReady,
                    RequestId = notification.ClaimId,
                    ShopId = notification.ShopId,
                    UserId = notification.UserId,
                };
                AddNotification(addrequest);
                response.ErrorMsg = "Request processed successfully";
                response.Success=true; return response;

            }
            catch (Exception ex)
            {
                _isettings.LogRequests(ex.Message, "SendNotification", RequestType.Error); response.Success = false;
            }
            return response;
        }
        public async Task<ResponseDTO<UserDetails>> ClaimUserAuth(ClaimAuth repairShopUser)
        {
            var response = new ResponseDTO<UserDetails>();
            try
            {
                string query = "select distinct isnull(Id,0) as Id,shopType from vw_portalauth where" +
                    " right(Phonenumber,9)='" + repairShopUser.username.Substring(repairShopUser.username.Length - 9) + "' and isnull(iSActive,'0')<>'0' " +
                    "and isnull(IsDeleted,'0')<>'1'";
                var Id = await _db.Connection.QueryFirstOrDefaultAsync<UserDetails>(query);
                if (Id != null)
                {
                    if (Id.Id > 0)
                    {
                        // create a message for TO Safaricom 
                        var otpcode = GenerateCode(5);
                        string createotpquery = " INSERT INTO [OTPValidation]([shopUserId],[OTP],[Sent],[DateCreated],ExpiresIn," +
                            "PensionerId,PhoneNumber,NotificationType) values " +
                            " (" + Id.Id + ",'" + otpcode + "',0,GETDATE(),DATEADD(minute,3,GETDATE()),0,(select Phonenumber " +
                            "from vw_portalauth where shopType='" + Id.shopType + "' and Id=" + Id.Id + " )," + (int)NotificationType.OTP + "); ";
                        await _db.Connection.ExecuteAsync(createotpquery);

                        response.ErrorMsg = "";
                        response.Success = true;
                        response.Result = Id;
                        // response.

                    }
                    else
                    {
                        response.ErrorMsg = "Could not verify login details";
                        response.Success = false; return response;

                    }
                }
                else
                {
                    response.ErrorMsg = "Could not verify login details";
                    response.Success = false; return response;
                }
                

            }
            catch (Exception ex)
            {
                _isettings.LogRequests(ex.Message, "ClaimUserAuth", RequestType.Error);
            }
            return response;
        }
        public async Task<ResponseDTO> ResendOTP(string UserId,int shoptype)
        {
            var response = new ResponseDTO();
            try
            {
                var otpcode =  GenerateCode(5);
                string createotpquery = " INSERT INTO [OTPValidation]([shopUserId],[OTP],[Sent],[DateCreated],ExpiresIn,PensionerId,PhoneNumber) values " +
                        " (" + UserId + ",'" + otpcode + "',0,GETDATE(),DATEADD(minute,3,GETDATE()),0,(select Phonenumber " +
                        "from vw_portalauth where shopType='"+ shoptype +"' and Id="+ UserId +" )); ";
                await _db.Connection.ExecuteAsync(createotpquery);
                response.ErrorMsg = "";
                response.Success = true;

            }
            catch (Exception ex)
            {
                _isettings.LogRequests(ex.Message, "VerifyOTP", RequestType.Error);
            }
            return response;
        }
       
        public async Task<ResponseDTO> MakeClaim(ClaimRequestDTOPortal request)
        {
            var response = new ResponseDTO();
            bool passedForProcessing = true;
            try
            {
               response.ErrorMsg = "Your request was received, please wait for an approval";
                var partnerdetails = await _db.Connection.QueryFirstOrDefaultAsync<PartnerDetails>("select Id ,PartnerName from  [dbo].[Partners] where [PartnerCode]='" + request.PartnerCode + "'");
                DateTime IncidenceDate = DateTime.Now;
                int claimstatus = (int)ClaimStatus.pending;
                if (!string.IsNullOrEmpty(request.IncidentDate))
                {
                    DateTime.TryParse(request.IncidentDate, out IncidenceDate);

                }
                if (IncidenceDate > DateTime.Now)
                {
                    response.ErrorMsg = "Invalid Incident date";
                    response.Success = false;
                    passedForProcessing = false;
                    claimstatus = (int)ClaimStatus.declined;
                    //return response;
                }
                if (IncidenceDate.ToString() == "01/01/0001 00:00:00")
                {
                    response.ErrorMsg = "Invalid Incident date";
                    response.Success = false;
                    passedForProcessing = false;
                    claimstatus = (int)ClaimStatus.declined;

                    // return response;
                }
                string ImeiNumber = "", ImeiNumber1 = "", ImeiNumber2 = "";
                string customerId = "";

                string ClaimRefNumber = "";
                string requestId = Guid.NewGuid().ToString();
                int refcount = 0;
                do
                {
                    ClaimRefNumber = "S/" + partnerdetails.PartnerName.Substring(0, 3).ToUpper() + "/" +GenerateCode(10) ;
                    refcount = Convert.ToInt16(_db.Connection.ExecuteScalar("select count(1) from  claimRequests where ClaimRefNumber ='" + ClaimRefNumber + "' "));
                } while (refcount >0 );
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
                var phonedetails = await _db.Connection.QueryFirstOrDefaultAsync<PhoneDetails>("select [IMEINumber],[PhoneCost],[IMEINumber1],[IMEINumber1],[IMEINumber2]" +
                      " from [dbo].[PhoneInsuranceRequest] where Id='" + request.PhoneId + "'");

                claimcount = (int)await _db.Connection.ExecuteScalarAsync("select count(0) as claimcount from [dbo].[claimRequests] " +
                   "where PhoneId='" + request.PhoneId + "' and ClaimType=" + (int)ClaimType.theft + " and isnull(Approved,'0') ='1' ");
                if (claimcount > 0)
                {
                    response.Success = false;
                    response.ErrorMsg = "This phone  has exceeded the claim limits";
                    passedForProcessing = false;
                    claimstatus = (int)ClaimStatus.declined;
                    //return response;
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
                    //return response;
                }
                string abstractfile = "", PhoneUpload = "", ImeiUpload = "",deactivatitionproof="";
                byte[] deactivattionproofbyte= null;
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
                        }
                    }
                    else
                    {
                        response.Success = false;
                        response.ErrorMsg = "There is no abstract attachment";
                        passedForProcessing = false;
                        claimstatus = (int)ClaimStatus.declined;
                    }
                    if (request.deactivationProof != null)
                    {
                       
                        var deactivatetion = await UploadFileBytesAsync(request.deactivationProof, "deactivationProof" + request.CustomerName);
                        deactivattionproofbyte = Encoding.ASCII.GetBytes(request.deactivationProof.data);
                        if (string.IsNullOrEmpty(deactivatetion.Item1))
                        {
                            

                            response.Success = false;
                            response.ErrorMsg = "There is no deactivationProof attachment";
                            passedForProcessing = false;
                            claimstatus = (int)ClaimStatus.declined;
                        }
                    }
                    else
                    {
                        response.Success = false;
                        response.ErrorMsg = "There is no deactivationProof attachment";
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
                            response.ErrorMsg = "Repair cost exceeds the predefined threshold. Please inform the customer to wait for further instructions via SMS";
                            passedForProcessing = false;
                            claimstatus = (int)ClaimStatus.pending;
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
                var validateclaim = await ValidateClaim(new ValidateClaimDTO() { Idnumber = request.IDNumber, PhoneId = request.PhoneId });
                string damagedPartselect = ""; string partid = "";
                if (request.ClaimType == ClaimType.damage)
                {
                    if (request.Partid.Count > 0)
                    {
                        partid = string.Join(",", request.Partid);
                        damagedPartselect = await _db.Connection.ExecuteScalarAsync<string>("SELECT STRING_AGG([Name],' | ') as damagedPartselect FROM [PartCosts]  where Id in('" + string.Join("','", request.Partid) + "')");
                        // damagedParts = string.Join("|", damagedPartselect);
                    }
                }


                string addcommand = "addClaim";
                var param = new DynamicParameters();
                param.Add("PartnerID", partnerdetails.Id);
                param.Add("PhoneId", request.PhoneId);
                param.Add("ClaimRefNumber", ClaimRefNumber);
                param.Add("IDNumber", request.IDNumber);
                param.Add("Narration", request.Narration);
                param.Add("ClaimType", request.ClaimType);
                param.Add("CustomerName", request.CustomerName);
                param.Add("DamagePart", damagedPartselect);
                param.Add("ReplacementCost", request.ReplacementCost);
                param.Add("IncidentDate", IncidenceDate);
                param.Add("Abstract", abstractfile);
                param.Add("Processed", '0');
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
                param.Add("passedForProcessing", passedForProcessing);
                param.Add("ErrorMessage", response.ErrorMsg);
                param.Add("policeAbstractUploadBase64", request.AbstractAttachment?.data);
                param.Add("imagePhoneUploadbase64", request.phoneUpload?.data);
                param.Add("imageIMEIUploadbase64", request.ImeiUpload?.data);
                param.Add("deactivationProof", request.deactivationProof?.data);
                param.Add("SourceOfClaim", request.sourceOfClaim);
                param.Add("saveForLaterId", request.SaveForLaterId);

                var Id = await _db.Connection.QueryFirstAsync<long>(addcommand, param, commandType: System.Data.CommandType.StoredProcedure);
                response.Success = passedForProcessing == true ? true : false;
                response.ErrorMsg = passedForProcessing == true ? "Your request has been received, please wait for an approval" : response.ErrorMsg;
                var actions = new ActionsDTO()
                {
                    ActionDescription = "Create a new Claim request",
                    ActionName = "Create Claim",
                    ClaimType =request.ClaimType,
                    IncidenceDate = request.IncidentDate.ToString(),
                    Reference = ClaimRefNumber,
                    RequestId = Id.ToString(),
                    ShopId = request.ShopId,
                    ShopType = request.shopType.ToString(),
                    userId = request.UserId,

                };
                await AddActions(actions);
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMsg = ex.Message;
                _isettings.LogRequests(ex.Message, "MakeClaim", RequestType.Error);
            }
            return response;
        }
        public async Task<List<PartsCostDTO>> getPartsCost(PartsQuery parts)
        {
            var response = new List<PartsCostDTO>();
            try
            {
                string query = "SELECT a.[Id],a.[Name],a.[Description],a.[Active],a.[CreatedOn] ,a.[LaborCost] ,a.[PartCosts] " +
                    ",a.[ReplacementLimit] FROM [dbo].[PartCosts] a,[dbo].[PhoneInsuranceRequest] b where a.ModelName= b.PhoneModel " +
                    " and b.Id='" + parts.DeviceId + "'";
                //string query = "SELECT [Id],[Name],[Description],[Active]," +
                //    "[CreatedOn] ,[LaborCost] ,[PartCosts] ,[ReplacementLimit] FROM [dbo].[PartCosts]" +
                //    " where isnull(Active,'0')='1' and ";
                var result = await _db.Connection.QueryAsync<PartsCostDTO>(query);
                response = result.Adapt<List<PartsCostDTO>>();
                _logger.Error("test errors");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _isettings.LogRequests(ex.Message, "getPartsCost", RequestType.Error);
            }
            return response;
        }
        public async Task<ResponseDTO<RepairShopDTO>> AuthVerifyOTP(VerifyOTP verifyOTP)
        {
            var response = new ResponseDTO<RepairShopDTO>();
            try
            {
                string query = "select top 1 Convert(int, DATEDIFF(minute,[ExpiresIn],getdate())) as expires,OTP from [dbo].[OTPValidation] where [shopUserId] =" +
                    verifyOTP.UserId + "  order by id desc";
                var expires = await _db.Connection.QueryFirstAsync<OTPValidation>(query);
                if (expires != null)
                {
                   
                    if (Convert.ToInt16(expires.expires) < 3)
                    { 
                        if(expires.OTP.ToLower()== verifyOTP.OTP.ToLower())
                        {
                            string authquery = "select distinct Id,ShopName,Phonenumber,Email,ContactName,PartnerCode,loginType,shopType," +
                            "ProductCode,Location from vw_portalauth where" +
                      " Id ='" + verifyOTP.UserId + "'";
                        var repairshop = await _db.Connection.QueryFirstOrDefaultAsync<RepairShopDTO>(authquery);

                        response.Result = repairshop;
                        response.Success = true;
                        response.ErrorMsg = "";
                        return response;
                        }
                        else
                        {
                            response.ErrorMsg = "Invalid OTP";
                        response.Success = false;

                        }
                        
                    }
                    else
                    {
                        response.ErrorMsg = "OTP expired";
                        response.Success = false;

                    }
                }
                else
                {
                    response.Success = false;
                    response.ErrorMsg = "Invalid OTP , please try again";
                }

            }
            catch (Exception ex)
            {
                _isettings.LogRequests(ex.Message, "VerifyOTP", RequestType.Error);
            }
            return response;
        }
        public string GenerateCode(int length)
        {
            //  var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var chars = "123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789123456789";
            var stringChars = new char[length];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);
            return finalString;
        }

    }
    public class OTPValidation
    {
        public string OTP { get; set; }
        public int expires { get;set; }
    }
    public class PartnerDetails
    {
        public int Id { get; set; }
        public string PartnerName { get; set; }
    }
    public class PhoneDetails
    {
        public string? IMEINumber { get; set; }
        public string? IMEINumber1 { get; set; }
        public string? IMEINumber2 { get; set; }
        public string? PhoneCost { get; set; } = null;

    }
    public class PhoneCostDetails
    {
        public double? Amt { get; set; }
        public double? PhoneCost { get; set; }
    }
    public class UserDetails
    {
        public int shopType { get; set; }
        public int Id { get; set; }
    }

    
}

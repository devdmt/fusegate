using DAL.ModelView.Pension;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Infrastructure.Interface;
using DAL;
using Dapper;
using PhoneNumbers;
using DAL.ModelView;
using Newtonsoft.Json;
using Azure.Core;
using DAL.Model;
namespace API.Infrastructure.Application.Pension
{
    internal partial class PensionManager:IPension
    {
        readonly ApplicationDbContext _db;
       readonly MainDbContext _mainDb;
       
        readonly Isettings _settings;
        readonly AkibappDbContext _akiba;
        public PensionManager(AkibappDbContext akibaDb, Isettings settings,ApplicationDbContext db, MainDbContext mainDb)
        {
            _db = db;
            _mainDb = mainDb;
            _settings = settings;
            
            _akiba = akibaDb;
           
        }
       
        public async Task<OnboardResponse> OnboardingRequest(PensionOnboardingDTO request,string Ip,string PartnerId)
        {
            var response = new OnboardResponse();
            string requestId = "";
            try
            {
             requestId= await _settings.AddRequest( new ApiRequestsDTO() { ApiName="/api/v1/pension/oboarding", IP=Ip,
                    PayLoad = JsonConvert.SerializeObject(BuildAuditPayload(request)), RequestName="OnboardingRequest", RequestType=(int)ApiRequestType.Create});
                   var util = PhoneNumberUtil.GetInstance();
        var parsed = util.Parse(request.PhoneNumber, null); // null = don't assume a default region
                
                if (!parsed.HasCountryCode)
                {
                    response.ErrorMsg = "Please add the full phonenumber e.g +254712345678"; 
                    response.Success=false;
                    response.ResponseId = string.Empty;
                    return response;
                }

                var idNumberExists = await _akiba.Connection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(1) FROM [dbo].[Customers] WHERE [Idnumber] = @Idnumber and Active='1'",
                    new { Idnumber = request.IDNumber }
                ) > 0;

                if (idNumberExists)
                {
                    response.ErrorMsg = "Customer with similar details already exists in the system.";
                    response.Success = false;
                    response.ResponseId = string.Empty;
                    return response;
                }
                 var phoneNumberExists = await _akiba.Connection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(1) FROM [dbo].[Customers] WHERE right(PhoneNumber,9) = @PhoneNumber and Active='1'", new {PhoneNumber = parsed.NationalNumber.ToString() }) > 0;

                if (phoneNumberExists)
                {
                    response.ErrorMsg = "Customer with similar details already exists in the system.";
                    response.Success = false;
                    response.ResponseId = string.Empty;
                    return response;
                }
                 
                
                string countrycode=parsed.CountryCode.ToString();
              string customerId=Guid.NewGuid().ToString();
                string referalcode;
do { referalcode =  _settings.GenerateRadomCode(); }
while (await _akiba.Connection.ExecuteScalarAsync<int>("select " +
"count(1) from [dbo].[Customers] where [RefferalCode]=@RefferalCode", new { RefferalCode = referalcode }) >0);
                string fullname = request.FirstName;
                
                if (!string.IsNullOrEmpty(request.OtherNames)) 
                    fullname += " " + request.OtherNames;
            
                      string memberNo="";
do { memberNo= DateTime.Now.ToString("yy") + "-" + _settings.GenerateRadomCode(6); }
while (await _akiba.Connection.ExecuteScalarAsync<int>("select " +
"count(1) from [dbo].[Customers] where [MemberNumber]=@MemberNumber", new { MemberNumber = memberNo }) >0);
              
                const string addcustomerQuery = "INSERT INTO [dbo].[Customers]  ([Id] ,[Fullname] ,[CustomerType]" +
                    " ,[DateOfBirth] ,[Email] ,[PhoneNumber] ,[KRAPin] ,[AddressLine1] ,[Created] ,[CreatedBy]" +
                    " ,[CreatedFromIP] ,[Approved] ,[Confirmed],[ApprovalStatus] ,[Firstname] " +
                    ",[Lastname] ,[MemberNumber] ,[EmployementStatus] ,[Gender] ,[Idnumber]" +
                    " ,[MaritalStatus] ,[MemberJoinDate] ,[NSSFCode] ,[Occupation] " +
                    " ,[StaffNumber] ,[Residency] ,[CountryCode] " +
                    ",[RefferalCode] " +
                    ",[RegisterChannel]) VALUES (@Id,@Fullname,@CustomerType,@DateOfBirth,@Email,@PhoneNumber,@KRAPin,@AddressLine1,getdate(),@CreatedBy,@CreatedFromIP," +
                    "@Approved,@Confirmed,@ApprovalStatus,@Firstname,@Lastname,@MemberNumber,@EmployementStatus,@Gender,@Idnumber,@MaritalStatus,@MemberJoinDate,@NSSFCode,@Occupation," +
                    "@StaffNumber,@Residency,@CountryCode,@RefferalCode,@RegisterChannel)";
                var customerPayload = new
                {
                    Id = customerId,
                    Fullname = fullname,
                    CustomerType = (int)CustomerType.Individual,
                    request.DateOfBirth,
                    request.Email,
                    PhoneNumber = request.PhoneNumber,
                    KRAPin = "",
                    AddressLine1 = "",
                    CreatedBy = "API",
                    CreatedFromIP = Ip,
                    Approved = false,
                    Confirmed = false,
                    ApprovalStatus = 0,
                    request.FirstName,
                    Lastname = request.OtherNames,
                    MemberNumber = memberNo,
                    EmploymentStatus=  "",
                    Gender = (int)request.Gender,
                    Idnumber = request.IDNumber,
                    
                    // request.Marital_Status,
                    MemberJoinDate = "",
                    NSSFCode = "",
                    request.Occupation,
                  //  request.Staff_Number,
                    request.Residency,
                    CountryCode = countrycode,
                    RefferalCode = referalcode,
                    RegisterChannel = (int)RegistrationChannel.API
                };
                await _akiba.Connection.ExecuteAsync(addcustomerQuery, customerPayload);
foreach(var product in request.Product_Type)
{

    string  addProductQuery="INSERT INTO [dbo].[PensionerFund]([Id] ,[CustomerId] " +
                        ",[AccountType] ,[Approved],[RefNo] ,[Created] ,[CreatedBy] " +
                        ",[CreatedFromIP],ProductTypes,FundStatus,PartnerId)  " +
                        "VALUES (newid(),@CustomerId,@AccountType,@Approved,@RefNo,getdate()," +
                        "'API',@Ip,@ProductTypes,@FundStatus,@PartnerId)";
    await _akiba.Connection.ExecuteAsync(addProductQuery, new { CustomerId = customerId,
        AccountType = (int)CustomerType.Individual, Approved = false, RefNo = referalcode,
        Ip = Ip,ProductTypes=(int)product,FundStatus= 2,PartnerId=PartnerId });
}


                 const string addEsbcustomerQuery = "INSERT INTO [dbo].[AkibaCustomers]  ([Id] ,[Fullname] ,[CustomerType]" +
                    " ,[DateOfBirth] ,[Email] ,[PhoneNumber] ,[KRAPin] ,[AddressLine1] ,[Created] ,[CreatedBy]" +
                    " ,[CreatedFromIP] ,[Approved] ,[Confirmed],[ApprovalStatus] ,[Firstname] " +
                    ",[Lastname] ,[MemberNumber] ,[EmployementStatus] ,[Gender] ,[Idnumber]" +
                    " ,[MaritalStatus] ,[MemberJoinDate] ,[NSSFCode] ,[Occupation] " +
                    " ,[StaffNumber] ,[Residency] ,[CountryCode] " +
                    ",[RefferalCode] " +
                    ",[RegisterChannel]) VALUES (@Id,@Fullname,@CustomerType,@DateOfBirth,@Email,@PhoneNumber,@KRAPin,@AddressLine1,getdate(),@CreatedBy,@CreatedFromIP," +
                    "@Approved,@Confirmed,@ApprovalStatus,@Firstname,@Lastname,@MemberNumber,@EmployementStatus,@Gender,@Idnumber,@MaritalStatus,@MemberJoinDate,@NSSFCode,@Occupation," +
                    "@StaffNumber,@Residency,@CountryCode,@RefferalCode,@RegisterChannel)";
                await _db.Connection.ExecuteAsync(addEsbcustomerQuery, customerPayload);
                response.MemberNo = memberNo;
                response.Success = true;
                response.ErrorMsg = "";
                response.ResponseId = requestId;

                _settings.UpdateRequest(new UpdateRequestsDTO() { ErrorMsg = "", Failed = false, 
                    Id = requestId, Responded = true, Response = JsonConvert.SerializeObject(response) });
                return response;
            }
            catch (Exception ex)
            {
                  _settings.UpdateRequest(new UpdateRequestsDTO() { ErrorMsg = ex.Message, Failed = true, 
                    Id = requestId, Responded = true, Response = JsonConvert.SerializeObject(response) });
                _settings.LogRequests( ex.Message + "|" + ex.StackTrace,"PensionOnboardingError",RequestType.Error);
                // throw new RecovXException(ex.Message);
            }

            return response;
        }

        private static object BuildAuditPayload(PensionOnboardingDTO request) =>
            new
            {
                request.FirstName,
                request.OtherNames,
                request.Email,
                PhoneNumber = MaskValue(request.PhoneNumber),
                IdNumber = MaskValue(request.IDNumber),
                EmployementStatus="",
                Marital_Status="",
                request.Product_Type
            };

        private static string MaskValue(string? value, int unmaskedChars = 4)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            var safeUnmasked = Math.Min(Math.Max(unmaskedChars, 0), value.Length);
            var maskedLength = value.Length - safeUnmasked;
            var suffix = safeUnmasked == 0 ? string.Empty : value[^safeUnmasked..];
            return new string('*', maskedLength) + suffix;
        }
    }
}

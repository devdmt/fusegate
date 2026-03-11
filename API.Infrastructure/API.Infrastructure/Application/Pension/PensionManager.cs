using API.Infrastructure.Interface;
using API.Infrastructure.Otp;
using Azure;
using Azure.Core;
using DAL;
using DAL.Model;
using DAL.ModelView;
using DAL.ModelView.Pension;
using Dapper;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
namespace API.Infrastructure.Application.Pension
{
    internal partial class PensionManager:IPension
    {
        readonly ApplicationDbContext _db;
       readonly MainDbContext _mainDb;
        readonly IPay _ipay;
        readonly Isettings _settings;
        readonly AkibappDbContext _akiba;
        readonly OtpSettings _otpSettings;

        public PensionManager(AkibappDbContext akibaDb, Isettings settings, ApplicationDbContext db,
            MainDbContext mainDb, IOptions<OtpSettings> otpSettings,IPay pay)
        {
            _db = db;
            _mainDb = mainDb;
            _settings = settings;
            _akiba = akibaDb;
            _otpSettings = otpSettings?.Value ?? new OtpSettings();
            _ipay = pay;
        } 
         public static  (int age,bool success,string? error) CalculateAge(string birthInfo)
            {
                 // Updated: single parameter for birth info that could be a date or year string
            // Usage: input like "2001-01-01" (date) or "2001" (year)
                if (string.IsNullOrWhiteSpace(birthInfo))
                    return (0,false,"invalid date of birth information");

                DateTime dob;

                // If birthInfo is a 4-digit year
                if (birthInfo.Length == 4 && int.TryParse(birthInfo, out int yearOnly))
                {
                    // Assume January 1st of that year
                    dob = new DateTime(yearOnly, 1, 1);
                }
                else
                {
                    string[] dateFormats = new[]
                    {
                        "dd-MMM-yyyy",   // e.g. 01-Jan-2001
                        "dd/MM/yyyy",    // e.g. 01/01/2001
                        "dd-MM-yyyy",    // e.g. 01-01-2001
                        "yyyy-MM-dd",    // e.g. 2001-01-01
                        "MM/dd/yyyy",    // e.g. 01/01/2001 (US)
                        "yyyy/MM/dd",    // e.g. 2001/01/01
                        "d/M/yyyy",
                        "d-M-yyyy"
                    };

                    if (!DateTime.TryParseExact(birthInfo, dateFormats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dob))
                    {
                        // fallback to DateTime.TryParse (handles e.g. locale default)
                        if (!DateTime.TryParse(birthInfo, out dob))
                            //throw new ArgumentException("Invalid date of birth or year format.");
                          return (0,false,"Invalid date of birth or year format.");
                    }
                }

                var today = DateTime.Today;
                int age = today.Year - dob.Year;
                if (today.Month < dob.Month || (today.Month == dob.Month && today.Day < dob.Day))
                    age--;

                return (age,true,""); // Per original spec: Add 1 to calculated age
            }
        
        public async Task<PensionQuoteResponse> GetQuote(PensionCalculatorDTO request)
        {
            var response= new PensionQuoteResponse();
            try
            {
                   double monthlycontribution = 0;
                double contributions = request.MonthlyContribution;
                int retirementAge = request.RetireAge;
                int currentage = 0;
                var ageresult = CalculateAge(request.DateOfBirth);
                if (!ageresult.success)
                {
                    response.Error = ageresult.error;
                    response.Success = false;
                    return response;
                }
                currentage = ageresult.age;
                switch (request.frequency)
                {
                    case  ContributionFrequency.daily:

                        monthlycontribution = contributions * 30;
                        break;
                    case  ContributionFrequency.weekly:
                        monthlycontribution = contributions * 4;
                        break;
                    case  ContributionFrequency.monthly:
                        monthlycontribution = contributions;
                        
                        break;
                    default:
                         monthlycontribution = contributions * 30;
                        break;
                }
                int rate = 5;
                switch (request.InterestGrowthRate)
                {
                    case interestGrowthRate.guaranteed:
                        rate= 5;
                        break;
                    case interestGrowthRate.moderate:
                        rate = 8;
                        break;
                    case interestGrowthRate.aggressive:
                        rate = 12;
                        break;
                }
                
                //double.TryParse(res.request, out monthlycontribution);
                var amt = await CalculateTRP(monthlycontribution, retirementAge, currentage, 0, rate, "Monthly");
                response.TotalPot = amt;
                response.DateOfBirth = request.DateOfBirth;
                response.DesiredRetirementIncome = request.RetireIncome;
                response.Age= ageresult.age;
                 response.RetirementAge = request.RetireAge.ToString();
                response.Success = true;

                return response;

            }
            catch (Exception ex) 
            {
                response.Success = false;
                response.Error = "An error occurred while calculating the pension quote. Please check your input and try again.";
                _settings.LogRequests(ex.Message, "GetQuote", RequestType.Error);
            }
        
        return response;
        }

         public async Task<double> CalculateTRP(
    double monthlyPersonalContributions,
    int retirementAge,
    int currentAge,
    double totalCombinedPension,
    double growthRate = 5,
    string contributionMode = "Monthly")
{

        int months = CalculateMTR(retirementAge, currentAge);

        var compoundInterest = CalculateCompoundInterest(
            monthlyPersonalContributions,
            totalCombinedPension,
            growthRate,
            months,
            contributionMode
        );

        return Math.Round(compoundInterest.totalAmount, 0);
   
}
          public  (double totalInterest, double totalAmount) CalculateCompoundInterest(
        double personalMonthlyContribution,
        double oneTimeContribution,
        double annualRate,
        int months,
        string type = "Monthly")
    {
        double monthlyRate = Math.Pow(1 + annualRate / 100, 1.0 / 12) - 1;
        double monthlyContribution = personalMonthlyContribution;
        double futureValue = 0;

        if (type == "both")
        {
            futureValue = oneTimeContribution;
        }
        else if (type == "Monthly")
        {
            futureValue = monthlyContribution;
        }
        else if (type == "One-time")
        {
            futureValue = oneTimeContribution;
        }

        double interestEarned = 0;

        for (int i = 0; i < months; i++)
        {
            double periodInterest = futureValue * monthlyRate;

            if (type != "One-time")
            {
                futureValue += (monthlyContribution + periodInterest);
            }
            else
            {
                futureValue += periodInterest;
            }

            interestEarned += periodInterest;
        }

        if (type == "Monthly")
        {
            futureValue -= monthlyContribution;
        }

        return (Math.Round(interestEarned, 2), futureValue);
    }
         public  int CalculateMTR(int retirementAge, int currentAge)
    {
        return (retirementAge - currentAge) * 12;
    }

        public async Task<OnboardResponse> OnboardingRequest(PensionOnboardingDTO request, string Ip, string PartnerId)
        { 
           
        var response = new OnboardResponse();
            string requestId = "";
            try
            {
             requestId= await _settings.AddRequest( new ApiRequestsDTO() { ApiName="/api/v1/pension/oboarding", IP=Ip,
                    PayLoad = JsonConvert.SerializeObject(BuildAuditPayload(request)), RequestName="OnboardingRequest", RequestType=(int)ApiRequestType.Create});
                   var util = PhoneNumberUtil.GetInstance();
        // Set default region to Kenya (KE) for phone number parsing
        var parsed = util.Parse(request.PhoneNumber, "KE");
        
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
                    ",[Lastname] ,[MemberNumber] ,[Gender] ,[Idnumber]" +
                    " , " +
                    " [Residency] ,[CountryCode] " +
                    ",[RefferalCode] " +
                    ",[RegisterChannel]) VALUES (@Id,@Fullname,@CustomerType,@DateOfBirth,@Email,@PhoneNumber,@KRAPin,@AddressLine1,getdate(),@CreatedBy,@CreatedFromIP," +
                    "@Approved,@Confirmed,@ApprovalStatus,@Firstname,@Lastname,@MemberNumber,@Gender,@Idnumber," +
                    "@Residency,@CountryCode,@RefferalCode,@RegisterChannel)";
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
                   // EmploymentStatus=  "",
                    Gender = (int)request.Gender,
                    Idnumber = request.IDNumber,
                    
                    // request.Marital_Status,
                  
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
                        ",[AccountType] ,[Approved],[RefNo] ,[CreatedBy] " +
                        ",[CreatedFromIP],ProductTypes,FundStatus)  " +
                        "VALUES (@Id,@CustomerId,@AccountType,@Approved,@RefNo," +
                        "'API',@Ip,@ProductTypes,@FundStatus)";
    await _akiba.Connection.ExecuteAsync(addProductQuery, new {Id=Guid.NewGuid().ToString(), CustomerId = customerId,
        AccountType = (int)CustomerType.Individual, Approved = false, RefNo = referalcode,
        Ip = Ip,ProductTypes=(int)product,FundStatus= 2 });
}


                 const string addEsbcustomerQuery = "INSERT INTO [dbo].[AkibaCustomers]   ([Id] ,[Fullname] ,[CustomerType]" +
                    " ,[DateOfBirth] ,[Email] ,[PhoneNumber] ,[KRAPin] ,[AddressLine1] ,[Created] ,[CreatedBy]" +
                    " ,[CreatedFromIP] ,[Approved] ,[Confirmed],[ApprovalStatus] ,[Firstname] " +
                    ",[Lastname] ,[MemberNumber] ,[Gender] ,[Idnumber]" +
                    " , " +
                    " [Residency] ,[CountryCode] " +
                    ",[RefferalCode] " +
                    ",[RegisterChannel]) VALUES (@Id,@Fullname,@CustomerType,@DateOfBirth,@Email,@PhoneNumber,@KRAPin,@AddressLine1,getdate(),@CreatedBy,@CreatedFromIP," +
                    "@Approved,@Confirmed,@ApprovalStatus,@Firstname,@Lastname,@MemberNumber,@Gender,@Idnumber," +
                    "@Residency,@CountryCode,@RefferalCode,@RegisterChannel)";
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

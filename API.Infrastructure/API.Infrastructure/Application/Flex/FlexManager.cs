using API.Infrastructure.Interface;
using DAL;
using DAL.Model;
using DAL.Model.LastExpense;
using DAL.ModelView;
using DAL.ModelView.Flex;
using DAL.ModelView.FuneralExpense;
using Dapper;
using Mapster;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace API.Infrastructure.Application.Flex
{
    internal partial class FlexManager:IflexManager
    {
        readonly ApplicationDbContext _db;
        readonly Isettings _isettings;
        readonly ILogger<FlexManager> _logger;
    readonly IComunication _icomm;
        public FlexManager(ApplicationDbContext db, Isettings isettings, ILogger<FlexManager> logger, IComunication icomm)
        {
            _db = db;
            _isettings = isettings;
            _logger = logger;
            _icomm = icomm;
        }

        // Validates if the input string is a valid phone number in E.164 or local formats (basic check)
        // You might want to use a more robust library like libphonenumber-csharp for production scenarios
        public bool IsValidPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        // Remove spaces, dashes, and parentheses for a simple check
        string cleaned = new string(phoneNumber.Where(c => char.IsDigit(c) || c == '+').ToArray());

        // E.164 numbers start with +, are at least 10 digits (excluding '+'), and usually at most 15
        if (cleaned.StartsWith("+"))
        {
            var digits = cleaned.Substring(1);
            return digits.All(char.IsDigit) && digits.Length >= 10 && digits.Length <= 15;
        }

        // Local phone number: at least 9 digits, could tweak for specific countries
        return cleaned.All(char.IsDigit) && cleaned.Length >= 9 && cleaned.Length <= 12;
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

                return (age + 1,true,""); // Per original spec: Add 1 to calculated age
            }
      
      
    public async  Task CreateLead(LeadsDTO leads)
    {
        try
        {
           
            const string sql = @"
INSERT INTO [dbo].[CustomerLeads]
(
    [contributionMode],
    [countryCode],
    [currentAge],
    [dateOfBirth],
    [desiredRetirementIncome],
    [sumAssured],
    [employerContribution],
    [growthRate],
    [oneTimeContribution],
    [personalContribution],
    [phoneNumber],
    [fullName],
    [potDepletion],
    [retirementAge],
    [retirementIncome],
    [totalCP],
    [totalRetirementPot],
    [yearItWillLast],
    [CreatedOn],
    product,
OtherDetails
)
VALUES
(
    @contributionMode,
    @countryCode,
    @currentAge,
    @dateOfBirth,
    @desiredRetirementIncome,
    @sumAssured,
    @employerContribution,
    @growthRate,
    @oneTimeContribution,
    @personalContribution,
    @phoneNumber,
    @fullName,
    @potDepletion,
    @retirementAge,
    @retirementIncome,
    @totalCP,
    @totalRetirementPot,
    @yearItWillLast,
    @CreatedOn,
    @product,
@OtherDetails
);";

            var parameters = new
            {
                contributionMode = leads.ContributionMode,
                countryCode = leads.CountryCode,
                currentAge = leads.CurrentAge,
                dateOfBirth = leads.DateOfBirth,
                desiredRetirementIncome = leads.DesiredRetirementIncome,
                sumAssured = leads.SumAssured,
                employerContribution = leads.EmployerContribution,
                growthRate = leads.GrowthRate,
                oneTimeContribution = leads.OneTimeContribution,
                personalContribution = leads.PersonalContribution,
                phoneNumber = leads.PhoneNumber,
                fullName = leads.FullName,
                potDepletion = leads.PotDepletion,
                retirementAge = leads.RetirementAge,
                retirementIncome = leads.RetirementIncome,
                totalCP = leads.TotalCP,
                totalRetirementPot = leads.TotalRetirementPot,
                yearItWillLast = leads.YearItWillLast,
                CreatedOn = leads.CreatedOn ?? DateTime.UtcNow,
                product= leads.Product,
                OtherDetails=leads.OtherDetails,
            };

            await _db.Connection.ExecuteAsync(sql, parameters);

            
        }
        catch (Exception ex)
        {
            _isettings.LogRequests(
                $"Error in addleads: {ex.Message}",
                "addleads",
                Interface.RequestType.Error);

        }
    }
//         public async Task<FuneralExpenseQuotationResponseDto> GetQuote(FuneralExpenseQuotationDto request)
//    {
//        try
//        {
           
            
//                if (request.PolicyTerm <= 0)
//                {
//                    throw new InvalidOperationException("Invalid Policy Term");
//                }
//                if (string.IsNullOrEmpty(request.PartnerId.ToString()))
//                {
//                    throw new InvalidOperationException("Invalid partnerId");
                  
//                }
//                if (string.IsNullOrEmpty(request.AgentId.ToString()))
//                {
//                    throw new InvalidOperationException("Invalid AgentId");

//                }

//                var Partner = _db.Connection.ExecuteScalar<int>("select count(0) from Partners where PartnerCode='" + request.PartnerId + "'");

//                //var P = await _db.Partners.FirstOrDefaultAsync(x => x.PartnerCode == request.PartnerId);

//                if (Partner <= 0)
//                {
//                    throw new InvalidOperationException("Incorrect partnerId");
//                }

//                //decimal rate = await _db.Connection.ExecuteScalarAsync<decimal>("select Rate from PartnerRates where PartnerCode='" +
//                //    request.PartnerId + "'");

//                var v_rate = _db.Connection.ExecuteScalar("select isnull([Rate],0) as v_rate from [dbo].[MainRates] where [MainRatesSAId]=(select top 1 Id from [dbo].[MainRatesSA] where Productenum=" + (int)request.PolicyType + " and " + request.SumAssured
//                    + " between FromAmt and ToAmt) and [MainTermsId]=" + request.TermId + " and productenum=" + (int)request.PolicyType + "");

//                if (v_rate == null || Double.Parse(v_rate.ToString()) == 0)
//                {
                    
//                    throw new InvalidOperationException("we are unable to retrieve rate please fill in the details below");
//                }
//                double rate = Convert.ToDouble(v_rate);
//                double basicPremium =(double) Math.Round(rate * (request.SumAssured / 1000), 0);
//                string premiumquery = "SELECT [Id],[PolicyFee],[CompensationRate],[DiscountMonthly],[DiscountQuartely],[DiscountSemi],[DsicountAnuall]  FROM [dbo].[MainPremiumSettings] where Productenum=" + (int)request.PolicyType + " and [DefaultSetup]='0'";
//                var premiumsettings = await _db.Connection.QueryFirstOrDefaultAsync<PremiumSettings>(premiumquery);


//                //decimal premium = (decimal)(rate / 100 * request.SumAssured) * ((decimal)request.Loanterm / 12);
//                //decimal premium = (decimal)(rate / 100 * request.SumAssured);

//                var QuoteId = Guid.NewGuid().ToString();

//                var Quote = new FuneralExpenseQuotation
//                {
//                    QuoteId = QuoteId,
//                    PartnerId = request.PartnerId,
//                    AgentId = request.AgentId,
//                    SumAssured = request.SumAssured,
//                    PolicyType = request.PolicyType,
//                    CustomerPhone = request.CustomerPhone,
//                    //CoverPremium = premium,
//                    //TotalPremium = premium,
//                    //CallbackUrl = request.CallbackUrl,
//                    //CompensationLevy = premium,
//                    //PolicyFee = premium,
//                    PolicyTerm = request.PolicyTerm,
//                    DateOfBirth = request.DateOfBirth,
//                    PaymentFrequency = request.PaymentFrequency,
//                    Maturity =(decimal) request.SumAssured,
//                    PTDAccidental =(decimal) request.SumAssured,
//                    PTDNatural =(decimal) request.SumAssured,
//                    NaturalDeath =(decimal) request.SumAssured,
//                    AccidentialDeath =(decimal) request.SumAssured,
//                    CriticalIllness =(decimal) request.SumAssured,
//                };

//                var riders = new List<RiderAmount>();

//                switch (request.PaymentFrequency)
//                {
//                    case Frequency.monthly:
//                        riders = await GetRiderAmount(request, 1);
//                        //response.Riders = riders;
//                        //response.Discount = Math.Round(premiumsettings.DiscountMonthly * basicPremium, 0);
//                        Quote.CoverPremium = basicPremium; // - response.Discount;
//                        Quote.PolicyFee = premiumsettings.PolicyFee;
//                       // Quote.CompensationLevy = Math.Round((premiumsettings.CompensationRate / 100) * (basicPremium + Quote.PolicyFee + (Math.Round(riders.Sum(a => a.Amount), 0)), 0);
//                      // Replace the following incorrect line in the switch-case for Frequency.monthly:
//// Quote.CompensationLevy = Math.Round((premiumsettings.CompensationRate / 100) * (basicPremium + Quote.PolicyFee + (Math.Round(riders.Sum(a => a.Amount), 0)), 0);

//// With the corrected line below:
//Quote.CompensationLevy = Math.Round(
//    (premiumsettings.CompensationRate / 100) * (basicPremium + Quote.PolicyFee + Math.Round(riders.Sum(a => a.Amount), 0)),
//    0
//);
//                        Quote.TotalPremium = Math.Round(Quote.CoverPremium + Quote.PolicyFee + Quote.CompensationLevy, 0) + Math.Round(riders.Sum(a => a.Amount), 0);
//                        //response.Success = true;
//                        //response.Processed = true;
//                        //return response;
//                        break;      
//                    case Frequency.quarterly:
//                        riders = await GetRiderAmount(request, 3);
//                        //response.Riders = riders;
//                        basicPremium = basicPremium * 3;
//                        //response.Discount = Math.Round(premiumsettings.DiscountQuartely / 100 * (basicPremium + Math.Round(riders.Sum(a => a.Amount), 0)), 0);
//                        Quote.CoverPremium = basicPremium; // - response.Discount;
//                        Quote.PolicyFee = premiumsettings.PolicyFee * 3;
//                        Quote.CompensationLevy = Math.Round((premiumsettings.CompensationRate / 100) * (basicPremium - Quote.PolicyFee + Math.Round(riders.Sum(a => a.Amount), 0)), 0);
//                        Quote.TotalPremium = Quote.CoverPremium + Quote.PolicyFee + Quote.CompensationLevy + Math.Round(riders.Sum(a => a.Amount), 0);
//                        //response.Success = true;
//                        //response.Processed = true;
//                        //return response;
//                        break;
//                    case Frequency.halfyearly:
//                        riders = await GetRiderAmount(request, 6);
//                        //response.Riders = riders;
//                        basicPremium = basicPremium * 6;
//                        //response.Discount = Math.Round(premiumsettings.DiscountSemi / 100 * (basicPremium + Math.Round(riders.Sum(a => a.Amount), 0)), 0);
//                        Quote.CoverPremium = basicPremium; // - response.Discount;
//                        Quote.PolicyFee = premiumsettings.PolicyFee * 6;
//                        Quote.CompensationLevy = Math.Round((premiumsettings.CompensationRate / 100) * (basicPremium + Math.Round(riders.Sum(a => a.Amount), 0) - Quote.PolicyFee), 0);
//                        Quote.TotalPremium = Quote.CoverPremium + Quote.PolicyFee + Quote.CompensationLevy + Math.Round(riders.Sum(a => a.Amount), 0);
//                        //response.Success = true;
//                        //response.Processed = true;
//                        //return response;
//                        break;
//                    case Frequency.yearly:
//                        riders = await GetRiderAmount(request, 12);
//                        //response.Riders = riders;
//                        basicPremium = basicPremium * 12;
//                        //response.Discount = Math.Round(premiumsettings.DsicountAnuall / 100 * (basicPremium + Math.Round(riders.Sum(a => a.Amount), 0)), 0);
//                        Quote.CoverPremium = basicPremium; // - response.Discount;
//                        Quote.PolicyFee = premiumsettings.PolicyFee * 12;
//                        Quote.CompensationLevy = Math.Round((premiumsettings.CompensationRate / 100) * (basicPremium + Math.Round(riders.Sum(a => a.Amount), 0) - Quote.PolicyFee), 0);
//                        Quote.TotalPremium = Quote.CoverPremium + Quote.PolicyFee + Quote.CompensationLevy + Math.Round(riders.Sum(a => a.Amount), 0);
//                        //response.Success = true;
//                        //response.Processed = true;
//                        //return response;
//                        break;
//                    default:
//                        riders = await GetRiderAmount(request, 1);
//                        //response.Riders = riders;
//                        //response.Discount = Math.Round(premiumsettings.DiscountMonthly * basicPremium, 0);
//                        Quote.CoverPremium = basicPremium; // - response.Discount;
//                        Quote.PolicyFee = premiumsettings.PolicyFee;
//                        Quote.CompensationLevy = Math.Round((premiumsettings.CompensationRate / 100) * (basicPremium + Quote.PolicyFee + Math.Round(riders.Sum(a => a.Amount), 0)), 0);
//                        Quote.TotalPremium = Math.Round(Quote.CoverPremium + Quote.PolicyFee + Quote.CompensationLevy, 0) + Math.Round(riders.Sum(a => a.Amount), 0);
//                        //response.Success = true;
//                        //response.Processed = true;
//                        //return response;
//                        break;
//                }


                

//                //Save to database

//                _db.FuneralExpenseQuotations.Add(Quote);
//                await _db.SaveChangesAsync();

//                //Return Response
//                var results = new FuneralExpenseQuotationResponseDto
//                {
//                    QuoteId = QuoteId,
//                    SumAssured = request.SumAssured,
//                    Maturity = request.SumAssured,
//                    PTDAccidental = request.SumAssured,
//                    PTDNatural = request.SumAssured,
//                    NaturalDeath = request.SumAssured,
//                    AccidentialDeath = request.SumAssured,
//                    CriticalIllness = request.SumAssured,
//                    CoverPremium = Quote.CoverPremium,
//                    TotalPremium = Quote.TotalPremium,
//                    CompensationLevy = Quote.CompensationLevy,
//                    PolicyFee = Quote.PolicyFee,
//                    Riders = riders,
//                    Message = "Success"
//                };

//               return results;
//            }
//            catch (Exception ex)
//            {

//                _isettings.LogRequests(ex.Message, "GetQuote", RequestType.Error);
//                throw;
//            }
            
//        }
        
        public async Task<List<LastExpenseOptions>> GetOptions()
        {
            try
            {
                string query = "SELECT [OptionId] as OptionId ,[Description] ,[isGroup] as HasGroup FROM [dbo].[LastExpense]" +
                    " where isGroup ='0'";
                var result = await _db.Connection.QueryAsync<LastExpenseOptions>(query);

                return result.Adapt<List<LastExpenseOptions>>();

            }catch(Exception e)
            {
                _isettings.LogRequests(e.Message, "GetOptions", RequestType.Error);
            }
            return null;
        }
     public async Task<RateResponse> CalculateLastExpense(LastExpenseCalcDTO request)
        {
            var ratersponse = new RateResponse();
            try
            {
                int hasParent = 0;
                int childcount = 0;
                double total = 0;
                if (request != null)
                {
                    //hasParent = (int)_db.Connection.ExecuteScalar("select count(1) as hasParent from fadhiliFamilyMembers a,[dbo].[relationShips] b " +
                    //    "where a.relationShip=b.id and a.productId=" + request.productId + " and b.RelationType=" + (int)RelationType.parent + " ");
                    //childcount = (int)_db.Connection.ExecuteScalar("select count(1) as hasParent from fadhiliFamilyMembers a,[dbo].[relationShips] b " +
                    //        "where a.relationShip=b.id and a.productId=" + request.productId + " and b.RelationType=" + (int)RelationType.child + " ");
                   
                    //if (hasParent > 0)
                    //{
                    //    total = (double)await _db.Connection.ExecuteScalarAsync("select ExtendedRate from" +
                    //        " [dbo].[LastExpenseNCBA] where OptionId=" + (int)request.optionType + " and isGroup='"+ request.group +"' ");

                    //}
                    //else
                    //{
                    //    total = (double)await _db.Connection.ExecuteScalarAsync("select MainRate from [dbo].[LastExpenseNCBA] where" +
                    //        " OptionId=" + (int)request.optionType + " and isGroup='"+ request.group +"'");
                    //}

                    //if (childcount > 4)
                    //{
                    //    double childrates = ((double)await _db.Connection.ExecuteScalarAsync("select ChildRate from [dbo].[LastExpenseNCBA] where " +
                    //        "OptionId=" + (int)request.optionType + " and isGroup='"+ request.group +"'") * (childcount - 4));
                    //    total += childrates;
                    //}
                }

            }
            catch (Exception ex)
            {
                _isettings.LogRequests(ex.Message, "CalculateNCBALastExpense", RequestType.Error);
            }
            return ratersponse;

        }
        public  async Task<MainRateRiderResponse> CalculateRates(RateSDTO rateSDTO)
        {


            var response = new MainRateRiderResponse();
            try
            {
                if(rateSDTO.ProductType==Productenum.flex)
                {
                     var v_rate = _db.Connection.ExecuteScalar("select isnull([Rate],0) as v_rate from [dbo].[MainRates] where [MainRatesSAId]=(select top 1 Id from [dbo].[MainRatesSA]" +
                    " where  "+ rateSDTO.Sumassured +""
                    + " between FromAmt and ToAmt) and [MainTermsId]=(select Id from [dbo].[MainTerms] where [Term]=" + rateSDTO.Term + ") and productenum=" + (int)Productenum.flex + "");
                if (v_rate == null || Double.Parse(v_rate.ToString()) == 0)
                {
                    response.Success = true;
                    response.Processed = false;
                    response.Errormsg = " we are unable to retrieve rate please fill in the correct details";
                    return response;
                }
                if (!IsValidPhoneNumber(rateSDTO.Phonenumber))
                {
                     response.Success = true;
                    response.Processed = false;
                    response.Errormsg = " Please enter a valid phone number";
                    return response;
                }
                var agedetails=CalculateAge(rateSDTO.DateOfBirth);
                if (!agedetails.success)
                {
                    response.Success = true;
                    response.Processed = false;
                    response.Errormsg = agedetails.error;
                    return response;
                }
                var leads = new LeadsDTO()
                {
                    PhoneNumber = rateSDTO.Phonenumber,
                    FullName = rateSDTO.Phonenumber,
                    CountryCode = rateSDTO.Phonenumber,
                    CurrentAge = agedetails.age.ToString(),
                    DateOfBirth = rateSDTO.DateOfBirth,
                    SumAssured = rateSDTO.Sumassured.ToString(),
                    Product = rateSDTO.ProductType,
                    OtherDetails = JsonConvert.SerializeObject(rateSDTO),
                    CreatedOn=DateTime.Now
                      
                };
                 await CreateLead(leads);

                double rate = Convert.ToDouble(v_rate);
                var basicPremium = Math.Round(rate * (rateSDTO.Sumassured / 1000), 0);
                string premiumquery = "SELECT [Id],[PolicyFee],[CompensationRate],[DiscountMonthly],[DiscountQuartely],[DiscountSemi],[DsicountAnuall] " +
                    " FROM [dbo].[MainPremiumSettings] where Productenum=" + (int)rateSDTO.ProductType  + " and [DefaultSetup]='0'";
                var premiumsettings = await _db.Connection.QueryFirstOrDefaultAsync<PremiumSettings>(premiumquery);

                switch (rateSDTO.frequency)
                {
                    case Frequency.monthly:
                        var riders = await GetRiderAmount(rateSDTO, 1);
                        response.Riders = riders;
                        response.Discount = Math.Round(premiumsettings.DiscountMonthly * basicPremium, 0);
                        response.CoverPremium = basicPremium - response.Discount;
                        response.PolicyFee = premiumsettings.PolicyFee;
                        response.CompensationLevy = Math.Round((premiumsettings.CompensationRate / 100) * (basicPremium + response.PolicyFee + Math.Round(riders.Sum(a => a.Amount), 0)), 0);
                        response.TotalPremium = Math.Round(response.CoverPremium + response.PolicyFee + response.CompensationLevy, 0) + Math.Round(riders.Sum(a => a.Amount), 0);
                        response.Success = true;
                        response.Processed = true;
                        return response;
                        break;
                    case Frequency.quarterly:
                        riders = await GetRiderAmount(rateSDTO, 3);
                        response.Riders = riders;
                        basicPremium = basicPremium * 3;
                        response.Discount = Math.Round(premiumsettings.DiscountQuartely / 100 * (basicPremium + Math.Round(riders.Sum(a => a.Amount), 0)), 0);
                        response.CoverPremium = basicPremium - response.Discount;
                        response.PolicyFee = premiumsettings.PolicyFee * 3;
                        response.CompensationLevy = Math.Round((premiumsettings.CompensationRate / 100) * (basicPremium - response.Discount + response.PolicyFee + Math.Round(riders.Sum(a => a.Amount), 0)), 0);
                        response.TotalPremium = response.CoverPremium + response.PolicyFee + response.CompensationLevy + Math.Round(riders.Sum(a => a.Amount), 0);
                        response.Success = true;
                        response.Processed = true;
                        return response;
                        break;
                    case Frequency.halfyearly:
                        riders = await GetRiderAmount(rateSDTO, 6);
                        response.Riders = riders;
                        basicPremium = basicPremium * 6;
                        response.Discount = Math.Round(premiumsettings.DiscountSemi / 100 * (basicPremium + Math.Round(riders.Sum(a => a.Amount), 0)), 0);
                        response.CoverPremium = basicPremium - response.Discount;
                        response.PolicyFee = premiumsettings.PolicyFee * 6;
                        response.CompensationLevy = Math.Round((premiumsettings.CompensationRate / 100) * (basicPremium + Math.Round(riders.Sum(a => a.Amount), 0) - response.Discount + response.PolicyFee), 0);
                        response.TotalPremium = response.CoverPremium + response.PolicyFee + response.CompensationLevy + Math.Round(riders.Sum(a => a.Amount), 0);
                        response.Success = true;
                        response.Processed = true;
                        return response;
                        break;
                    case Frequency.yearly:
                        riders = await GetRiderAmount(rateSDTO, 12);
                        response.Riders = riders;
                        basicPremium = basicPremium * 12;
                        response.Discount = Math.Round(premiumsettings.DsicountAnuall / 100 * (basicPremium + Math.Round(riders.Sum(a => a.Amount), 0)), 0);
                        response.CoverPremium = basicPremium - response.Discount;
                        response.PolicyFee = premiumsettings.PolicyFee * 12;
                        response.CompensationLevy = Math.Round((premiumsettings.CompensationRate / 100) * (basicPremium + Math.Round(riders.Sum(a => a.Amount), 0) - response.Discount + response.PolicyFee), 0);
                        response.TotalPremium = response.CoverPremium + response.PolicyFee + response.CompensationLevy + Math.Round(riders.Sum(a => a.Amount), 0);
                        response.Success = true;
                        response.Processed = true;
                        return response;
                        break;
                    default:
                        riders = await GetRiderAmount(rateSDTO, 1);
                        response.Riders = riders;
                        response.Discount = Math.Round(premiumsettings.DiscountMonthly * basicPremium, 0);
                        response.CoverPremium = basicPremium - response.Discount;
                        response.PolicyFee = premiumsettings.PolicyFee;
                        response.CompensationLevy = Math.Round((premiumsettings.CompensationRate / 100) * (basicPremium + response.PolicyFee + Math.Round(riders.Sum(a => a.Amount), 0)), 0);
                        response.TotalPremium = Math.Round(response.CoverPremium + response.PolicyFee + response.CompensationLevy, 0) + Math.Round(riders.Sum(a => a.Amount), 0);
                        response.Success = true;
                        response.Processed = true;
                        return response;
                        break;
                }
                }
               

            }
            catch (Exception ex)
            {
                _isettings.LogRequests(ex.Message, "CalculateRates",RequestType.Error);

            }
            response.Success = true;
            return response;
        }

          public async Task<List<RiderAmount>> GetRiderAmount(RateSDTO rate, int frequency)
        {
            var response = new List<RiderAmount>();
            try
            {
                if (rate.Riders != null && rate.Riders.Count > 0)
                {
                    for (int i = 0; i < rate.Riders.Count; i++)

                    {
                        double sumassurred = 0;
                        double maturityBenefit = 0;
                        var Age = CalculateAge(rate.DateOfBirth);
                        string getdatequery = "SELECT   [Rate]  FROM [dbo].[MainRidersRates] where productenum=" + (int)Productenum.flex + " and AgeBandId=(select Id from AgeBand" +
                           " where " + Age.age + " between MinAge and MaxAge) and   RiderTYPE=" + (int)rate.Riders[i] + " and MainTermsId=(select Id from [dbo].[MainTerms] where [Term]=" + rate.Term + ")  ";
                        if (rate.Riders[i] == RiderType.waiverretirement)
                        {

                            getdatequery = "SELECT   [Rate]  FROM [dbo].[MainRidersRates] where productenum=" + (int)Productenum.flex + " " +
                                " and  RiderTYPE=" + (int)rate.Riders[i] + " and MainTermsId=(select Id from [dbo].[MainTerms] where [Term]=" + rate.Term + ")  ";
                        }

                        var scalarResult = await _db.Connection.ExecuteScalarAsync(getdatequery);
                        var ratevalue = scalarResult != null ? Convert.ToDouble(scalarResult) : 0;

                        double rideramount = 0;

                        if (rate.Riders[i] != RiderType.death)
                        {
                            rideramount = Math.Round(ratevalue * rate.Sumassured / 1000 * frequency, 0);
                            if (rate.Riders[i] == RiderType.criticalillness)
                            {
                                rideramount = Math.Round(ratevalue * 0.5 * rate.Sumassured / 1000 * frequency, 0);
                            }
                        }
                        switch (rate.Riders[i])
                        {
                            case RiderType.death:
                                if (rate.DeathBenefits > 0)
                                {

                                    //string getdeathquery = "SELECT   [Rate]  FROM [dbo].[MainRidersRates] where productenum=" + (int)Productenum.ncba + " and AgeBandId=(select Id from AgeBand" +
                                    //      " where " + rate.Age + " between MinAge and MaxAge) and   RiderTYPE=" + (int)RiderType.death + " and MainTermsId=" + (int)rate.TermId + " ";

                                    //var deatyhratevalue = (double)await _db.Connection.ExecuteScalarAsync(getdeathquery);

                                    string deathquery = "select  Rate from [dbo].[DeathBenefits] where Alias='" + rate.DeathBenefits + "';";
                                    var deathrate = (double)await _db.Connection.ExecuteScalarAsync(deathquery);
                                    ratevalue = ratevalue * deathrate;
                                    rideramount = Math.Round(ratevalue * rate.Sumassured / 1000 * frequency, 0);
                                    sumassurred = Math.Round(deathrate * rate.Sumassured,0);
                                maturityBenefit = 0;
                                    //for(int j =0; j < rate.MaturityNumber; j++)
                                    //{
                                    //    //maturityBenefit += Math.Round(rate.Sumassured/rate.MaturityNumber *(1+);
                                    //}
                                }
                                
                                break;
                            case RiderType.criticalillness:
                                sumassurred = Math.Round(.5 * rate.Sumassured, 0);
                                break;
                            case RiderType.disability:
                                sumassurred = rate.Sumassured; break;

                        }




                        response.Add(new RiderAmount()
                        {
                            Amount = rideramount,
                            Rider = rate.Riders[i].ToString(),
                            SumAssured = sumassurred,
                            MaturityBenefit = maturityBenefit,
                        });


                    }
                }
                //if (rate.DeathBenefits > 0)
                //{

                //    string getdatequery = "SELECT   [Rate]  FROM [dbo].[MainRidersRates] where productenum=" + (int)Productenum.ncba + " and AgeBandId=(select Id from AgeBand" +
                //          " where " + rate.Age + " between MinAge and MaxAge) and   RiderTYPE=" + (int)RiderType.death + " and MainTermsId=" + (int)rate.TermId + " ";

                //    var ratevalue = (double)await _db.Connection.ExecuteScalarAsync(getdatequery);

                //    string deathquery = "select  Rate from [dbo].[DeathBenefits] where id='" + rate.DeathBenefits + "';";
                //    var deathrate = (double)await _db.Connection.ExecuteScalarAsync(deathquery);
                //    ratevalue = ratevalue * deathrate;
                //    response.Add(new RiderAmount()
                //    {
                //        Amount = Math.Round(ratevalue * rate.Sumassured / 1000 * frequency, 0),
                //        Rider = "death"
                //    });
                //}

            }
            catch (Exception ex) {

                _isettings.LogRequests(ex.Message, "GetRiderAmount", RequestType.Error);
            }
            return response;
        }


    }
}

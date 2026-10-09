using API.Infrastructure.Interface;
using Azure;
using DAL;
using DAL.Model;
using DAL.Model.FuneralExpense;
using DAL.Model.LastExpense;
using DAL.ModelView;
using DAL.ModelView.Flex;
using DAL.ModelView.FuneralExpense;
using Dapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace API.Infrastructure.Application.FuneralExpense;

public class FuneralExpenseManager : IFuneralExpense
{
    private readonly ApplicationDbContext _db;
    readonly MainDbContext _mainDb;
    readonly Isettings _isettings;

    public FuneralExpenseManager(ApplicationDbContext db, Isettings isettings, MainDbContext mainDb)
    {
        _db = db;
        _isettings = isettings;
        _mainDb = mainDb;
    }
    Task<List<ProductDTO>> IFuneralExpense.GetProducts(string partnerCode)
    {
        throw new NotImplementedException();
    }

    public async Task<FuneralExpenseQuotationResponseDto> GetQuote(FuneralExpenseQuotationDto request)
    {
        try
        {
           
            try
            {
                if (request.PolicyTerm <= 0)
                {
                    throw new InvalidOperationException("Invalid Policy Term");
                }
                if (string.IsNullOrEmpty(request.PartnerId.ToString()))
                {
                    throw new InvalidOperationException("Invalid partnerId");
                  
                }
                if (string.IsNullOrEmpty(request.AgentId.ToString()))
                {
                    throw new InvalidOperationException("Invalid AgentId");

                }

                var Partner = _db.Connection.ExecuteScalar<int>("select count(0) from Partners where PartnerCode='" + request.PartnerId + "'");

                //var P = await _db.Partners.FirstOrDefaultAsync(x => x.PartnerCode == request.PartnerId);

                if (Partner <= 0)
                {
                    throw new InvalidOperationException("Incorrect partnerId");
                }

                //decimal rate = await _db.Connection.ExecuteScalarAsync<decimal>("select Rate from PartnerRates where PartnerCode='" +
                //    request.PartnerId + "'");

                var v_rate = _db.Connection.ExecuteScalar("select isnull([Rate],0) as v_rate from [dbo].[MainRates] where [MainRatesSAId]=(select top 1 Id from [dbo].[MainRatesSA] where Productenum=" + (int)request.PolicyType + " and " + request.SumAssured
                    + " between FromAmt and ToAmt) and [MainTermsId]=" + request.TermId + " and productenum=" + (int)request.PolicyType + "");

                if (v_rate == null || Double.Parse(v_rate.ToString()) == 0)
                {
                    
                    throw new InvalidOperationException("we are unable to retrieve rate please fill in the details below");
                }
                double rate = Convert.ToDouble(v_rate);
                double basicPremium =(double) Math.Round(rate * (request.SumAssured / 1000), 0);
                string premiumquery = "SELECT [Id],[PolicyFee],[CompensationRate],[DiscountMonthly],[DiscountQuartely],[DiscountSemi],[DsicountAnuall]  FROM [dbo].[MainPremiumSettings] where Productenum=" + (int)request.PolicyType + " and [DefaultSetup]='0'";
                var premiumsettings = await _db.Connection.QueryFirstOrDefaultAsync<PremiumSettings>(premiumquery);


                //decimal premium = (decimal)(rate / 100 * request.SumAssured) * ((decimal)request.Loanterm / 12);
                //decimal premium = (decimal)(rate / 100 * request.SumAssured);

                var QuoteId = Guid.NewGuid().ToString();

                var Quote = new FuneralExpenseQuotation
                {
                    QuoteId = QuoteId,
                    PartnerId = request.PartnerId,
                    AgentId = request.AgentId,
                    SumAssured = request.SumAssured,
                    PolicyType = request.PolicyType,
                    CustomerPhone = request.CustomerPhone,
                    //CoverPremium = premium,
                    //TotalPremium = premium,
                    //CallbackUrl = request.CallbackUrl,
                    //CompensationLevy = premium,
                    //PolicyFee = premium,
                    PolicyTerm = request.PolicyTerm,
                    DateOfBirth = request.DateOfBirth,
                    PaymentFrequency = request.PaymentFrequency,
                    Maturity =(decimal) request.SumAssured,
                    PTDAccidental =(decimal) request.SumAssured,
                    PTDNatural =(decimal) request.SumAssured,
                    NaturalDeath =(decimal) request.SumAssured,
                    AccidentialDeath =(decimal) request.SumAssured,
                    CriticalIllness =(decimal) request.SumAssured,
                };

                var riders = new List<RiderAmount>();

                switch (request.PaymentFrequency)
                {
                    case Frequency.monthly:
                        riders = await GetRiderAmount(request, 1);
                        //response.Riders = riders;
                        //response.Discount = Math.Round(premiumsettings.DiscountMonthly * basicPremium, 0);
                        Quote.CoverPremium = basicPremium; // - response.Discount;
                        Quote.PolicyFee = premiumsettings.PolicyFee;
                       // Quote.CompensationLevy = Math.Round((premiumsettings.CompensationRate / 100) * (basicPremium + Quote.PolicyFee + (Math.Round(riders.Sum(a => a.Amount), 0)), 0);
                      // Replace the following incorrect line in the switch-case for Frequency.monthly:
// Quote.CompensationLevy = Math.Round((premiumsettings.CompensationRate / 100) * (basicPremium + Quote.PolicyFee + (Math.Round(riders.Sum(a => a.Amount), 0)), 0);

// With the corrected line below:
Quote.CompensationLevy = Math.Round(
    (premiumsettings.CompensationRate / 100) * (basicPremium + Quote.PolicyFee + Math.Round(riders.Sum(a => a.Amount), 0)),
    0
);
                        Quote.TotalPremium = Math.Round(Quote.CoverPremium + Quote.PolicyFee + Quote.CompensationLevy, 0) + Math.Round(riders.Sum(a => a.Amount), 0);
                        //response.Success = true;
                        //response.Processed = true;
                        //return response;
                        break;      
                    case Frequency.quarterly:
                        riders = await GetRiderAmount(request, 3);
                        //response.Riders = riders;
                        basicPremium = basicPremium * 3;
                        //response.Discount = Math.Round(premiumsettings.DiscountQuartely / 100 * (basicPremium + Math.Round(riders.Sum(a => a.Amount), 0)), 0);
                        Quote.CoverPremium = basicPremium; // - response.Discount;
                        Quote.PolicyFee = premiumsettings.PolicyFee * 3;
                        Quote.CompensationLevy = Math.Round((premiumsettings.CompensationRate / 100) * (basicPremium - Quote.PolicyFee + Math.Round(riders.Sum(a => a.Amount), 0)), 0);
                        Quote.TotalPremium = Quote.CoverPremium + Quote.PolicyFee + Quote.CompensationLevy + Math.Round(riders.Sum(a => a.Amount), 0);
                        //response.Success = true;
                        //response.Processed = true;
                        //return response;
                        break;
                    case Frequency.halfyearly:
                        riders = await GetRiderAmount(request, 6);
                        //response.Riders = riders;
                        basicPremium = basicPremium * 6;
                        //response.Discount = Math.Round(premiumsettings.DiscountSemi / 100 * (basicPremium + Math.Round(riders.Sum(a => a.Amount), 0)), 0);
                        Quote.CoverPremium = basicPremium; // - response.Discount;
                        Quote.PolicyFee = premiumsettings.PolicyFee * 6;
                        Quote.CompensationLevy = Math.Round((premiumsettings.CompensationRate / 100) * (basicPremium + Math.Round(riders.Sum(a => a.Amount), 0) - Quote.PolicyFee), 0);
                        Quote.TotalPremium = Quote.CoverPremium + Quote.PolicyFee + Quote.CompensationLevy + Math.Round(riders.Sum(a => a.Amount), 0);
                        //response.Success = true;
                        //response.Processed = true;
                        //return response;
                        break;
                    case Frequency.yearly:
                        riders = await GetRiderAmount(request, 12);
                        //response.Riders = riders;
                        basicPremium = basicPremium * 12;
                        //response.Discount = Math.Round(premiumsettings.DsicountAnuall / 100 * (basicPremium + Math.Round(riders.Sum(a => a.Amount), 0)), 0);
                        Quote.CoverPremium = basicPremium; // - response.Discount;
                        Quote.PolicyFee = premiumsettings.PolicyFee * 12;
                        Quote.CompensationLevy = Math.Round((premiumsettings.CompensationRate / 100) * (basicPremium + Math.Round(riders.Sum(a => a.Amount), 0) - Quote.PolicyFee), 0);
                        Quote.TotalPremium = Quote.CoverPremium + Quote.PolicyFee + Quote.CompensationLevy + Math.Round(riders.Sum(a => a.Amount), 0);
                        //response.Success = true;
                        //response.Processed = true;
                        //return response;
                        break;
                    default:
                        riders = await GetRiderAmount(request, 1);
                        //response.Riders = riders;
                        //response.Discount = Math.Round(premiumsettings.DiscountMonthly * basicPremium, 0);
                        Quote.CoverPremium = basicPremium; // - response.Discount;
                        Quote.PolicyFee = premiumsettings.PolicyFee;
                        Quote.CompensationLevy = Math.Round((premiumsettings.CompensationRate / 100) * (basicPremium + Quote.PolicyFee + Math.Round(riders.Sum(a => a.Amount), 0)), 0);
                        Quote.TotalPremium = Math.Round(Quote.CoverPremium + Quote.PolicyFee + Quote.CompensationLevy, 0) + Math.Round(riders.Sum(a => a.Amount), 0);
                        //response.Success = true;
                        //response.Processed = true;
                        //return response;
                        break;
                }


                

                //Save to database

                _db.FuneralExpenseQuotations.Add(Quote);
                await _db.SaveChangesAsync();

                //Return Response
                var results = new FuneralExpenseQuotationResponseDto
                {
                    QuoteId = QuoteId,
                    SumAssured = request.SumAssured,
                    Maturity = request.SumAssured,
                    PTDAccidental = request.SumAssured,
                    PTDNatural = request.SumAssured,
                    NaturalDeath = request.SumAssured,
                    AccidentialDeath = request.SumAssured,
                    CriticalIllness = request.SumAssured,
                    CoverPremium = Quote.CoverPremium,
                    TotalPremium = Quote.TotalPremium,
                    CompensationLevy = Quote.CompensationLevy,
                    PolicyFee = Quote.PolicyFee,
                    Riders = riders,
                    Message = "Success"
                };

               return results;
            }
            catch (Exception ex)
            {

                _isettings.LogRequests(ex.Message, "GetQuote", RequestType.Error);
                throw;
            }
            
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<FuneralExpenseOnboardingResponseDto> OnboardingRequest(FuneralExpenseOnboardingDto onboardingDto)
    {
        try
        {
            string onboardingId = Guid.NewGuid().ToString();
            var query = """
                    SELECT Id AS partnerId FROM  Partners 
                    WHERE PartnerCode=@PartnerCode
                    """;
            string partnerId = _db.Connection.ExecuteScalar<string>(query, new { PartnerCode = onboardingDto.ProductId });
            if (partnerId == null)
            {
                throw new InvalidOperationException("Partner does not exist");
               
            }

            // Last Expense product enum check


            string prod = """
                                 SELECT b.Id AS partnerProductId 
                                 FROM partnersProducts a, [dbo].[Products] b where a.ProductId=b.Id 
                                 and a.PartnerId=@PartnerId and b.Productenum=@Productenum
                                 """;
            int partnerProductId = _db.Connection.ExecuteScalar<int>(prod, new { PartnerId = partnerId, Productenum = (int)Productenum.lastexpense });
            if (partnerProductId <= 0)
            {
                throw new InvalidOperationException("Partner Product does not exist");
            }

           
            //Get the Quote
            var Quote = await _db.FuneralExpenseQuotations.FindAsync(onboardingDto.QuoteId);
            if (Quote == null)
            {
                throw new InvalidOperationException("Invalid QuoteId");
            }

            var NewOnboarding = new FuneralExpenseOnboarding
            {
                MemberId = onboardingId,
                QuoteId = onboardingDto.QuoteId,
                PolicyStatus = PolicyStatus.Pending,
                FullName = onboardingDto.FullName,
                IdNumber = onboardingDto.IdNumber,
                Gender = onboardingDto.Gender,
                Nationality = onboardingDto.Nationality,
                Residency = onboardingDto.Residency,
                Email = onboardingDto.Email,
                Occupation = onboardingDto.Occupation,
                MonthlyIncomeRange = onboardingDto?.MonthlyIncomeRange,
                ProductId = onboardingDto?.ProductId,
                PartnerId = onboardingDto?.PartnerId,
                AgentId = onboardingDto?.AgentId,
                CallbackUrl = onboardingDto?.CallbackUrl,

            };

            _db.FuneralExpenseOnboardings.Add(NewOnboarding);

            await _db.SaveChangesAsync();

            //response
            var response = new FuneralExpenseOnboardingResponseDto
            {
                MemberId = NewOnboarding.MemberId,
                QuoteId = NewOnboarding.QuoteId,
                SumAssured = (decimal)Quote.SumAssured,
                Message = "Success"
            };

            return response;

        }
        catch(Exception ex)
        {
            throw;
        }
    }

    public async Task<MemberHealthResponseDto> ProcessMedical(MemberHealthDto memberHealthDto)
    {
        try
        {
            //Check member exists
            var Member = await _db.FuneralExpenseOnboardings.FindAsync(memberHealthDto.MemberId);
            if (Member == null)
            {
                throw new InvalidOperationException("Member Not Found!!!");
            }

            var Quote = await _db.FuneralExpenseQuotations.FindAsync(memberHealthDto.QuoteId);
            
            if (Quote == null)
            {
                throw new InvalidOperationException("Quote Not Found!!!");
            }

            var RecordId = Guid.NewGuid().ToString();

            var Exists = await _db.MemberHealths
                .Where(x => x.MemberId == memberHealthDto.MemberId &&
                x.QuoteId == memberHealthDto.QuoteId).FirstOrDefaultAsync();

            if (Exists != null)
            {
                Exists.MemberId = memberHealthDto.MemberId;
                Exists.QuoteId = memberHealthDto.QuoteId;
                Exists.Height = memberHealthDto.Height;
                Exists.Weight = memberHealthDto.Weight;
                Exists.PriorDeclinedInsurance = memberHealthDto.PriorDeclinedInsurance;
                Exists.ExistingConditions = memberHealthDto.ExistingConditions;
                Exists.DrugOrAlcoholAbuse = memberHealthDto.DrugOrAlcoholAbuse;
                Exists.Respiratory = memberHealthDto.Respiratory;
                Exists.HeartOrCirculation = memberHealthDto.HeartOrCirculation;
                Exists.ChronicConditions = memberHealthDto.ChronicConditions;
                Exists.Wellness = memberHealthDto.Wellness;
                Exists.ImmuneOrViral = memberHealthDto.ImmuneOrViral;
                Exists.Senses = memberHealthDto.Senses;
            }
            else
            {
                var Record = new MemberHealth
                {
                    Id = RecordId,
                    MemberId = memberHealthDto.MemberId,
                    QuoteId = memberHealthDto.QuoteId,
                    Height = memberHealthDto.Height,
                    Weight = memberHealthDto.Weight,
                    PriorDeclinedInsurance = memberHealthDto.PriorDeclinedInsurance,
                    ExistingConditions = memberHealthDto.ExistingConditions,
                    DrugOrAlcoholAbuse = memberHealthDto.DrugOrAlcoholAbuse,
                    Respiratory = memberHealthDto.Respiratory,
                    HeartOrCirculation = memberHealthDto.HeartOrCirculation,
                    ChronicConditions = memberHealthDto.ChronicConditions,
                    Wellness = memberHealthDto.Wellness,
                    ImmuneOrViral = memberHealthDto.ImmuneOrViral,
                    Senses = memberHealthDto.Senses
                };

                _db.MemberHealths.Add(Record);

            }


            await _db.SaveChangesAsync();

            var response = new MemberHealthResponseDto
            {
                TransactionId = RecordId,
                QuoteId = memberHealthDto.QuoteId,
                Message = "Success"
            };

            return response;

        }
        catch(Exception ex)
        {
            throw;
        }
    }

    public async Task<PaymentContributionResponseDto> Contribution(PaymentContributionDto paymentContributionDto)
    {
        try
        {
            //Check if member exists
            var Member = await _db.FuneralExpenseOnboardings.FindAsync(paymentContributionDto.MemberId);

            if (Member == null)
            {
                throw new InvalidOperationException("Invalid MemberId");
            }

            string prod = """
                                 SELECT b.Id AS partnerProductId 
                                 FROM partnersProducts a, [dbo].[Products] b where a.ProductId=b.Id 
                                 and a.PartnerId=@PartnerId and b.Productenum=@Productenum
                                 """;
            int partnerProductId = _db.Connection.ExecuteScalar<int>(prod, new { PartnerId = paymentContributionDto.ProductId, Productenum = (int)Productenum.lastexpense });
            if (partnerProductId <= 0)
            {
                throw new InvalidOperationException("Partner Product does not exist");
            }

            //Get Quote
            var Quote = await _db.FuneralExpenseOnboardings
                .Where(x => x.MemberId == Member.MemberId && x.QuoteId == Member.QuoteId)
                .FirstOrDefaultAsync();

            if (Quote == null)
            {
                throw new InvalidOperationException("Quote Not Found");
            }

            var RecordId = Guid.NewGuid().ToString();

            var NewPayments = new List<MemberPayment>();
            foreach (var p in paymentContributionDto.Payments)
            {
                var PaymentId = Guid.NewGuid().ToString();

                var payment = new MemberPayment
                {
                    Id = PaymentId,
                    PaymentMode = p.PaymentMode,
                    PaymentReference = p.PaymentReference,
                    Amount = p.Amount,
                    MpesaNumber = p.MpesaNumber,
                };

                NewPayments.Add(payment);
            }

            var contribution = new PaymentContribution
            {
                Id = RecordId,
                MemberId = paymentContributionDto.MemberId,
                ProductId = paymentContributionDto.ProductId,
                CallbackUrl = paymentContributionDto.CallbackUrl,
                Payments = NewPayments
            };

            _db.PaymentContributions.Add(contribution);

            // Quote
            Quote.PolicyStatus = PolicyStatus.PaidUp;

            await _db.SaveChangesAsync();

            var response = new PaymentContributionResponseDto
            {
                TransactioId = RecordId,
                Message = "Successful"
            };

            return response;

        }
        catch (Exception ex)
        {
            throw;
        }
    }

   

    public async Task<PolicyActivationResponseDto> Activation(PolicyActivationDto policyActivationDto)
    {
        try
        {
            //Get Member
            var Member = await _db.FuneralExpenseOnboardings.FindAsync(policyActivationDto.MemberId);

            if(Member == null || Member.PolicyStatus == PolicyStatus.Active)
            {
                throw new InvalidOperationException("Member Not Found Or Already Activated");
            }

            //Get MemberOTP
            var Otp = await _db.OTPOnboardings.
                OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(x => x.MemberId == policyActivationDto.MemberId && x.IsUsed != true);

            if(Otp == null || Otp.IsUsed == true)
            {
                throw new InvalidOperationException("Invalid OTP");
            }

            if (DateTime.Now > Otp.ExpiredAt)
            {
                throw new InvalidOperationException("OTP Expired");
            }

            Member.SignatureBase64 = policyActivationDto.SignatureBase64;
            Member.PolicyStatus = PolicyStatus.Active;

            var RecordId = Guid.NewGuid().ToString();

            var NewActivation = new PolicyActivation
            {
                Id = RecordId,
                MemberId = policyActivationDto.MemberId,
                SignatureBase64 = policyActivationDto.SignatureBase64
            };

            _db.PolicyActivations.Add(NewActivation);

            Otp.IsUsed = true;
            Otp.UsedAt = DateTime.Now;
            Otp.ErrorMessage = "Used";

            await _db.SaveChangesAsync();

            var response = new PolicyActivationResponseDto
            {
                MemberId = Member.MemberId,
                Otp = "",
                Message = "Successful"
            };

            

            return response;

        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<PolicyResponseDto> Status(PolicyStatusDto policyStatusDto)
    {
        try
        {
            var Onboarding = await _db.FuneralExpenseOnboardings
                .Where(x => x.ProductId == policyStatusDto.ProductId &&
                x.MemberId == policyStatusDto.MemberId &&
                x.PartnerId == policyStatusDto.PartnerId)
                .FirstOrDefaultAsync();

            if(Onboarding == null)
            {
                throw new InvalidOperationException("Policy not Found");
            }

            var response = new PolicyResponseDto
            {
                MemberId = Onboarding.MemberId,
                PolicyStatus = Onboarding.PolicyStatus,
                Message = "Successful"
            };

            return response;
        }
        catch(Exception ex)
        {
            throw;
        }
    }

    public async Task<List<Terms>> GetMainTerms()
    {
        var response = new List<Terms>();
        try
        {
            string termsqueries = "SELECT [Id],[Term]  FROM [dbo].[MainTerms] where [Productenum] =" + (int)Productenum.lastexpense + "";
            var results = await _db.Connection.QueryAsync<Terms>(termsqueries);
            response = results.ToList();

        }
        catch (Exception ex)
        {

        }
        return response;
    }
    public async Task<List<RiderAmount>> GetRiderAmount(FuneralExpenseQuotationDto rate, int frequency)
    {
        var response = new List<RiderAmount>();
        try
        {
            if (rate.Riders != null && rate.Riders.Count > 0)
            {
                for (int i = 0; i < rate.Riders.Count; i++)

                {
                    decimal sumassurred = 0;
                    decimal maturityBenefit = 0;
                    string getdatequery = "SELECT   [Rate]  FROM [dbo].[MainRidersRates] where productenum=" + (int)Productenum.lastexpense + " and AgeBandId=(select Id from AgeBand" +
                       " where " + rate.Age + " between MinAge and MaxAge) and   RiderTYPE=" + (int)rate.Riders[i] + " and MainTermsId=" + (int)rate.TermId + " ";
                    if (rate.Riders[i] == RiderType.waiverretirement)
                    {

                        getdatequery = "SELECT   [Rate]  FROM [dbo].[MainRidersRates] where productenum=" + (int)Productenum.lastexpense + " " +
                            " and  RiderTYPE=" + (int)rate.Riders[i] + " and MainTermsId=" + (int)rate.TermId + " ";
                    }

                    var ratevalue = (decimal)await _db.Connection.ExecuteScalarAsync(getdatequery);

                    decimal rideramount = 0;

                    if (rate.Riders[i] != RiderType.death)
                    {
                        rideramount = Math.Round(ratevalue * (decimal) rate.SumAssured / 1000 * frequency, 0);
                        if (rate.Riders[i] == RiderType.criticalillness)
                        {
                            rideramount = Math.Round(ratevalue * 0.5m * (decimal)rate.SumAssured / 1000 * frequency, 0);
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

                                string deathquery = "select  Rate from [dbo].[DeathBenefits] where id='" + rate.DeathBenefits + "';";
                                var deathrate = (decimal)await _db.Connection.ExecuteScalarAsync(deathquery);
                                ratevalue = ratevalue * deathrate;
                                rideramount = Math.Round(ratevalue * (decimal)rate.SumAssured / 1000 * frequency, 0);
                                sumassurred = Math.Round(deathrate * (decimal)rate.SumAssured, 0);
                                maturityBenefit = 0;
                                //for (int j = 0; j < rate.MaturityNumber; j++)
                                //{
                                //    //maturityBenefit += Math.Round(rate.Sumassured/rate.MaturityNumber *(1+);
                                //}
                            }

                            break;
                        case RiderType.criticalillness:
                            sumassurred = Math.Round(0.5m * (decimal)rate.SumAssured, 0);
                            break;
                        case RiderType.disability:
                            sumassurred = (decimal)rate.SumAssured; break;

                    }




                    response.Add(new RiderAmount()
                    {
                        Amount = (double) rideramount,
                        Rider = rate.Riders[i].ToString(),
                        SumAssured =(double) sumassurred,
                        MaturityBenefit =(double) maturityBenefit,
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
        catch (Exception ex)
        {

            //_settings.LogRequest(ex.Message, "GetRiderAmount", RequestType.Error, Trn_Log_Type.Agent);
        }
        return response;
    }

    public async Task<string> SendOTP(string memberId)
    {
        try
        {
            //Get Member
            var Member = await _db.FuneralExpenseOnboardings.FindAsync(memberId);

            if(Member == null)
            {
                throw new InvalidOperationException("Member not Found");
            }

            //Get Quote
            var Quote = await _db.FuneralExpenseQuotations.FindAsync(Member.QuoteId);

            if (Member == null)
            {
                throw new InvalidOperationException("Quote not Found");
            }

            int number = RandomNumberGenerator.GetInt32(10000, 100000);

            var otp = new OTPOnboarding
            {
                Id = Guid.NewGuid().ToString(),
                Code = number.ToString(),
                Message = number.ToString(),
                MemberId = Member.MemberId,
                CustomerPhoneNumber = Quote.CustomerPhone,
                IsUsed = false,
                ISsent = false,
                CreatedAt = DateTime.Now,
                SentAt = null,
                UsedAt = null,
                EmailPlaceHolder = null,
                ErrorMessage = null,
                ExpiredAt = DateTime.Now.AddMinutes(5)
            };

            _db.OTPOnboardings.Add(otp);

            await _db.SaveChangesAsync();


            return "Successful";

        }
        catch(Exception ex)
        {
            throw;
        }
    }
}

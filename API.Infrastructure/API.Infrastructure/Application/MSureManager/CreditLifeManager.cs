using API.Infrastructure.Interface;
using DAL;
using DAL.Model;
using DAL.ModelView;
using DAL.ModelView.CreditLife;
using Dapper;
using Mapster;
using Microsoft.IdentityModel.Tokens;

namespace API.Infrastructure.Application.MSureManager
{
    public class CreditLifeManager : ICreditLife
    {
        private readonly ApplicationDbContext _db;
        readonly MainDbContext _mainDb;
        readonly Isettings _isettings;
        public CreditLifeManager(ApplicationDbContext db, Isettings isettings,MainDbContext mainDb)
        {
            _db = db;
            _isettings = isettings;
            _mainDb = mainDb;
        }
        public string GenerateTrnNo(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public async Task<ResponseDTO> OnboardingRequest(CreditLifeDTO onboardingDto)
        {
            var responseDTO = new ResponseDTO();

            try
            {
                string TransactionId;
                do
                {
                    TransactionId = GenerateTrnNo(8);
                    var existingTransactionId = await _db.Connection.ExecuteScalarAsync<string>(
                        "SELECT TransactionId FROM CreditLifeRequests WHERE TransactionId = @TransactionId", 
                        new { TransactionId }
                    );
                    if (existingTransactionId == null)
                    {
                        break;
                    }
                } while (true);
              

                string onboardingId = Guid.NewGuid().ToString();
                var query = """
                    SELECT Id AS partnerId FROM  Partners 
                    WHERE PartnerCode=@PartnerCode
                    """;
                string partnerId = _db.Connection.ExecuteScalar<string>(query, new { PartnerCode = onboardingDto.PartnerCode });
                if (partnerId ==null)
                {
                    responseDTO.ErrorMsg = "Partner does not exist";
                    responseDTO.Success = false;
                    return responseDTO;
                }
                // Credit Life product enum check
               

                string prod = """
                                 SELECT b.Id AS partnerProductId 
                                 FROM partnersProducts a, [dbo].[Products] b where a.ProductId=b.Id 
                                 and a.PartnerId=@PartnerId and b.Productenum=@Productenum
                                 """;
                int partnerProductId = _db.Connection.ExecuteScalar<int>(prod, new { PartnerId = partnerId,Productenum=(int)Productenum.creditlife });
                if (partnerProductId <= 0)
                {
                    responseDTO.ErrorMsg = "Partner Product does not exist";
                    responseDTO.Success = false;
                    return responseDTO;
                }

                //if (!string.IsNullOrEmpty(onboardingDto.TransactionId))
                //{
                //    var reg = """
                //    SELECT RegNumber FROM Customers 
                //    WHERE RegNumber=@RegNumber and PartnerId=@PartnerId
                //    """;
                //var RegNumber = _db.Connection.ExecuteScalar<string>(reg, new { RegNumber=onboardingDto.TransactionId ,PartnerId=partnerId});
                //if (!string.IsNullOrEmpty(RegNumber))
                //{
                //    responseDTO.ErrorMsg = "Registration Number already exists";
                //    responseDTO.Success = false;
                //    return responseDTO;
                //}
                //}
                

                //var idNo = """
                //    SELECT IDNumber FROM OnboardingRequests 
                //    WHERE IDNumber=@IDNumber
                //    """;
                //var IDNumber = _db.Connection.ExecuteScalar<string>(idNo, new { onboardingDto.IDNumber });

                //if (!string.IsNullOrEmpty(IDNumber))
                //{
                //    responseDTO.ErrorMsg = "ID Number already exists";
                //    responseDTO.Success = false;
                //    return responseDTO;
                //}

                Gender? gender = null;
                var genderStr = onboardingDto.Gender?.ToString()?.Trim().ToLower();
                if (!string.IsNullOrEmpty(genderStr))
                {
                    switch (genderStr)
                    {
                        case "f":
                        case "female":
                            gender = Gender.female;
                            break;
                        case "m":
                        case "male":
                            gender = Gender.male;
                            break;
                        case "other":
                            gender = Gender.other;
                            break;
                        default:
                            gender = null;
                            break;
                    }
                }
                var request = new Customers()
                {
                     PartnerId = partnerId,
                  
                    Id = onboardingId.ToString(),
                    ProductId = partnerProductId,
                    Firstname=onboardingDto.CustomerName.Split(' ')[0],
                    OtherNames = onboardingDto.CustomerName,
                    DateOfBirth = onboardingDto.DateOfBirth,
                    IDNumber = onboardingDto.IDNumber,
                    Gender = gender,
                    Email= onboardingDto.EmailAddress??"",
                     Nationality = "Kenyan",
                      Occupation =  "",
                    Residency = "",
                     RequestDate =  DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    PhoneNumber = onboardingDto.PhoneNumber,   
                     CreatedOn=DateTime.Now,
                    Processed = false,
                    Status = "success"
                };
                _db.customers.Add(request);
                await _db.SaveChangesAsync();

                string firstName = "";
                string otherNames = "";
                if(onboardingDto.CustomerName.Contains(" "))
                {
                    var names = onboardingDto.CustomerName.Split(' ');
                    firstName = names[0];
                    otherNames = string.Join(" ", names.Skip(1));
                }
                else
                {
                    firstName = onboardingDto.CustomerName;
                    otherNames = "";
                }
                var customerRecords = new OnboardingDTO()
                { DateOfBirth = onboardingDto.DateOfBirth, Email= onboardingDto.EmailAddress, FirstName=firstName,
                    Gender=gender, IDNumber=onboardingDto.PhoneNumber, Nationality = "Kenyan",
                    Occupation = "", OtherNames = otherNames,
                    PartnerCode = onboardingDto.PartnerCode,  PhoneNumber = onboardingDto.PhoneNumber, RequestDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Residency = "",

                };
                AddMainRecord(customerRecords, onboardingId, partnerId.ToString(),
                    TransactionId, partnerProductId);


                string addCreditRequest = "INSERT INTO [dbo].[CreditLifeRequests]([PartnerId],[CustomerId],[Premium],[SumAssured]," +
                    "[LoanTenure],[TransactionId],[RepaymentPeriod],[LoanReference],[Processed])" +
                    "VALUES('"+ partnerId.ToString() +"','"+ request.Id  +"','"+ onboardingDto.PremiumAmount +"','"+
                    onboardingDto.SumAssured +"','"+ onboardingDto.Loanterm +"','"+ TransactionId +"','"+ 
                    onboardingDto.RepaymentPeriod +"','"+ onboardingDto.LoanReference +"','0')";
                await _db.Connection.ExecuteAsync(addCreditRequest);
                responseDTO.Success = true;
                responseDTO.ErrorMsg = "";
                responseDTO.ResponseId = onboardingId;
                responseDTO.ProductRef = TransactionId;
            }
            catch (Exception ex)
            {
                _isettings.LogRequests(ex.Message, "OnboardingRequest", RequestType.Error);
            }
            return responseDTO;
        }
        public void AddMainRecord(OnboardingDTO request,string onboardingId,string partnerId,
            string TransactionId,int  partnerProductId)
        {
            
           
                
                string addquery = "INSERT INTO [dbo].[Customers] ([surname] ,[otherNames] ,[email]" +
                " ,[phoneNumber] ,[dob] ,[gender] ,[idNumber]  ,[nationality] ,[residency] " +
                ",[occupation],[DateCreated] )  VALUES ('"+ request.FirstName +"','"+ request.OtherNames +"','"+ 
                request.Email +"','"+ request.PhoneNumber +"','"+ request.DateOfBirth +"','"+ request.Gender.ToString() +"','"+ request.IDNumber +"'," +
                "'"+ request.Nationality +"','"+ request.Residency +"'," +
                "'"+ request.Occupation  +"',getdate())";
               _mainDb.Connection.Execute(addquery);
        }
         public async Task<QuoteResponseDTO> GetQuote(QuoteRequestDTO request,string partnerCode)
        {
            var response = new QuoteResponseDTO();
            try
            {
                if (string.IsNullOrEmpty(request.Loanterm.ToString()))
                {

                }
                if (request.Loanterm <= 0)
                {
                    response.success = false;
                    response.processed = false;

                      return response;
                }
                if (string.IsNullOrEmpty(partnerCode.ToString()))
                {
                    response.success = false;
                    response.processed = false;
                    response.errormsg = "Invalid partnerCode";
                      return response;
                }
                if (string.IsNullOrEmpty(request.Loanterm.ToString()))
                {
                    response.success = false;
                    response.processed = false;
                    response.errormsg = "Invalied Loanterm";
                    return response;
                }
                if (string.IsNullOrEmpty(request.sumAssured.ToString()))
                {
                    response.success = false;
                    response.processed = false;
                    response.errormsg = "Invalid sumAssured";
                    return response;
                }
                if((int) _db.Connection.ExecuteScalar<int>("select count(0) from Partners where PartnerCode='"+partnerCode + "'")==0)
                {
                response.success = false;
                response.processed = false;
                response.discount = 0;
                response.totalPremium = 0;
                response.compensationLevy = 0;
                response.coverPremium = 0;
                response.errormsg = "Invalid Partner Codes";
                    return response;
                }
                double rate = await _db.Connection.ExecuteScalarAsync<double>("select Rate from PartnerRates where PartnerCode='" +
                    partnerCode + "'");
                decimal premium = (decimal)(rate / 100 * request.sumAssured) *  ((decimal)request.Loanterm / 12) ;
                response.success = true;
                response.processed = true;
                response.discount = 0;
                response.totalPremium = Math.Round(premium,0);
                response.compensationLevy = 0;
                response.coverPremium = Math.Round(premium,0);
                response.errormsg = "";

            }
            catch (Exception ex) {
            
            _isettings.LogRequests(ex.Message, "GetQuote", RequestType.Error);
            }
            return response;
        }
        public async Task<ResponseDTO> ProcessRequest(MsureDTO msureDTO)
        {
            var responseDTO = new ResponseDTO();
            try
            {
                string trnId = Guid.NewGuid().ToString();
                int partnerId = (int)_db.Connection.ExecuteScalar("select Id as partnerId from  [dbo].[Partners] where [PartnerCode]='" + msureDTO.partnerCode + "'");
                int transactionId = (int)_db.Connection.ExecuteScalar(" select count(1) from [dbo].[MsureRequests] where transactionId='" + msureDTO.transactionId + "'");
                if (transactionId > 0)
                {
                    responseDTO.ErrorMsg = "Transaction already exists";
                    responseDTO.Success = false;
                    return responseDTO;
                }

                int custId = (int)_db.Connection.ExecuteScalar(" select count(1) from [dbo].[MsureRequests] where customerId='" + msureDTO.customerId + "'");
                if (custId > 0)
                {
                    responseDTO.ErrorMsg = "Customer already exists";
                    responseDTO.Success = false;
                    return responseDTO;
                }
                var request = new MsureRequests()
                {
                    benefitOption = msureDTO.benefitOption,
                    CreatedOn = DateTime.Now,
                    customerId = msureDTO.customerId,
                    optinTime = msureDTO.optinTime,
                    PartnersId = partnerId,
                    Customername = msureDTO.customerName,
                    Gender = msureDTO.gender.ToString(),
                    premium = msureDTO.premium,
                    ProductsId = msureDTO.productId == null ? null : Convert.ToUInt16(msureDTO.productId),
                    status = msureDTO.status,
                    transactionId = msureDTO.transactionId,
                    Processed = false,
                    Id = trnId,
                };
                _db.msureRequests.Add(request);
                await _db.SaveChangesAsync();
                responseDTO.Success = true;
                responseDTO.ErrorMsg = "";
                responseDTO.ResponseId = trnId;
                 responseDTO.ProductRef = msureDTO.transactionId.ToString();
            } catch(Exception ex){
                responseDTO.ResponseId = msureDTO.transactionId;
           
                 _isettings.LogRequests(ex.Message, "GetQuote", RequestType.Error);
            }

            return responseDTO;
        }
      //public async Task<RateResponse> CalculateLastExpenseRate(LastExpenseDTOCalcrequest lastExpenseRequest)
      //  {

      //      var ratersponse = new RateResponse();
      //      try
      //      {
      //          int hasParent = 0;
      //          int childcount = 0;
      //          double total = 0;
      //          if (lastExpenseRequest != null)
      //          {
      //              hasParent = (int)_db.Connection.ExecuteScalar("select count(1) as hasParent from fadhiliFamilyMembers a,[dbo].[relationShips] b " +
      //                  "where a.relationShip=b.id and a.productId=" + lastExpenseRequest.productId + " and b.RelationType=" + (int)RelationType.parent + " ");
      //              childcount = (int)_db.Connection.ExecuteScalar("select count(1) as hasParent from fadhiliFamilyMembers a,[dbo].[relationShips] b " +
      //                      "where a.relationShip=b.id and a.productId=" + lastExpenseRequest.productId + " and b.RelationType=" + (int)RelationType.child + " ");
                   
      //              if (hasParent > 0)
      //              {
      //                  total = (double)await _db.Connection.ExecuteScalarAsync("select ExtendedRate from" +
      //                      " [dbo].[LastExpenseNCBA] where OptionId=" + (int)lastExpenseRequest.optionType + " and isGroup='"+ lastExpenseRequest.group +"' ");

      //              }
      //              else
      //              {
      //                  total = (double)await _db.Connection.ExecuteScalarAsync("select MainRate from [dbo].[LastExpenseNCBA] where" +
      //                      " OptionId=" + (int)lastExpenseRequest.optionType + " and isGroup='"+ lastExpenseRequest.group +"'");
      //              }

      //              if (childcount > 4)
      //              {
      //                  double childrates = ((double)await _db.Connection.ExecuteScalarAsync("select ChildRate from [dbo].[LastExpenseNCBA] where " +
      //                      "OptionId=" + (int)lastExpenseRequest.optionType + " and isGroup='"+ lastExpenseRequest.group +"'") * (childcount - 4));
      //                  total += childrates;
      //              }
      //          }
      //          ratersponse.Success = true; ;
      //          ratersponse.Errormsg = "";
      //          ratersponse.CoverPremium = total;
      //          ratersponse.Processed = true;
      //          ratersponse.TotalPremium = total;
      //          return ratersponse;

      //      }
      //      catch (Exception ex) {
      //          _isettings.LogRequests(ex.Message, "CalculateLastExpenseRate",RequestType.Error);
      //      }
      //      return null;
      //  }
        public async Task<List<ProductDTO>> GetProducts(string partnerCode)
        {
            var product = new List<ProductDTO>();
            try
            {
               string query = "SELECT [Id],[Name],[Description] ,[Image] FROM [dbo].[partnersProducts] where isnull([Active],'0')='1' " +
                    "and [PartnerId]=(select Id from Partners where [PartnerCode]='" + partnerCode + "')";
                var result = await _db.Connection.QueryAsync(query);
                product = result.Adapt<List<ProductDTO>>();

            }
            catch (Exception ex)
            {
 _isettings.LogRequests(ex.Message, "GetQuote", RequestType.Error);
            }
            return product;
        }
        public async Task<ResponseDTO> LastExpenseOnboardingRequest(OnboardingFuneralRequestDTO onboardingDto,string PartnerCode)
        {
            var responseDTO = new ResponseDTO();

            try
            {
                //    string TransactionId = GenerateTrnNo(7);

                //    string onboardingId = Guid.NewGuid().ToString();
                //    var query = """
                //        SELECT Id AS partnerId FROM  Partners 
                //        WHERE PartnerCode=@PartnerCode
                //        """;
                //    int partnerId = _db.Connection.ExecuteScalar<int>(query, new { PartnerCode =PartnerCode });
                //    if (partnerId <= 0)
                //    {
                //        responseDTO.ErrorMsg = "Partner does not exist";
                //        responseDTO.Success = false;
                //        return responseDTO;
                //    }

                //    string prod = """
                //                     SELECT Id AS partnerProductId 
                //                     FROM partnersProducts 
                //                     WHERE PartnerId=@PartnerId
                //                     """;
                //    int partnerProductId = _db.Connection.ExecuteScalar<int>(prod, new { PartnerId = partnerId });
                //    if (partnerProductId <= 0)
                //    {
                //        responseDTO.ErrorMsg = "Partner Product does not exist";
                //        responseDTO.Success = false;
                //        return responseDTO;
                //    }

                //    var reg = """
                //        SELECT RegNumber FROM OnboardingRequests 
                //        WHERE RegNumber=@RegNumber
                //        """;
                //    var RegNumber = _db.Connection.ExecuteScalar<string>(reg, new { onboardingDto.RegNumber });
                //    if (!string.IsNullOrEmpty(RegNumber))
                //    {
                //        responseDTO.ErrorMsg = "Registration Number already exists";
                //        responseDTO.Success = false;
                //        return responseDTO;
                //    }

                //    var idNo = """
                //        SELECT IDNumber FROM OnboardingRequests 
                //        WHERE IDNumber=@IDNumber
                //        """;
                //    var IDNumber = _db.Connection.ExecuteScalar<string>(idNo, new { onboardingDto.IDNumber });

                //    if (!string.IsNullOrEmpty(IDNumber))
                //    {
                //        responseDTO.ErrorMsg = "ID Number already exists";
                //        responseDTO.Success = false;
                //        return responseDTO;
                //    }
                //    var request = new OnboardingFuneralRequestDTO()
                //    {
                //        PartnerId = partnerId,
                //        TransactionId = TransactionId,
                //        ProductId = partnerProductId,
                //        CustomerName = onboardingDto.CustomerName,
                //        DateOfBirth = onboardingDto.DateOfBirth,
                //        IDNumber = onboardingDto.IDNumber,
                //        Gender = onboardingDto.Gender,
                //        Premium = onboardingDto.Premium,
                //        BenefitOption = onboardingDto.BenefitOption,
                //        BeneficiaryName = onboardingDto.BeneficiaryName,
                //        RegNumber = onboardingDto.RegNumber,
                //        BeneficiaryMobileNumber = onboardingDto.BeneficiaryMobileNumber,
                //        CreatedOn = DateTime.Now,
                //        Id = onboardingId.ToString(),
                //        Processed = true,
                //        Status = "success"
                //    };
                //    _db.OnboardingRequests.Add(request);
                //    await _db.SaveChangesAsync();
                //    responseDTO.Success = true;
                //    responseDTO.ErrorMsg = "";
                //    responseDTO.ResponseId = onboardingId;
                //    responseDTO.TransactionId = TransactionId;
            }
            catch (Exception ex)
            {
                _isettings.LogRequests(ex.Message, "OnboardingRequest", RequestType.Error);
            }
            return responseDTO;
        }


    }
}

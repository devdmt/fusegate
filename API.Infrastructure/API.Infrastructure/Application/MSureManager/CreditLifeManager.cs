using API.Infrastructure.Interface;
using DAL;
using DAL.Model;
using DAL.ModelView;
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
                var request = new Customers()
                {
                     PartnerId = partnerId,
                  
                    Id = onboardingId.ToString(),
                    ProductId = partnerProductId,
                    CustomerName = onboardingDto.FirstName +" "+ onboardingDto.OtherNames,
                    DateOfBirth = onboardingDto.DateOfBirth,
                    IDNumber = onboardingDto.IDNumber,
                    Gender = onboardingDto.Gender,
                    Email= onboardingDto.Email??"",
                     Nationality = onboardingDto.Nationality??"",
                      Occupation = onboardingDto.Occupation ?? "",
                    Residency = onboardingDto.Residency ?? "",
                     RequestDate = onboardingDto.RequestDate?? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    PhoneNumber = onboardingDto.PhoneNumber,   
                     CreatedOn=DateTime.Now,
                    Processed = false,
                    Status = "success"
                };
                _db.customers.Add(request);
                await _db.SaveChangesAsync();


                AddMainRecord(onboardingDto, onboardingId, partnerId.ToString(),
                    TransactionId, partnerProductId);


                string addCreditRequest = "INSERT INTO [dbo].[CreditLifeRequests]([PartnerId],[CustomerId],[Premium],[SumAssured]," +
                    "[LoanTenure],[TransactionId],[RepaymentPeriod],[LoanReference],[Processed])" +
                    "VALUES('"+ partnerId.ToString() +"','"+ request.Id  +"','"+ onboardingDto.Premium +"','"+
                    onboardingDto.SumAssured +"','"+ onboardingDto.LoanTenure +"','"+ TransactionId +"','"+ 
                    onboardingDto.RepaymentPeriod +"','"+ onboardingDto.LoanReference +"','0')";
                await _db.Connection.ExecuteAsync(addCreditRequest);
                responseDTO.Success = true;
                responseDTO.ErrorMsg = "";
                responseDTO.ResponseId = onboardingId;
                responseDTO.TransactionId = TransactionId;
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
                 responseDTO.TransactionId = msureDTO.transactionId.ToString();
            } catch(Exception ex){
                responseDTO.ResponseId = msureDTO.transactionId;
                responseDTO.TransactionId = msureDTO.transactionId.ToString();
            }

            return responseDTO;
        }

        public async Task<List<ProductDTO>> GetProducts(string partnerCode)
        {
            var product = new List<ProductDTO>();
            try
            {
                string query = "SELECT [Id],[Name],[Description] ,[Image] FROM [dbo].[partnersProducts] where isnull([Active],'0')='1' " +
                    "and PartnerCode=(select Id from Partners where PartnerCode=" + partnerCode + ")";
                var result = await _db.Connection.QueryAsync(query);
                product = result.Adapt<List<ProductDTO>>();

            }
            catch (Exception ex)
            {

            }
            return product;
        }
    }
}

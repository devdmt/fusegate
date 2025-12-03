using API.Infrastructure.Interface;
using DAL;
using DAL.Model;
using DAL.ModelView;
using Dapper;
using Mapster;
using Microsoft.IdentityModel.Tokens;

namespace API.Infrastructure.Application.MSureManager
{
    public class MSureManager : IMSureManager
    {
        private readonly ApplicationDbContext _db;
        readonly MainDbContext _mainDb;
        readonly Isettings _isettings;
        public MSureManager(ApplicationDbContext db, Isettings isettings,MainDbContext mainDb)
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

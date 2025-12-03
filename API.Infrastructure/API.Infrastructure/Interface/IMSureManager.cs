
using DAL.ModelView;


namespace API.Infrastructure.Interface
{
    public interface IMSureManager:ITransientService
    {
       
        Task<ResponseDTO> ProcessRequest(MsureDTO msureDTO);
        Task<List<ProductDTO>> GetProducts(string partnerCode);
    }
}

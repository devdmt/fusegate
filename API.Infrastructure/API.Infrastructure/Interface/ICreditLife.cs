
using DAL.ModelView;
using DAL.ModelView.CreditLife;


namespace API.Infrastructure.Interface
{
    public interface ICreditLife:ITransientService
    {
        Task<ResponseDTO> OnboardingRequest(CreditLifeDTO onboardingDto);
        Task<ResponseDTO> ProcessRequest(MsureDTO msureDTO);
        Task<List<ProductDTO>> GetProducts(string partnerCode);
        Task<QuoteResponseDTO> GetQuote(QuoteRequestDTO request,string partnerCode);
    }
}

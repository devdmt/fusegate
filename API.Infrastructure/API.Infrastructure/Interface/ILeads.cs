using DAL.Model;
using DAL.ModelView;
using System.Threading.Tasks;

namespace API.Infrastructure.Interface
{
    public interface ILeads : ITransientService
    {
        Task<ResponseDTO> AddLeads(CustomerApiLeads customerLeads,string PartnerCode);
    }
}

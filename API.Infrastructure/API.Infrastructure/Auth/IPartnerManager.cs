using DAL.ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Infrastructure.Auth
{
    public interface IPartnerManager:ITransientService
    {
        Task<AuthResponse> AuthenticatePartner(UserLoginDTO userLogin);
        Task<ResponseDTO> CreatePartner(PartnerDTO userLogin);
        Task<ResponseDTO> CreatePartnerUser(PartnerUserDTO partnerUser);
    }
}

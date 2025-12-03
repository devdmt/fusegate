using DAL.ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Infrastructure.Interface
{
    public interface IAuthenticate: ITransientService
    {
        Task<AuthResponse> GenerateToken(string key);
    }
}

using DAL.ModelView;
using DAL.ModelView.Pension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Infrastructure.Interface
{
    public interface IPay:ITransientService
    {
        Task<ResponseDTO> ProcessSTK(STKContributionDTO contributionDTO);
    }
}

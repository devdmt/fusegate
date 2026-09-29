using Azure;
using DAL.Model;
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
        Task<ResponseDTO> ProcessSTK(STKContributionDTO contributionDTO, EndPointType endPointType= EndPointType.Mpesa_Pension_STK_CallbackUrl);
         Task<ResponseDTO>  ProcessSTK_Insure(STKContributionDTO request, EndPointType endPointType = EndPointType.Mpesa_Pension_STK_CallbackUrl);
        Task<ResponseDTO> ProcessPensionSTKResult(string? body, MpesaSTKResult sTKResult); 
        Task<ResponseDTO> ProcessInsureSTKResult(string? body, MpesaSTKResult sTKResult);
    }
}

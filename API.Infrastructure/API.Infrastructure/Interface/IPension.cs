using DAL.ModelView.Pension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Infrastructure.Interface
{
    public interface IPension :ITransientService
    {
        Task<OnboardResponse> OnboardingRequest(PensionOnboardingDTO onboardingDto,string Ip,string partnerId);
    }
}

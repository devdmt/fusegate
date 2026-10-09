using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Infrastructure.Interface
{
    public interface IComunication: ITransientService
    {
        //  Task ProcessEmailNotifications();
        Task ProcessSMS();
     //   Task ProcessGmails();
    }
}

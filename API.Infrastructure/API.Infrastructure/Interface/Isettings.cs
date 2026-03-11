

using DAL.ModelView;

namespace API.Infrastructure.Interface
{
    public interface Isettings:ITransientService
    {
        void LogRequests(string errMsg, string module, RequestType requestType,string request="");
        string GenerateRadomCode(int length = 8);
        Task<string> AddRequest(ApiRequestsDTO apiRequestsDTO);
       void UpdateRequest(UpdateRequestsDTO updateRequests);
          (int Age, bool Success) CalculateAge(string input);

    }

    public enum RequestType
    {
        Incoming,
        Outgoing,
        Error,
        Info, Comparison, fortesting
    }
}

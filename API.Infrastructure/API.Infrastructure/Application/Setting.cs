using API.Infrastructure.Interface;
using DAL;
using DAL.ModelView;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Dapper;
namespace API.Infrastructure.Application;

    public  class Setting:Isettings
    {
     readonly ApplicationDbContext _db;
    public Setting(ApplicationDbContext db)
    {
        _db = db;
    } 
    
    public void UpdateRequest(UpdateRequestsDTO request)
        {
        try
        {

            string insertQuery = "UPDATE [dbo].[ApiRequests]  SET [Response] =@Response " +
            " ,[Responded] =@Responded ,[Failed] =@Failed ,[ErrorMsg] =@ErrorMsg,RespondedOn=getdate()  WHERE Id=@Id ";
            var parameters = new
            {
                RequestName = request.Response,
                Responded = request.Responded,
                Failed = request.Failed,
                ErrorMsg = request.ErrorMsg,
                Id = request.Id

            };
             _db.Connection.ExecuteScalar(insertQuery, parameters);

        }
        catch (Exception ex)
        {
            LogRequests(ex.Message + "|" + ex.StackTrace, "PensionOnboardingError", RequestType.Error);
        }
        }
    public async Task<string> AddRequest(ApiRequestsDTO apiRequestsDTO)
        {
            try
            {
             var Id= Guid.NewGuid().ToString(); 
            string insertQuery = "INSERT INTO [dbo].[ApiRequests](Id,[RequestName] ,[RequestType] " +
                ",[ApiName] ,[PayLoad],[CreatedOn] ,[IP] " +
                ")VALUES (@Id,@RequestName ,@RequestType ,@ApiName ,@PayLoad  " +
                ",getdate() ,@IP);";
                var parameters = new
                {
                    Id= Id,
                    RequestName = apiRequestsDTO.RequestName,
                    RequestType = apiRequestsDTO.RequestType,
                    ApiName = apiRequestsDTO.ApiName,
                    PayLoad = apiRequestsDTO.PayLoad,
                    IP = apiRequestsDTO.IP

                };
                 await _db.Connection.ExecuteScalarAsync(insertQuery, parameters);
                return Id;

            } catch (Exception ex)
            {
                LogRequests(ex.Message + "|" + ex.StackTrace, "PensionOnboardingError", RequestType.Error);
            }
          return null;
        }
    public  string GenerateRadomCode(int length = 8)
    {
             string Alphabet = "ABCDEFGHIJKLMN0PQRSTUVWXYZ23456789"; // no 0,O,1,I
        Span<char> code = stackalloc char[length];
        for (int i = 0; i < length; i++)
        {
            int idx = RandomNumberGenerator.GetInt32(Alphabet.Length);
            code[i] = Alphabet[idx];
        }
        return new string(code);
    } 
    public void LogRequests(string errMsg, string module, RequestType requestType, string request = "")
    {
        try
        {
            //HERE I LOG THE ERRORS BECAUSE I WAS TAUGHT WELL
            //I WILL ALSO CREATE A CLASS FOR MAILING THEM - BECAUSE AM EXEMPLARY
            DateTime currtime = DateTime.Now;
            errMsg = "module: " + module + "  " + requestType.ToString() + ":  " + errMsg + "  " + currtime;
            if(!string.IsNullOrEmpty(request))
            {
                 errMsg = errMsg + "\nrerrorMsg:  " + request;
            }
           
            //string appPath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            string appPath = System.IO.Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "Logs";// _config.configs.Logpath;
            if (!Directory.Exists(appPath))
            {
                Directory.CreateDirectory(appPath);
            }

                appPath = appPath + Path.DirectorySeparatorChar + requestType.ToString() + "_" + String.Format("{0:yyyy-MM-dd}", DateTime.Now).ToString() + "log.msg";
                using (StreamWriter sw = File.AppendText(appPath))
                {
                    sw.WriteLine(errMsg);
                }
            


        }
        catch (Exception ex)
        {

        }
    }
}


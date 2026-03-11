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
                Response = request.Response,
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

            // A general-purpose function to calculate age from a date string supporting multiple formats.
                        // Returns a tuple: (int age, bool success)
                     public    (int Age, bool Success) CalculateAge(string input)
                        {
                            if (string.IsNullOrWhiteSpace(input))
                                return (0, false);

                            // Attempt to handle if input is year only
                            if (int.TryParse(input, out int yearOnly))
                            {
                                // Year must be reasonable (between 1900 and current year)
                                int thisYear = DateTime.Now.Year;
                                if (yearOnly > 1900 && yearOnly <= thisYear)
                                {
                                    int age = thisYear - yearOnly;
                                    return (age, true);
                                }
                                else
                                {
                                    return (0, false);
                                }
                            }

                            // Potential date formats to try
                            var formats = new[]
                            {
                                "dd-MM-yyyy", "yyyy-MM-dd", "dd-MMM-yyyy", "dd-MM-yy", "d-M-yyyy", "d-MMM-yyyy",
                                "yyyy/MM/dd", "dd/MM/yyyy", "MM/dd/yyyy", "M/d/yyyy", "d/MM/yyyy", "dd.MM.yyyy",
                                "d.M.yyyy", "MMM dd, yyyy"
                            };

                            DateTime dob;
                            bool parsed = DateTime.TryParseExact(
                                input,
                                formats,
                                System.Globalization.CultureInfo.InvariantCulture,
                                System.Globalization.DateTimeStyles.None,
                                out dob);

                            if (!parsed)
                            {
                                // Try general parse as last resort
                                if (!DateTime.TryParse(input, out dob))
                                    return (0, false);
                            }

                            DateTime today = DateTime.Today;
                            int age2 = today.Year - dob.Year;
                            if (dob > today.AddYears(-age2)) age2--;

                            if (age2 < 0) // in case of bad future dates
                                return (0, false);

                            return (age2, true);
                        }
    public async Task<string> AddRequest(ApiRequestsDTO apiRequestsDTO)
        {
            try
            {
             var Id= Guid.NewGuid().ToString();
            string insertQuery = "INSERT INTO [dbo].[ApiRequests](Id,[RequestName] ,[RequestType] " +
                ",[ApiName] ,[PayLoad],[IP],[CreatedOn] " +
                ")VALUES ('" + Id + "','" + apiRequestsDTO.RequestName + "','" + (int)apiRequestsDTO.RequestType + "','" + apiRequestsDTO.ApiName + "'" +
                ",'" +  apiRequestsDTO.PayLoad + "','" + apiRequestsDTO.IP + "',getdate())";
                //"" +
                //",@RequestName ,@RequestType ,@ApiName ,@PayLoad  " +
                //",@IP);";
                var parameters = new
                {
                    Id= Id,
                    RequestName = apiRequestsDTO.RequestName,
                    RequestType = apiRequestsDTO.RequestType,
                    ApiName = apiRequestsDTO.ApiName,
                    PayLoad = apiRequestsDTO.PayLoad,
                    IP = apiRequestsDTO.IP

                };
                 await _db.Connection.ExecuteScalarAsync(insertQuery);
                return Id;

            } catch (Exception ex)
            {
                LogRequests(ex.Message + "|" + ex.StackTrace, "PensionOnboardingError", RequestType.Error);
            }
          return null;
        }
    public  string GenerateRadomCode(int length = 8)
    {
             string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // no 0,O,1,I
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


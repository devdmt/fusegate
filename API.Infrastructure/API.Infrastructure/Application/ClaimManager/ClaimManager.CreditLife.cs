using API.Infrastructure.Interface;
using DAL.ModelView.Safaricom;
using DAL.ModelView;
using Azure.Core;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Dapper;
using System.Drawing;
namespace API.Infrastructure.Application.ClaimManager
{
    internal partial class ClaimManager
    {
        public async Task<ValidateResult> ValidateUpload(ValidateRequest request)
        {
            var response= new ValidateResult();
            try
            {
               
                if (!string.IsNullOrEmpty(request.idnumber))
                {
                    int idexistcound = await _db.Connection.ExecuteScalarAsync<int>("select count(0) from [dbo].[phoneInsuranceCustomers] where IdNumber='" + request.idnumber + "'");
                    if(idexistcound == 0) {
                    response.Success = false;
                        response.Error = "Customer Id does not exist";
                        return response;
                    }
                }
                int imisexistcount = await _db.Connection.ExecuteScalarAsync<int>("select count(a.Id) from [dbo].[phoneInsuranceCustomers] a " +
                    ",[dbo].[PhoneInsuranceRequest] b where" +
                    " a.Id=b.PhoneInsuranceCustomerId and  a.IdNumber='" + request.idnumber + "' and (b.IMEINumber ='"+
                    request.imeinumber + "' or b.IMEINumber1='"+ request.imeinumber + "' or b.IMEINumber2='"+ request.imeinumber + "')");
                if (imisexistcount == 0)
                {
                    response.Success = false;
                    response.Error = "Imei uploaded does match with the customer";
                    return response;
                }
                int claimimei = await _db.Connection.ExecuteScalarAsync<int>("select count(0) from [dbo].[claimRequests] where  and Processed ='1' ");
                if (claimimei> 0)
                {
                      response.Success = false;
                    response.Error = "Imei has been claimed before";
                    return response;
                }

            } catch(Exception ex)
            {
                _isettings.LogRequests(ex.Message,"",RequestType.Error);

            }
            return response;
        }
        public async Task<ResponseDTO<UploadResponse>> UploadCreditLife(CreditLifeUpload upload,string userId,string browser,string Ip)
        {
            var response = new ResponseDTO<UploadResponse>();
            var uploadResponse = new UploadResponse();
            try
            {
                var file = await UploadCreditLifeFileAsync(upload.fileDetails, "CreditLife");
                if (!file.Item1.IsNullOrEmpty())
                {
                    FileInfo fileInfo = new FileInfo(file.Item2);
                    ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                    ExcelPackage package = new ExcelPackage(fileInfo);
                    //int columns = worksheet.Dimension.Columns;      
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    int rows = worksheet.Dimension.Rows; // 20
                    int columns = worksheet.Dimension.Columns; // 
                    bool withFailure = false;
                    int firstRecord = 2;
                    int failurecount = 0, successcount = 0;
                    var successfulupload = new List<UploadsDTO>();
                    var uploads = new List<UploadsDTO>();
                    for (int i = firstRecord; i <= rows; i++)
                    {
                        var IdNumber = worksheet.Cells[i, 1].Value ?? "";
                        var Name = worksheet.Cells[i, 2].Value ?? "";
                        var Imei = worksheet.Cells[i, 3].Value ?? "";
                        var LoanBal = worksheet.Cells[i, 4].Value ?? "";
                        var Incidentdate = worksheet.Cells[i, 5].Value ?? "";
                        var loanrefNo = worksheet.Cells[i, 6].Value ?? "";
                        var Deathcert = worksheet.Cells[i, 7].Value ?? "";
                        if (string.IsNullOrEmpty(IdNumber.ToString()))
                        {
                            uploads.Add(new UploadsDTO
                            {
                                DeathCert = Deathcert?.ToString(),
                                ErrorRow = i + 1,
                                FailedReason = "Invalid Id Number",
                                IMeiNumber = Imei?.ToString(),
                                LoanBal = LoanBal?.ToString(),
                                IncidentDate = Incidentdate?.ToString(),
                                Name = Name?.ToString(),
                                IdNumber = IdNumber?.ToString(),
                                RowNumber = i + 1,
                                LoanRef = loanrefNo.ToString(),
                                Valid = false
                            });
                            continue;
                        }
                        if (string.IsNullOrEmpty(Name?.ToString()))
                        {
                            uploads.Add(new UploadsDTO
                            {
                                DeathCert = Deathcert?.ToString(),
                                ErrorRow = i + 1,
                                FailedReason = "Invalid Name",
                                IMeiNumber = Imei?.ToString(),
                                LoanBal = LoanBal?.ToString(),
                                IncidentDate = Incidentdate?.ToString(),
                                IdNumber = IdNumber?.ToString(),
                                RowNumber = i + 1,
                                LoanRef = loanrefNo.ToString(),
                                Valid = false
                            });
                            continue;
                        }

                        if (string.IsNullOrEmpty(LoanBal?.ToString()))
                        {
                            uploads.Add(new UploadsDTO
                            {
                                DeathCert = Deathcert?.ToString(),
                                ErrorRow = i + 1,
                                FailedReason = "Invalid Loan Balance",
                                IMeiNumber = Imei?.ToString(),
                                IdNumber = IdNumber?.ToString(),
                                RowNumber = i + 1,
                                LoanRef = loanrefNo.ToString(),
                                IncidentDate = Incidentdate?.ToString(),
                                Name = Name?.ToString(),
                                Valid = false
                            });
                            continue;
                        }
                        if (string.IsNullOrEmpty(Incidentdate?.ToString()))
                        {
                            uploads.Add(new UploadsDTO
                            {
                                DeathCert = Deathcert?.ToString(),
                                ErrorRow = i + 1,
                                FailedReason = "Invalid Incidence Date",
                                IMeiNumber = Imei?.ToString(),
                                IdNumber = IdNumber?.ToString(),
                                RowNumber = i + 1,
                                LoanRef = loanrefNo.ToString(),
                                IncidentDate = Incidentdate?.ToString(),
                                Name = Name?.ToString(),
                                Valid = false
                            });
                            continue;
                        }
                        if (string.IsNullOrEmpty(Imei?.ToString()))
                        {
                            uploads.Add(new UploadsDTO
                            {
                                DeathCert = Deathcert?.ToString(),
                                ErrorRow = i + 1,
                                FailedReason = "Invalid Imei Number",
                                IMeiNumber = "",
                                IdNumber = IdNumber?.ToString(),
                                RowNumber = i + 1,
                                LoanRef = loanrefNo.ToString(),
                                IncidentDate = Incidentdate?.ToString(),
                                Name = Name?.ToString(),
                                Valid = false
                            });
                            continue;
                        }
                        
                        var validateResult = await ValidateUpload(new ValidateRequest
                        (
                             IdNumber?.ToString(),
                            Imei?.ToString(),
                           LoanBal?.ToString(),
                             loanrefNo?.ToString()
                        ));

                        if (validateResult.Success == false)
                        {
                             uploads.Add(new UploadsDTO
                            {
                                DeathCert = Deathcert?.ToString(),
                                ErrorRow = i + 1,
                                FailedReason = validateResult.Error,
                                IMeiNumber = "",
                                IdNumber = IdNumber?.ToString(),
                                RowNumber = i + 1,
                                LoanRef = loanrefNo.ToString(),
                                IncidentDate = Incidentdate?.ToString(),
                                Name = Name?.ToString(),
                                Valid = false
                            });
                            continue;
                        }
                        successfulupload.Add(new UploadsDTO
                        {
                            DeathCert = Deathcert?.ToString(),
                            ErrorRow = i + 1,
                            FailedReason = "",
                            IMeiNumber = Imei?.ToString(),
                            IdNumber = IdNumber?.ToString(),
                            RowNumber = i + 1,
                            LoanRef = loanrefNo.ToString(),
                            LoanBal = LoanBal?.ToString(),
                            IncidentDate = Incidentdate?.ToString(),
                            Name = Name?.ToString(),
                            Valid = true
                        });

                    }
                    bool withFailed = uploads.Count > 0 ? true : false;

                  

                    string summaryQuery = "INSERT INTO [dbo].[UploadSummary]([FileName]" +
                                 ",[FilePath],[WithFailure],[FailureCount],[SuccessCount],[Processed],[Deleted]) " +
                                 "    VALUES('" + file.Item1 + "','" + file.Item2 + "','" + withFailed + "','" + uploads.Count + "','" + successfulupload.Count + "'," +
                                 "'0','0');select @@identity as Id";
                    var summaryId = _db.Connection.ExecuteScalar<Int64>(summaryQuery);
                   string filename= writetofile(uploads, DateTime.Now.ToString("ddMMyyyyHHmmss"));
                         string   respondfile=  writetofile(uploads, DateTime.Now.ToString("DDMMyyyyHHmmss")+"CreditLifeUploadFailed");
                    successcount=successfulupload.Count;
                     addBulk(successfulupload, summaryId,userId,browser,Ip,true);
                       var content_type = "data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64,";
                    uploadResponse.File = !string.IsNullOrEmpty(respondfile)?content_type + respondfile:"";
                    response.Success=true;
                    response.ErrorMsg = "";
                    uploadResponse.Id = summaryId;
                    uploadResponse.SuccessCount = successcount;
                    uploadResponse.FailureCount = uploads.Count;
                     uploadResponse.withFailed = withFailed;
                    response.Result = uploadResponse;

                }


            }
            catch (Exception ex)
            {
                _isettings.LogRequests(ex.Message, "UploadCreditLife", RequestType.Error);
            }
            return response;
        }
        void addBulk(List<UploadsDTO> stagingdata,long Id,string userId,string browser,string Ip,bool complete)
        {
            try
            {

            } catch (Exception ex)
            {

            }
            for(int i = 0; i < stagingdata.Count; i++)
            {
                var data= stagingdata[i];
                  string uploadquery = "Add_CreditLife_Temp";
                    var param = new DynamicParameters();
                param.Add("@summaryId",Id.ToString());
                param.Add("IdNumber", data.IdNumber);
                param.Add("name", data.Name);
                param.Add("loanBalance", data.LoanBal);
                param.Add("IncidenceDate",data.IncidentDate);
                param.Add("loanref",data.LoanRef);
                param.Add("deathcer",data.DeathCert);
                param.Add("deathcertPath",data.DeathCertPath);
                param.Add("userId",userId);
                param.Add("browser",browser);
                param.Add("Ip",Ip);
                param.Add("complete",complete);

                _db.Connection.Execute(uploadquery, param,commandType: System.Data.CommandType.StoredProcedure);

            }
        }
        string writetofile(List<UploadsDTO> stagingdata,string filename)
        {
            try
            {
                 string path =  Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "CreditLife","FailedFiles"));
                 if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                 string outputfailedfilename = Path.Combine(path, filename + "_" + DateTime.Now.ToString("ddMMyyHHmmss") + "FailedUpload.xlsx");
                string response = "";
                FileInfo newfile = new FileInfo(filename);
                if(newfile.Exists)
                {
                    newfile.Delete();
                    newfile = new FileInfo(filename);
                }
                using(ExcelPackage package = new ExcelPackage(newfile))
                {
                    ExcelWorksheet worksheet= package.Workbook.Worksheets.Add( "Credit Life Uploads");

                    worksheet.Cells[1, 1].Value = "Id Number";
                    worksheet.Cells[1, 2].Value = "Name";
                    worksheet.Cells[1, 3].Value = "Imei Number";
                    worksheet.Cells[1, 4].Value = "Loan Balance";
                    worksheet.Cells[1, 5].Value = "Incidence Date";
                    worksheet.Cells[1, 6].Value = "LoanRef number";
                    worksheet.Cells[1, 7].Value = "Death Certificate";
                    worksheet.Cells[1, 8].Value = "Error Message";
                    if(stagingdata.Count > 0)
                    {
                        var data = stagingdata.Where(a => a.Valid == false).ToList();
                        if(data.Count > 0)
                        {
                            for (int i = 0; i < data.Count; i++)
                            {
                                int row = 2 + i;
                                worksheet.Cells[row, 1].Value = row;
                                worksheet.Cells[row, 2].Value = data[i].IdNumber;
                                worksheet.Cells[row, 3].Value = data[i].Name;
                                worksheet.Cells[row, 4].Value = data[i].IMeiNumber;
                                worksheet.Cells[row, 5].Value = data[i].LoanBal;
                                worksheet.Cells[row, 6].Value = data[i].IncidentDate;
                                worksheet.Cells[row, 7].Value = data[i].LoanRef;
                                worksheet.Cells[row, 8].Value = data[i].DeathCert;
                                worksheet.Cells[row, 9].Value = data[i].ErrorRow;

                            }
                            using (var range = worksheet.Cells[2, 8, data.Count + 1, 8])
                            {
                                range.Style.Font.Bold = true;
                                range.Style.Font.Size = 14;
                                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                range.Style.Fill.BackgroundColor.SetColor(Color.Red);
                                range.Style.Font.Color.SetColor(Color.White);
                            }
                            // worksheet.Columns[4].Style.Fill.BackgroundColor.SetColor(Color.Red);
                            package.Save();
                            using (FileStream reader = new FileStream(filename, FileMode.Open))
                            {
                                byte[] buffer = new byte[reader.Length];
                                reader.Read(buffer, 0, (int)reader.Length);
                                response = Convert.ToBase64String(buffer);
                            }
                            return response;
                        }
                       
                    }
                }
                
              

            } catch(Exception ex)
            {

            }
            return "";
        }

        public void addtoTemp(List<UploadsDTO> uploads)
        {
            for (int i = 0; i < uploads.Count; i++)
            {

            }
        }
    }

    public class UploadsDTO
    {
        public string? Name { get; set; }
        public string? IdNumber { get; set; }
        public string? IMeiNumber { get; set; }
        public string? LoanBal { get; set; }
        public string? IncidentDate { get; set; }
        public string? LoanRef { get; set; }
        public string? DeathCert { get; set; }
        public string? DeathCertPath { get; set; }
        public int? ErrorRow { get; set; }
        public bool Valid { get; set; } = false;
        public string? FailedReason { get; set; }
        public int? RowNumber { get; set; }

    }
    public class ValidateResult
    {
        public bool? Success { get; set; }
        public string? Error { get; set; }
        public int RowNumber { get; set; }
    }

    public record ValidateRequest(string idnumber,string imeinumber,string loanbalance,string loanrefno);
}

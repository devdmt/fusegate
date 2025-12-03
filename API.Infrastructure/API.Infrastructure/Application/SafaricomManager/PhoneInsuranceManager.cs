using API.Infrastructure.Interface;
using DAL;
using DAL.ModelView;
using DAL.ModelView.Safaricom;
using Dapper;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
//using Newtonsoft.Json;
using System.Data;

namespace API.Infrastructure.Application.SafaricomManager
{
    internal partial class PhoneInsuranceManager:IPhoneInsurance
    {

        readonly Isettings _isettings;
        readonly ApplicationDbContext _db;
        public PhoneInsuranceManager(Isettings isettings,ApplicationDbContext db) {
            _isettings = isettings;
            _db = db;   
        
        }
        public async Task<int> AddCustomers(PhoneCustomerDTO request)
        {
            int custId = 0;
            try
            {
                //        string customerquery = "INSERT INTO [dbo].[phoneInsuranceCustomers] ([CustomerName] ,[PhoneNumber] ,[IdNumber] ,[CustomerAddress]  " +
                //",[CreatedOn] ,[CreatedBy]) VALUES ('" + request.CustomerName + "','" + request.PhoneNumber + "','" + request.IdNumber + "','" + request.CustomerAddress + "'" +
                //",getdate(),'" + request.CreatedBy + "');select @@identity as custId";

                var param = new DynamicParameters();
                param.Add("@CustomerName",request.CustomerName);
                param.Add("@PhoneNumber",request.PhoneNumber ?? "");
                param.Add("@IdNumber",request.IdNumber??"");
                param.Add("@CustomerAddress",request.CustomerAddress);
                param.Add("@CreatedBy",request.CreatedBy);
                param.Add("@dateofbirth",SafeDbObject(request.dateOfBirth));
                param.Add("@custId",dbType: DbType.Int32, direction: ParameterDirection.Output);

                 await  _db.Connection.ExecuteAsync("CreateCustomerSaf",param,commandType: System.Data.CommandType.StoredProcedure);
                custId=param.Get<int>("custId");

            }
            catch (Exception ex)
            {
                _isettings.LogRequests(ex.Message,"AddCustomers",RequestType.Error);
            }
           return custId;
        }

        public async Task<ClaimResponseDetails> GetCustomerClaims(string request)
        {
            var response = new ClaimResponseDetails();
            try
            {
                // int customerid = 0;
                var claimsDetails = new List<ClaimsDetailsDTO>();

                string scenario = "";
                if (string.IsNullOrEmpty(request))
                {
                    return null;

                }

                var customerid = await _db.Connection.ExecuteScalarAsync("select Id as customerid from  [phoneInsuranceCustomers] where IdNumber='" + request + "' or right(PhoneNumber,9)=right('" + request + "',9) ") ?? 0;
                if (Convert.ToInt16(customerid) == 0)
                {
                    customerid = (string)await _db.Connection.ExecuteScalarAsync("select PhoneInsuranceCustomerId as CustomerId from  [PhoneInsuranceRequest] where IMEINumber='" + request + "' or " +
                        "IMEINumber1='" + request + "' or IMEINumber2='" + request + "' ");
                }
                if (Convert.ToInt16(customerid) == 0)
                {
                    return null;
                }
                else
                {
                    var customer = await _db.Connection.QueryFirstOrDefaultAsync<PhoneCustomerDTO>("SELECT [Id],[CustomerName],[PhoneNumber],[IdNumber],[CustomerAddress],[Nextofkinname]," +
                        "[NextofkinId] FROM [dbo].[phoneInsuranceCustomers] where Id=" + customerid + "");
                    if (customer != null)
                    {
                        response.phoneCustomer = customer;
                    }
                }
                var claimsDeta = await _db.Connection.QueryAsync("select b.Id as RequestId, a.PhoneInsuranceCustomerId as CustomerId,a.[Id],a.[ProductID],a.[CustomerName],a.[ClaimRefNumber],a.[IDNumber] as IDNumber,a.[IMEINO] as IMEINO,a.[ClaimType],a.[DamagePart],a.[ReplacementCost],a.[IncidentDate],a.[ClaimDate]," +
                    "[Abstract],a.[Processed],a.[PartnerCode],a.[RequestId],a.[TrnId],a.[ResponseId]  from  [claimRequests] a, PhoneInsuranceRequest b where a.RequestId=b.Id and a.PhoneInsuranceCustomerId='" + customerid + "'");
                claimsDetails = claimsDeta.Adapt<List<ClaimsDetailsDTO>>();
                response.claimsDetails = claimsDetails;

                //var customers = new PhoneCustomerDTO();
                //customers = await _db.Connection.ExecuteAsync<PhoneCustomerDTO>("select Id,CustomerName,PhoneNumber,IdNumber,Nextofkinname,NextofkinId from  [phoneInsuranceCustomers] where Id='"++"'");
            }
            catch (Exception ex)
            {
                _isettings.LogRequests(ex.Message, "GetCustomerClaims", RequestType.Error);
            }
            return response;
        }
        public async Task<OnboardingResponseDTO> PurchaseInsurance(PhoneInsuranceRequest request)
        {

            var response = new OnboardingResponseDTO();
            try
            {
                // _isettings.LogRequests(JsonConvert.SerializeObject(request),"PurchaseInsurance",RequestType.Comparison);

                if(request == null)
                {
                    response.ErrorMsg = "invalid request, please  check the data";
                    response.Success = false;
                    
                    return response;
                }

                if(request.IMEINumber == null)
                {
                    response.ErrorMsg = "Invalid IMEI, please  check the data";
                    response.Success = false;

                    return response;
                }
                if (string.IsNullOrEmpty(request.CustomerName))
                {
                    response.ErrorMsg = "Invalid customer name, please  check the data";
                    response.Success = false;

                    return response;
                }
                if (string.IsNullOrEmpty(request.SerialNumber))
                {
                    response.ErrorMsg = "Invalid serial number";
                    response.Success = false;

                    return response;
                }
                if (Convert.ToDouble(request.PhoneCost)==0)
                {
                    response.ErrorMsg = "Invalid Phone cost";
                    response.Success = false;

                    return response;
                }
                if(request.ModeOfPurchase==null)
                {
                    response.ErrorMsg = "Invalid Mode of Purchase";
                    response.Success = false;

                    return response;
                }
                if (string.IsNullOrEmpty(request.PhoneModel))
                {
                    response.ErrorMsg = "Invalid Phone Model";
                    response.Success = false;

                    return response;
                }

                int partnerId = (int)_db.Connection.ExecuteScalar("select Id as partnerId from  [dbo].[Partners] where [PartnerCode]='" + request.PartnerCode + "'");
                if (partnerId == 0)
                {
                    response.ErrorMsg = "Invalid Partner Code";
                    response.Success = false;
                    response.ResponseId = request.RequestId ?? "";
                    return response;
                }
                int transactionId = (int)_db.Connection.ExecuteScalar(" select count(1) from [dbo].[PhoneInsuranceRequest] where RequestId='" + request.RequestId + "' and PartnerID='"+ partnerId +"'");
                if (transactionId > 0)
                {
                    response.ErrorMsg = "Request already exists";
                    response.Success = false;
                    response.ResponseId = request.RequestId ?? "";
                    return response;
                }
          string? LoanRefNumber = request.LoanPurchase?.LoanRefNumber;
          string? RepaymentTerms = request.LoanPurchase?.RepaymentTerms;
          double? LoanAmount  = request.LoanPurchase?.LoanAmount;
          double? InterestRate = request.LoanPurchase?.InterestRate;
           double? PremiumPaid= request.LoanPurchase?.PremiumPaid;
                var customer_Id = _db.Connection.ExecuteScalar("select isnull(Id,0) as customerId from  [dbo].[phoneInsuranceCustomers] where [IdNumber]='" + request.Idnumber + "' and " +
                    "right(PhoneNumber,9)=right('"+ request.Phonenumber +"',9)")??0;
                if (Convert.ToInt64(customer_Id)==0)
                {
                    var custdetails = new PhoneCustomerDTO()
                    {
                        CreatedBy = "API",
                         CustomerName = request.CustomerName,
                          IdNumber = request.Idnumber,
                        SecondaryContact = request.SecondaryContact,
                        SecondaryContactName = request.SecondaryContactName,
                             PhoneNumber= request.Phonenumber,
                              dateOfBirth= request.dateOfBirth

                    };
                    customer_Id = await AddCustomers(custdetails);
                }
                string ImeiNumber="",ImeiNumber1 = "",ImeiNumber2 = "";
                if (request.IMEINumber.Count > 0)
                {
                    ImeiNumber= request.IMEINumber[0];
                    ImeiNumber1= request.IMEINumber.Count>1? request.IMEINumber[1]:"";
                    ImeiNumber2 = request.IMEINumber.Count > 2?request.IMEINumber[2]:"";
                }
                string Id= Guid.NewGuid().ToString();
                //string query = "INSERT INTO [dbo].[PhoneInsuranceRequest] ([Id] ,[PartnerID], [ProductID] ,[CustomerName]  ,[PhoneModel] ,[RequestId] ,[IMEINumber] ,[PhoneCost] ,[ModeOfPurchase] ,[LoanRefNumber]," +
                //    "[RepaymentTerms],[LoanAmount] ,[InterestRate] ,[PremiumPaid] ,[PurchaseDate] ,[Processed] ,[RequestedOn] ,[PolicyStatus]," +
                //    "PhoneInsuranceCustomerId,IMEINumber1,IMEINumber2,SecondaryContactName,SecondaryContactPhone,repaymentPeriod)VALUES ('" + Id + "','" + partnerId + "','"+ request.ProductID +"','"+ request.CustomerName +"','" +
                //    request.PhoneModel  + "','" + request.RequestId  +"','" + ImeiNumber  +"','"+ request.PhoneCost +"','"+ (int)request.ModeOfPurchase +"'," +
                //    "'"+ LoanRefNumber + "','"+ RepaymentTerms + "','"+ LoanAmount + "','"+ InterestRate + "','"+ PremiumPaid + "'," +
                //    "'"+ request.PurchaseDate + "','0',getdate(),'"+ (int)request.PolicyStatus + "',"+ customer_Id + ",'"+ ImeiNumber1 + "','"+ ImeiNumber2 + "','"+ request.SecondaryContact + "'," +
                //    "'"+ request.SecondaryContact + "','12',)";

                var param = new DynamicParameters();
                param.Add("@Id", Id);
                param.Add("@PartnerID", partnerId);
                param.Add("@ProductID", request.ProductID);
                param.Add("@CustomerName", request.CustomerName);
                param.Add("@PhoneModel", request.PhoneModel);
                param.Add("@RequestId", request.RequestId);
                param.Add("@IMEINumber", ImeiNumber);
                param.Add("@PhoneCost", request.PhoneCost??"");
                param.Add("@ModeOfPurchase", request.ModeOfPurchase);
                param.Add("@LoanRefNumber", LoanRefNumber??"");
                param.Add("@RepaymentTerms", RepaymentTerms??"");
                param.Add("@LoanAmount", LoanAmount ?? 0);
                param.Add("@InterestRate", InterestRate??0);
                param.Add("@PremiumPaid", PremiumPaid??0);
                param.Add("@PurchaseDate", request.PurchaseDate ?? DateTime.Now.ToString());
                param.Add("@PolicyStatus", (int)request.PolicyStatus);
                param.Add("@PhoneInsuranceCustomerId", customer_Id);
                param.Add("@IMEINumber1", ImeiNumber1 ?? "");
                param.Add("@IMEINumber2", ImeiNumber2??"");
                param.Add("@SecondaryContactName", request.SecondaryContact ?? "");
                param.Add("@SecondaryContactPhone", request.SecondaryContact ?? "");
                param.Add("@repaymentPeriod", "12");
                param.Add("@serialNumber", request.SerialNumber);
                param.Add("@phoneNumber", SafeDbObject(request.Phonenumber));
                param.Add("@mpesaref", SafeDbObject(request.MpesaRef));
                string query = "AddInsuranceRequest";
                await _db.Connection.ExecuteAsync(query,param,commandType:System.Data.CommandType.StoredProcedure);
                response.ErrorMsg = "Your request was accepted successfuly";
                response.Success = true;
                response.ResponseId = request.RequestId ?? "";
                response.CustomerId = customer_Id.ToString();
                response.TransactionId = Id.ToString();


                return response;

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ResponseId = request.RequestId ?? "";
                response.CustomerId = "";
                response.TransactionId = "";
                //_isettings.LogRequests(JsonConvert.SerializeObject(request),"PurchaseInsurance",RequestType.Comparison);
                _isettings.LogRequests(ex.Message, "PurchaseInsurance", RequestType.Error);
                return response;
            }

            return null;
        }
        //public object SafeDbObject(object input)
        //{
        //    if(input == null)
        //    {
        //        return DBNull.Value;
        //    }
        //    return input;
        //}
        public object  SafeDbObject(object input)=>input==null?DBNull.Value:input;
        public async Task<ResponseDTO> ReplaceClaimRequest(ReplaceRequestDeviceDTO request)
        {
            var response = new ResponseDTO();
            try
            {
                var originalclaim = await _db.Connection.QueryFirstOrDefaultAsync<ReplaceClaimsData>("select Id as RequestId,[CustomerName] ,[IDNumber] as customerId from " +
                    "[dbo].[claimRequests] where ClaimRefNumber='" + request.claim_ref + "'");
                if ((originalclaim== null))
                {
                    response.Success= false;
                    response.ErrorMsg = "Could not find the reference number";
                    return response;
                }
                string imei1 = "", imei2 = "";
                if (request.IMEINO.Count > 1)
                {
                    imei2= request.IMEINO[1];
                }
                imei1 = request.IMEINO[0];
                string createreplace = "INSERT INTO [dbo].[ReplaceRequest]([response_code],[response_message],[claim_ref]" +
                    ",[requestClaimId],[customerId],[customername],[devicecost],[IMEINO1],[IMEINO2],[IMEINO],[replaceDate],[createdDate],[merchantId],[transactionRef])" +
                    "VALUES('"+ request.response_code +"','"+ request.response_message +"','"+ request.claim_ref +"','"+ originalclaim.RequestId +"','"+ originalclaim.customerId +"','"+ originalclaim.CustomerName +"'," +
                    "'"+ request.devicecost +"','"+ imei1 +"','"+ imei2 +"','"+ string.Join(',', request.IMEINO) +"','"+ request.replaceDate +"',getdate(),'"+ request.merchantId +"','"+ request.transactionRef +"')";
                await _db.Connection.ExecuteAsync(createreplace);
                response.Success = true;
                response.ErrorMsg = "";
            }
            catch (Exception ex)
            {
                _isettings.LogRequests(ex.Message, "ReplaceClaimRequest", RequestType.Error);
            }
            return response;
        }

        public async Task<ResponseDTO> SubmitClaim(ClaimRequestDTO request)
        {
            var response= new ResponseDTO();
            try
            {

                //check if customer exists using ID number
                string abstractfile = "";
                if (request == null)
                {
                    response.ErrorMsg = "Invalid Data";
                    return response;
                }
                if(request.ClaimType == null)
                {
                    response.ErrorMsg = "Please provide the claim type";
                    response.Success = false;
                    response.ResponseId = request.RequestId ?? "";
                    return response;
                }
                if(request.ClaimType== ClaimType.damage)
                {
                    if(request.DamagePart==null)
                    {
                        response.ErrorMsg = "Please provide details for the damage part";
                        response.Success = false;
                        response.ResponseId = request.RequestId??"";
                        return response;
                    }
                    if (request.ReplacementCost == null)
                    {
                        response.ErrorMsg = "Please provide details for the replacement cost";
                        response.ResponseId = request.RequestId ?? "";
                        response.Success = false;
                        return response;
                    }
                }
                else
                if(request.ClaimType==ClaimType.theft)
                {
                    if (request.AbstractAttachment == null)
                    {
                        response.ErrorMsg = "Please upload the abstract form";
                        response.ResponseId = request.RequestId ?? "";
                        response.Success = false;
                        return response;
                    }

                    abstractfile= await   UploadFileAsync(request.AbstractAttachment, "AbstractAttachment"+ request.CustomerName);
                }
                string trnId= Guid.NewGuid().ToString();
                int partnerId = (int)_db.Connection.ExecuteScalar("select Id as partnerId from  [dbo].[Partners] where [PartnerCode]='" + request.PartnerCode + "'");
                if(partnerId==0)
                {
                    response.ErrorMsg = "Invalid Partner Code";
                    response.Success = false;
                    response.ResponseId = request.RequestId ?? "";
                    return response;
                }
               int transactionId = (int)_db.Connection.ExecuteScalar(" select count(1) from [dbo].[claimRequests] where RequestId='" + request.RequestId + "'");
                if (transactionId > 0)
                {
                    response.ErrorMsg = "Request already exists";
                    response.Success = false;
                    response.ResponseId = request.RequestId ?? "";
                    return response; 
                }
                string incidentdate = "";
                string claimdate = "";
                try
                {
                    DateTime dt = new DateTime();
                    DateTime claimdt = new DateTime();
                    DateTime.TryParse(request.IncidentDate,out dt);
                    DateTime.TryParse(request.IncidentDate,out claimdt);
                    incidentdate = dt.ToString("dd-MMM-yyyyThh:mm:ss");
                    claimdate = dt.ToString("dd-MMM-yyyyThh:mm:ss");

                } catch(Exception x)
                {
                    _isettings.LogRequests(x.Message, " SubmitClaim Date validation", RequestType.Error);
                 }
                string query = "INSERT INTO [dbo].[claimRequests](RequestId,PartnerCode,[PartnerID],[CustomerName],[ClaimRefNumber],[IDNumber],[IMEINO],[ClaimType],[DamagePart],[ReplacementCost]," +
     "[IncidentDate],[ClaimDate],[Abstract],[Processed],[CreatedOn],TrnId) VALUES('" + request.RequestId + "','" + request.PartnerCode + "','" + partnerId + "','" + request.CustomerName
     + "','" + request.ClaimRefNumber + "','" + request.IDNumber + "','" + request.IMEINO + "','" + (int)request.ClaimType + "','" + request.DamagePart + "','" + request.ReplacementCost + "'," +
     "'" + incidentdate + "','" + claimdate + "','" + abstractfile + "','0',getdate(),'"+ trnId +"')";
                _db.Connection.Execute(query);
                response.ErrorMsg = "Claim record accepted";
                response.Success=true;
                response.ResponseId = request.RequestId;
                response.TransactionId = trnId;
                return response;

            }
            catch (Exception ex)
            {
                _isettings.LogRequests(ex.Message,"SubmitClaim",RequestType.Error);
            }
            return null;
        }
    }

            public class ReplaceClaimsData
    {
        public string? RequestId { get; set; }
        public string? CustomerName { get; set; }
        public string? customerId { get; set; }

    }
}

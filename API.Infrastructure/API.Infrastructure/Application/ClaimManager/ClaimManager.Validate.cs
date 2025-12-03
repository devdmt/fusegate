using DAL.ModelView.Safaricom;
using DAL.ModelView;

using API.Infrastructure.Interface;
using System.Reflection.Metadata.Ecma335;
using DAL.Model.Safaricom;
using Dapper;
using Mapster;
using DocumentFormat.OpenXml.Vml;

namespace API.Infrastructure.Application.ClaimManager
{
    internal partial class ClaimManager
    {
        public async Task<ValidateClaimRespose> ValidateClaim(ValidateClaimDTO claimDTO)
        {
            var response= new ValidateClaimRespose();
            try
            {
              // validate the cost 


            } catch (Exception ex)
            {
                _isettings.LogRequests(ex.Message,"Validate", RequestType.Error);
            }
            return response;
        }
       
        public async Task<List<ClaimsDTO>> QueryClaim(ClaimSearchDTO request)
        {
            var claims = new List<ClaimsDTO>();
            try
            {

                string  criteria= "";
                if (!string.IsNullOrEmpty(request.request))
                {
                    criteria = " and (a.ClaimRefNumber like '%" + request.request + "%' or " +
                          "a.IDNumber like '%" + request.request + "%' or a.IMEINumber like '%" + request.request + "%')";
                }
                
                string claimsquery = "SELECT a.claimStatus,a.ClaimType,a.Id,b.PhoneModel,b.PhoneModel,a.Approved,  Convert(nvarchar,a.ClaimDate,100) as ClaimDate,a.ClaimRefNumber,a.claimStatus,a.CustomerName,a.DamagePart,a.IDNumber as IDNumber, " +
                    "CONCAT(b.IMEINumber,'|'+b.IMEINumber1 ,'|'+ b.IMEINumber2) as IMEINO ,a.ReplacementCost," +
                    " Replace(a.IncidentDate,' 12:00AM','') as IncidentDate,a.claimStatus,a.Narration,a.PartCost,a.ErrorMessage as Response,Convert(nvarchar,a.DispatchedOn,105) as DispatchedOn,a.Dispatched from [dbo].[claimRequests]  a, [dbo].[PhoneInsuranceRequest] b  " +
                    "where a.[PhoneId] = b.Id and a.UserId='"+ request.UserId + "'  and ShopId="+ request.shopId + " ";
                if (request.ClaimStatus != null)
                {
                    claimsquery = "SELECT a.ClaimType,a.Id,b.PhoneModel,b.PhoneModel,  Convert(nvarchar,a.ClaimDate,100) as ClaimDate,a.ClaimRefNumber,a.claimStatus,a.CustomerName,a.DamagePart,a.IDNumber as IDNumber, CONCAT(b.IMEINumber,'|'+b.IMEINumber1 ,'|'+ b.IMEINumber2) as IMEINO ," +
                    " Replace(a.IncidentDate,' 12:00AM','') as IncidentDate ,a.Approved,a.claimStatus,a.Narration,a.PartCost,a.ErrorMessage as Response,Convert(nvarchar,a.DispatchedOn,105) as DispatchedOn,a.Dispatched from [dbo].[claimRequests]  a, [dbo].[PhoneInsuranceRequest] b  " +
                    "where a.[PhoneId] = b.Id and a.UserId='" + request.UserId + "'  and ShopId=" + request.shopId + "  " +
                    " and claimStatus=" + (int)request.ClaimStatus + "  ";
                }
                claimsquery = claimsquery + criteria;
                if (Convert.ToBoolean(request.Dispatch))
                {
                    claimsquery = claimsquery + " and isnull(a.[Dispatched],'0')='1'";
                }
                claimsquery = claimsquery + " order by a.Id desc";
                var claimresults=await _db.Connection.QueryAsync<ClaimsDTO>(claimsquery);
               claims= claimresults.Adapt<List<ClaimsDTO>>();
            }catch (Exception ex)
            {
                _isettings.LogRequests(ex.Message, "QueryClaim",RequestType.Error);
            }
            return claims;
        }
        public async Task<List<CustomerSearchDTO>> QueryCustomer(string request)
        {
            var response= new List<CustomerSearchDTO>();
            var phonelists= new List<PhoneDTO>();
            try
            {
                //search by Idnumber or phone
                string query = "select * from [dbo].[phoneInsuranceCustomers] where IdNumber='"+ request + "' or right(PhoneNumber,9)=right('"+ request +"',9) ";
                var customers= await _db.Connection.QueryAsync<CustomerSearchDTO>(query);
                response = customers.ToList();
                if(response != null && response.Count>0)
                {
                    for(int i=0;i<response.Count;i++)
                    
                    {
                        string phonequery = "select * from [dbo].[PhoneInsuranceRequest] where PhoneInsuranceCustomerId='"+ response[i].Id +"'";
                        var phones=await _db.Connection.QueryAsync<PhoneDTO>(phonequery);
                        phonelists=phones.Adapt<List<PhoneDTO>>();
                        response[i].Phones = phonelists;
                    }

                }
                else
                {
                    string queryImei = "select * from [dbo].[phoneInsuranceCustomers] where Id =(select PhoneInsuranceCustomerId from PhoneInsuranceRequest " +
                        "where IMEINumber='" + request + "' or IMEINumber1='" + request + "' or IMEINumber2='" + request + "')";
                    var customersImei = await _db.Connection.QueryAsync<CustomerSearchDTO>(queryImei);
                    response = customersImei.ToList();
                    if(customersImei != null)
                    {
                        for (int i = 0; i < response.Count; i++)

                        {
                            string phonequery = "select * from [dbo].[PhoneInsuranceRequest] where PhoneInsuranceCustomerId='" + response[i].Id + "' and" +
                                " (IMEINumber='" + request + "' or IMEINumber1='" + request + "')";
                            var phones = await _db.Connection.QueryAsync<PhoneDTO>(phonequery);
                            phonelists = phones.Adapt<List<PhoneDTO>>();
                            response[i].Phones = phonelists;
                        }
                    }
                }
               


            } catch(Exception ex) { 
            _isettings.LogRequests(ex.Message,"QueryCustomer",RequestType.Error);
            }
            return response;
        }
    }
}

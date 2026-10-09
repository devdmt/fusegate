 
 using DAL.Model;
 using API.Infrastructure.Interface;
 using DAL.ModelView;
 using Microsoft.EntityFrameworkCore;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.ModelView.Pension;

 namespace API.Infrastructure.Application.Pension
 {
    internal partial class PensionManager:IPension
    {

public async Task<ResponseDTO<TransferDTO>> Transfer(TransferRequestDTO requestDTO,string PartnerCode)
{
    var response = new ResponseDTO<TransferDTO>();
    try
    {
        var customer = await _akiba.customers.FirstOrDefaultAsync(a => a.MemberNumber == requestDTO.MemberNo);
        if (customer == null)
        {
            response.Success = false;
            response.AddError("transfer", "Customer not found");
            return response;
        }
        string partnerId = await _db.Connection.QueryFirstOrDefaultAsync<string>("select Convert(nvarchar(50),Id) as Id" +
            " from Partners where  PartnerCode = @PartnerCode", new { PartnerCode = PartnerCode });  
        if(string.IsNullOrEmpty(partnerId))
        {
            response.Success = false;
            response.AddError("transfer", "Partner not found");
            return response;
        }
       
       
        foreach (var employer in requestDTO.TransferDetails)
        {
            
       
    
     string empcode = employer.EmployerName;
     if (employer.EmployerName.Contains(" "))
     {
         empcode= employer.EmployerName.Split(" ")[0];
     }
     empcode= empcode.TrimStart().TrimEnd().Trim()+ _settings.GenerateRadomCode(5);
    if(string.IsNullOrEmpty(employer.PensionProviderName))
    {
        response.AddError("transfer", "Pension provider name is required");
        return response;
    }
    if(string.IsNullOrEmpty(employer.HrEmail))
    {
        response.AddError("transfer", "Hr email is required");
        return response;
    }
    if(string.IsNullOrEmpty(employer.EmployerName))
    {
        response.AddError("transfer", "Employer name is required");
        return response;
    }
    if(string.IsNullOrEmpty(employer.HrEmail))
    {
        response.AddError("transfer", "Hr email is required");
        return response;
    }

    
     string addemp = "INSERT INTO [dbo].[PensionTransfer]([Id],[CustomerId],[Employername] ,[HRemail] " +
                        ",[AdditionalInformation]" +
         " ,[Currentlyfunding],[TransferStatus] ,[Created],CreatedBy,LastModified ,ProviderName,AgentCode,PartnerId" +
         " )" +
         " VALUES ('"+ Guid.NewGuid().ToString() +"','"+ customer.Id +"','"+  
         employer.EmployerName+"','"+ employer.HrEmail +"','"+  employer.AdditionalInformation +"'," +
         "  '"+ employer.CurrentlyFunding +"','"+ (int) TransferStatus.Initiated +"'" +
         ",getdate(),'"+ Guid.Empty +"',getdate(),'"+ employer.PensionProviderName+"','"+ requestDTO.AgentCode +"'," +
         "'"+ partnerId +"')";

      await _akiba.Connection.ExecuteAsync(addemp);
 }

    response.ErrorMsg = "00";
    response.Success=true;
 response.Success = true;
    
    }
    catch (Exception ex)
    {
      _settings.LogRequests(string.Format(ex.Message + "/n"+ "Error occurred while processing pension transfer " +
          "for MemberNo: {MemberNo}", requestDTO.MemberNo),
                    "Transfer",RequestType.Error);
                response.Success = false;
        response.Errors.Add(new ValidationErrorItem { Field = "transfer", Message = ex.Message });
                return response;
    }
    return response;
}


    }
 }
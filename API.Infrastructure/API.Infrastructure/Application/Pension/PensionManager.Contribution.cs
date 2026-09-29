using Azure;
using DAL.Model;
using DAL.Model.Pensioner;
using DAL.ModelView;
using DAL.ModelView.Pension;
using Dapper;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NJsonSchema.Validation;
using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace API.Infrastructure.Application.Pension
{
    internal partial class PensionManager
    {

       
        public async Task<ResponseDTO> Contribute(contributeDTO contributionDTO,string PartnerCode)
        {
            var response = new ResponseDTO();
            var contribution = new PensionContributions();
            try
            {
                if (_akiba is null)
                {
                    response.AddError("contribution", "Database context is not initialized.");
                    return response;
                }

                var contributionSettings = await _akiba.contributionSettings.FirstOrDefaultAsync();
                if (contributionSettings != null) {
                    if (contributionDTO.Amount < contributionSettings.MaximumEeContribution)
                    {
                        response.AddError("Ee_Contribution", $"Employee contribution must be at least {contributionSettings.MaximumEeContribution}");
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
                    
                    if(string.IsNullOrEmpty(contributionDTO.PaymentReference))
        {
            response.Success = false;
            response.AddError("transfer", "Payment reference is required");
            return response;
        }
                    var existingContributionWithSameReference = await _akiba.pensionContributions
                        .FirstOrDefaultAsync(c => c.ThirdpartyRef == contributionDTO.PaymentReference); 
                    if (existingContributionWithSameReference != null)
                    {
                        response.Success = false;
                        response.AddError("contribution", "A contribution with this PaymentReference already exists.");
                        return response;
                    }
                        // Ensure contributionDTO.requestId is unique on callBackResponse
                        var existingContribution = await _db.callBackResponse
                        .FirstOrDefaultAsync(c => c.RequestId == contributionDTO.PaymentReference);

                    if (existingContribution != null)
                    {
                        response.Success = false;
                        response.AddError("contribution", "A contribution with this PaymentReference has already been processed.");
                        return response;
                    }
                    //var customer = await _akiba.customers.FirstOrDefaultAsync(c => c.MemberNo == contributionDTO.MemberNo);
                    var customer = await _akiba.Connection.QueryFirstOrDefaultAsync<PensionCustomer>("SELECT [Id],[Fullname],[CustomerType],[DateOfBirth]" +
                        ",[Email],[NationalNumber],[PhoneNumber],[KRAPin],[AddressLine1],[AddressLine2],[City]," +
                        "[State],[County],[PostalCode],[Country],[Created],[CreatedBy],[LastModified],[LastModifiedBy]," +
                        "[DeletedBy],[CreatedFromIP],[CreatedFromBrowser],[UserId],[Approved],[Confirmed],[ConfirmedBy]," +
                        "[ConfirmedOn],[Comments],[ApprovalStatus],[Firstname],[GroupId],[Lastname],[MemberNumber],[EmployementStatus]," +
                        "[Gender],[Idnumber],[MaritalStatus],[MemberJoinDate],[NSSFCode],[Occupation],[Pensionerstatus],[RegistrationComplete]," +
                        "[Signature],[StaffNumber],[Residency],[CountryCode],[Validate_IPRS_Status],[IPRS_FullNames],[Citizenship],[RefferalCode]" +
                        ",[YearOfBirth],[RegisterChannel],[Active],[RegisteredBackend],[ApprovedBackend],[UploadReference],[Surname]," +
                        "[AdditionalSourceOfIncome],[AverageIncomeId],[BusinessName],[Complete],[EmployerName],[EmploymentTerms],[IdType]" +
                        ",[IsGroupMember],[MailSent],[MailSentOn],[Modified],[NatureOfBusiness],[OtherNames],[Pin],[PolicyPath],[RecordProcessed]," +
                        "[RetryCount],[Role],[RoleInBusiness],[SignaturePath],[SourceOfIncome],[Stage],[Step],[TaxIdNumber],[USAddress],[Validated]," +
                        "[ValidationDate],[ValidationErrorCount],[ValidationErrors],[Processed]  FROM [dbo].[Customers]  where MemberNumber = @MemberNo",
                        new { MemberNo = contributionDTO.MemberNo });
                    if (customer == null)
                    {
                        response.AddError("contribution", "Customer not found.");
                        return response;
                    }

                    PensionerFund? fund = null;
                    ProductTypes ProductTypes  = ProductTypes.IPP;
                    switch (contributionDTO.ProductType)
                    {
                        case  Productenum.IPP:
                            ProductTypes = ProductTypes.IPP;
                            break;
                        case Productenum.PRMF:
                            ProductTypes = ProductTypes.PRMF;
                            break;
                        case Productenum.NSSF:
                            ProductTypes = ProductTypes.NSSF;
                            break;
                        default:
                            response.AddError("contribution", "Invalid product type.");
                            return response;
                    }
                    if (customer != null)
                    {
                        fund = await _akiba.PensionerFund.FirstOrDefaultAsync(
                            pf => pf.CustomerId == customer.Id
                                && pf.ProductTypes == ProductTypes
                        );
                    }


                    if (fund == null)
                    {
                        response.Success = false;
                        response.AddError("contribution", "There was a problem in processing your request");
                        return response;
                    }

                    contribution.Year = DateTime.Now.Year;
                    contribution.Month = DateTime.Now.Month;
                    contribution.Ee_Contribution = contributionDTO.Amount;
                    //contribution.Er_Contribution= contributionDTO.Er_Contribution;
                    contribution.Total_Contribution = (contribution.Er_Contribution ?? 0d) +
                        (contribution.Ee_Contribution ?? 0d) + (contribution.EVC_Contribution ?? 0d);
                    contribution.Created = DateTime.Now;


                    double Total_Contribution = Math.Round(Math.Round(contribution.Ee_Contribution ?? 0d, 2));


                    // var pensionFund = await _dbContext.PensionerFunds.FirstOrDefaultAsync(pf => pf.Id == contributionDTO.GroupId &&
                    // pf.CustomerId == contributionDTO.CustomerId);
                    double ee_unregistred = 0;
                    double ee_registred = 0;
                    double er_unregistred = 0;
                    double er_registred = 0;
                    double ee = Convert.ToDouble(contribution.Ee_Contribution);
                    double er = 0;
                    double total = ee + er;
                    double max = contributionSettings.MaximumRegistered;

                    if (total > max)
                    {
                        if (ee > max)
                        {
                            ee_registred = max;
                            ee_unregistred = ee - max;
                            er_registred = 0;
                            er_unregistred = er;
                        }
                        //else if (er > max)
                        //{
                        //    ee_registred = ee;
                        //    ee_unregistred = 0;

                        //    double er_limit = max - ee;
                        //    er_registred = Math.Min(er, er_limit);
                        //    er_unregistred = er - er_registred;
                        //}
                        else
                        {
                            ee_registred = ee;
                            //ee_unregistred = 0;
                            //er_registred = 0;
                            //er_unregistred = er;
                            ee_unregistred = 0;
                            double er_limit = max - ee;
                            er_registred = Math.Min(er, er_limit);
                            er_unregistred = er - er_registred;
                        }
                    }
                    else
                    {
                        ee_registred = ee;
                        ee_unregistred = 0;
                        er_registred = er;
                        er_unregistred = 0;
                    }
                    //contribution.contributionType = contributionDTO.contributionType;
                    contribution.PensionFundId = fund!.Id;
                    contribution.FullName = customer.Fullname ?? string.Empty;
                    contribution.Idnumber = customer.Idnumber;
                    contribution.Created = DateTime.Now;
                    contribution.Ee_Registered = ee_registred;
                    contribution.Ee_UnRegistered = ee_unregistred;
                    contribution.Er_Registered = er_registred;
                    contribution.Er_UnRegistered = er_unregistred;
                    contribution.PaymentAcknowledged = false;
                    contribution.ThirdpartyRef = contributionDTO.PaymentReference;
                    contribution.PaymentStatus = PaymentStatus.Pending;
                    contribution.CustomerId = customer.Id;
                    contribution.Naration = "Contribution";
                    contribution.PaymentMode = PaymentMode.Mpesa;
                    contribution.Total_Contribution = Total_Contribution;
                    contribution.MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(contribution.Month);

                    


                    _akiba.pensionContributions.Add(contribution);
                    await _akiba.SaveChangesAsync();

                    if (contribution.PaymentMode == PaymentMode.Mpesa)
                    {
                        var trnCode = _settings.GenerateRadomCode(10);
                        //initiate stkpush

                        var stk = await _ipay.ProcessSTK(new STKContributionDTO
                        {
                            MemberNo = contributionDTO.MemberNo,
                            PensionerId = customer.Id.ToString(),
                            Amount = Total_Contribution,
                            Phonenumber = contributionDTO.phoneNumber ?? customer.PhoneNumber ?? string.Empty,
                            TrnCode = contribution.Id.ToString(),

                            ProcessBatch = false


                        });
                        if (stk.Success)
                        {
                           

                           


                            // Ensure correlationId is unique in the callBackResponse table
                            string baseCorrelationId = stk.ProductRef;
                            int attempts = 0;

                            var callbackresponse = new CallBackResponse()
                            {
                                Id = Guid.NewGuid(),
                                CreatedAt = DateTime.Now,
                                CallbackUrl = contributionDTO.callbackUrl,
                                PartnerCode = PartnerCode,
                                Productenum = contributionDTO.ProductType,
                                PartnerId = partnerId,
                                RequestType = "Contribution",
                                Request = JsonConvert.SerializeObject(contributionDTO),
                                Response = "",
                                CorrelationId = baseCorrelationId.ToString(),
                                RequestId = contributionDTO.PaymentReference,
                                CallbackProcessed = false,
                                ProcessResponse = false,
                                Responded = false,
                                RespondedAT = null,
                                Status = "Pending",
                                StatusMessage = "Pending callback dispatch."

                            };

                            var savresponse = await SaveCallbackResponseAsync(contributionDTO: callbackresponse);
                            //_db.callBackResponse.Add(callbackresponse);
                            //await _db.SaveChangesAsync();

                            response.ResponseId = callbackresponse.CorrelationId;
                            
                        }
                    }
                   
                    response.Success=true;
                    response.Success = true;
                    return response;
                }
                response.AddError("contribution", "Contribution settings are not configured.");
                return response;
            }
            catch (Exception ex) {
                _settings.LogRequests(ex.Message, "contribute",Interface.RequestType.Error);
                _settings.LogRequests(ex?.StackTrace, "contribute",Interface.RequestType.Error);
                response.AddError("contribution", ex.Message);
                return response;
            }
        }



public async Task<bool> SaveCallbackResponseAsync(
    CallBackResponse contributionDTO
 )
{
    var id = Guid.NewGuid();
    var createdAt = DateTime.Now;

    const string sql = @"
        INSERT INTO CallBackResponse
        (
            Id,
            CreatedAt,
            CallbackUrl,
            PartnerCode,
            PartnerId,
            RequestType,
            Request,
            Response,
            CorrelationId,
            RequestId,
            CallbackProcessed,
            ProcessResponse,
            Responded,
            RespondedAT,
            Status,
            StatusMessage,Productenum
        )
        VALUES
        (
            @Id,
            @CreatedAt,
            @CallbackUrl,
            @PartnerCode,
            @PartnerId,
            @RequestType,
            @Request,
            @Response,
            @CorrelationId,
            @RequestId,
            @CallbackProcessed,
            @ProcessResponse,
            @Responded,
            @RespondedAT,
            @Status,
            @StatusMessage,@Productenum
        );";

    var parameters = new
    {
        Id = id,
        CreatedAt = createdAt,
        CallbackUrl = contributionDTO.CallbackUrl,
        PartnerCode = contributionDTO.PartnerCode,
        PartnerId = contributionDTO.PartnerId,
        RequestType = "Contribution",
        Request = JsonConvert.SerializeObject(contributionDTO),
        Response = contributionDTO.Response,
        CorrelationId = contributionDTO.CorrelationId.ToString(),
        RequestId = contributionDTO.RequestId,
        CallbackProcessed = false,
        ProcessResponse = false,
        Responded = false,
        RespondedAT = (DateTime?)null,
        Status = contributionDTO.Status,
        StatusMessage = contributionDTO.StatusMessage,Productenum=contributionDTO.Productenum
    };
    await _db.Connection.ExecuteAsync(sql, parameters);
return true;
        } 
    }
}

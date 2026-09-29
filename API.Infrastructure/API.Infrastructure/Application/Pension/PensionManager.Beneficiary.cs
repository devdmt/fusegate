using API.Infrastructure.Interface;
using Azure;
using DAL.Model;
using DAL.ModelView;
using DAL.ModelView.FuneralExpense;
using DAL.ModelView.Pension;
using Dapper;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Infrastructure.Application.Pension
{
    internal partial class PensionManager
    {
        public async Task<ResponseDTO<List<BeneficiaryDetailsDTO>>> AddBeneficiaries(PensionBeneficiaryDTO pensionerBeneficiarieAddDTO)
        {
            var response = new ResponseDTO<List<BeneficiaryDetailsDTO>> { Result = new List<BeneficiaryDetailsDTO>() };

            try
            {
                if (string.IsNullOrWhiteSpace(pensionerBeneficiarieAddDTO.memberNo))
                {
                    response.Success = false;
                    response.AddError("add beneficiary", "Member number is required.");
                    return response;
                }

                var pensionerdetails = await _akiba.Connection.QueryFirstOrDefaultAsync<PensionerDetails>("select" +
                    "  Convert(nvarchar(100),Id) as CustomerId,MemberNumber ,Fullname as PensionerName  from Customers where" +
                    " MemberNumber='" + pensionerBeneficiarieAddDTO.memberNo + "'");
                if (pensionerdetails == null || string.IsNullOrWhiteSpace(pensionerdetails.CustomerId))
                {
                    response.Success = false;
                    response.AddError("add beneficiary", "Member number was not found.");
                    return response;
                }

                var customerId = Guid.Parse(pensionerdetails.CustomerId);
                foreach (var beneficiary in pensionerBeneficiarieAddDTO.beneficiaryDetails)
                {
                    var beneficiaries = new PensionerBeneficiaries();
                    if (beneficiary.BeneficiaryCode == null)
                    {

                        if (string.IsNullOrWhiteSpace(beneficiary.FirstName) ||
                        string.IsNullOrWhiteSpace(beneficiary.OtherNames))
                        {
                            response.Success = false;
                            response.AddError("add beneficiary", "First name or other name is required.");
                            return response;
                        }

                        if (string.IsNullOrEmpty(beneficiary.Date_Of_Birth))
                        {
                            response.Success = false;
                            response.AddError("add beneficiary", "Invalid Date of Birth");
                            return response;
                        }


                        // Check for existing ID number
                        if (!string.IsNullOrWhiteSpace(beneficiary.IDNumber))
                        {
                            var existsByIdNumber = await _akiba.beneficiaries
                                .AnyAsync(b => b.CustomerId == customerId
                                    && b.IdNumber == beneficiary.IDNumber);
                            if (existsByIdNumber)
                            {
                                response.Success = false;
                                response.AddError("Beneficiary", "Beneficiary with this ID number already exists.");
                                return response;
                            }
                        }



                        var age = _settings.CalculateAge(beneficiary.Date_Of_Birth);
                        if (!age.Success)
                        {
                            response.Success = false;
                            response.AddError("Beneficiary", "Invalid Date of Birth");
                            return response;
                        }

                        if (age.Age < 18)
                        {
                            if (beneficiary.guardianDetailsDTO == null)
                            {
                                response.Success = false;
                                response.AddError("Beneficiary", "Guardian details are required for beneficiaries younger than 18 years.");
                                return response;
                            }
                            else
                            {
                                var guardian_age = _settings.CalculateAge(beneficiary.guardianDetailsDTO.Date_Of_Birth ?? "");

                                if (guardian_age.Age < 18)
                                {
                                    if (beneficiary.guardianDetailsDTO == null)
                                    {
                                        response.Success = false;
                                        response.AddError("Beneficiary", "Guardian details are required for beneficiaries younger than 18 years.");
                                        return response;
                                    }
                                }
                            }
                        }

                        // Check for existing phone number
                        if (!string.IsNullOrWhiteSpace(beneficiary.phoneNumber))
                        {
                            var existsByPhone = await _akiba.beneficiaries
                                .AnyAsync(b => b.CustomerId == customerId
                                    && b.Phone == beneficiary.phoneNumber);
                            if (existsByPhone)
                            {
                                response.Success = false;
                                response.AddError("Beneficiary", "Beneficiary with this phone number already exists.");
                                return response;
                            }
                        }

                        // check if dob is 18 years we need id

                        var existbeneficiary = await _akiba.beneficiaries.Where(a => a.CustomerId ==
                   customerId).ToListAsync();
                        if (existbeneficiary.Count > 0)
                        {
                            int totalallocation = existbeneficiary.Sum(a => Convert.ToInt16(a.Percentage));
                            if (totalallocation + Convert.ToInt16(beneficiary.Percentage) > 100)
                            {
                                response.Success = false;
                                response.AddError("Beneficiary", $"Your total beneficiary allocation exceed 100%");
                                return response;
                            }   
                        }


                        beneficiaries = beneficiary.Adapt<PensionerBeneficiaries>();
                        beneficiaries.Firstname = beneficiary.FirstName;
                        beneficiaries.OtherNames = beneficiary.OtherNames;
                        beneficiaries.Relationship = beneficiary.Relationship;
                        beneficiaries.Phone = beneficiary.phoneNumber;
                        beneficiaries.CustomerId = customerId;
                        beneficiaries.PartnerBeneficiaryCode = _settings.GenerateRadomCode(8);
                        beneficiaries.IdNumber = beneficiary.IDNumber;
                        beneficiaries.Date_of_birth = beneficiary.Date_Of_Birth;
                        beneficiaries.Percentage = Convert.ToInt32(beneficiary.Percentage);
                        beneficiaries.Confirmed = true;
                        _akiba.beneficiaries.Add(beneficiaries);
                        await _akiba.SaveChangesAsync();
                        var responsebeneficiary = beneficiary.Adapt<BeneficiaryDetailsDTO>();
                        responsebeneficiary.BeneficiaryCode = beneficiaries.PartnerBeneficiaryCode;
                        if (beneficiary.guardianDetailsDTO != null)
                        {

                            // Map and add Guardian details
                            var guardianDto = beneficiary.guardianDetailsDTO;
                            if (guardianDto != null)
                            {

                                //va
                                var guardian = guardianDto.Adapt<PensionerGurdian>();
                                guardian.BeneficiaryId = beneficiaries.Id; // Link guardian to beneficiary
                                guardian.CustomerId = beneficiaries.CustomerId;
                                guardian.GuardianCode=_settings.GenerateRadomCode(8);
                                guardian.DOB = guardianDto.Date_Of_Birth;
                                guardian.IdNumber = guardianDto.IDNumber;
                                guardian.Surname = guardianDto.OtherNames;
                                guardian.Relationship = guardianDto.Relationship;

                                _akiba.gurdian.Add(guardian);
                                await _akiba.SaveChangesAsync();

                                responsebeneficiary.guardianDetailsDTO = guardianDto.Adapt<GuardianDetailsDTO>();
                                responsebeneficiary.guardianDetailsDTO.GuardianCode = guardian.GuardianCode;

                            }
                        }
                        response.Result!.Add(responsebeneficiary);
                        response.Success = true;

                    }

                    else
                    {
                        beneficiaries = await _akiba.beneficiaries.Where(a => a.PartnerBeneficiaryCode == beneficiary.BeneficiaryCode
                        && a.CustomerId == customerId).FirstOrDefaultAsync();
                        if (beneficiaries != null)
                        {
                            beneficiaries.Firstname = beneficiary.FirstName;
                            beneficiaries.OtherNames = beneficiary.OtherNames;
                            beneficiaries.Relationship = beneficiary.Relationship;
                            beneficiaries.Phone = beneficiary.phoneNumber;
                            beneficiaries.IdNumber = beneficiary.IDNumber;
                            beneficiaries.Date_of_birth = beneficiary.Date_Of_Birth;
                            beneficiaries.Percentage = Convert.ToInt32(beneficiary.Percentage);
                            await _akiba.SaveChangesAsync();
                            var edited = beneficiary.Adapt<BeneficiaryDetailsDTO>();
                            edited.BeneficiaryCode = beneficiaries.PartnerBeneficiaryCode;
                            response.Result!.Add(edited);
                            response.Success = true;
                        }
                        else
                        {
                            response.Success = false;
                            response.AddError("Beneficiary", "Beneficiary not found for update.");
                            return response;
                        }

                        if (beneficiary.guardianDetailsDTO != null)
                        {
                            if(string.IsNullOrEmpty(beneficiary.guardianDetailsDTO.GuardianCode))
                            {
                                response.Success = false;

                        response.AddError("Beneficiary", $"No Guardian code on the record");
                                return response;
                            }
                            var gurdians = await _akiba.gurdian.Where(a => a.GuardianCode == beneficiary.guardianDetailsDTO.GuardianCode).FirstOrDefaultAsync();
                            if (gurdians != null)
                            {
                                gurdians.Surname = beneficiary.guardianDetailsDTO.OtherNames;
                                gurdians.OtherNames = beneficiary.guardianDetailsDTO.OtherNames;
                                gurdians.IdNumber = beneficiary.guardianDetailsDTO.IDNumber;
                                gurdians.DOB = beneficiary.guardianDetailsDTO.Date_Of_Birth;
                                gurdians.Relationship = beneficiary.guardianDetailsDTO.Relationship;
                                await _akiba.SaveChangesAsync();
                                response.Success = true;
                            }

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.AddError("Beneficiary", $"an error occured while creating a beneficiary");
                _settings.LogRequests(ex.Message, "AddBeneficiaries", RequestType.Error);
            }
            return response;
        }
    }
    public class PensionerDetails
    {
        public string CustomerId { get; set; }
        public string MemberCode { get; set; }
        public string PensionerName { get; set; }
    }
}

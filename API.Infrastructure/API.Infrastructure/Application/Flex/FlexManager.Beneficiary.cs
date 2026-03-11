
using API.Infrastructure.Common.Exceptions;
using DAL.Model;
using DAL.ModelView;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Claims;

namespace API.Infrastructure.Application.Flex
{
    // Uses UTC dates for "today" when computing age.
    internal partial class FlexManager
    {
        private static readonly string[] DateFormats = new[]
        {
            "dd-MM-yyyy", "dd/MM/yyyy", "dd/MMM/yyyy", "yyyy-MM-dd",
            "d-M-yyyy",  "d/M/yyyy",  "d/MMM/yyyy",  "yyyy/MM/dd",
            "dd MMM yyyy", "d MMM yyyy",
            // Additional reasonable variants
            "MM/dd/yyyy", "M/d/yyyy", "yyyyMMdd", "dd.MM.yyyy", "d.M.yyyy"
        };


     

        public async Task<ResponseDTO<BeneficiaryResultDto>> AddBeneficiaryAsync(BeneficiaryCreateDTO dto, Guid userId,string partnerCode)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (userId==Guid.Empty) throw new UnauthorizedException("unauthorised");

            var response = new ResponseDTO<BeneficiaryResultDto>();

            NormalizeDto(dto);

            // 1. Basic DTO validations
            ValidateRequiredFields(dto, response);
            if (response.HasErrors)
            {
                return ResponseDTO<BeneficiaryResultDto>.Fail("Validation failed.", response.Errors);
            }
            string dobError = "";
            // 2. Parse and validate DOB
            //if (dto.DateOfBirth.Length==4 || (!TryParseDob(dto.DateOfBirth, out var dob, out dobError)))
            //{
            //    response.AddError(nameof(dto.DateOfBirth), dobError ?? "Invalid DateOfBirth format.");
            //    return ResponseDTO<BeneficiaryResultDto>.Fail("Validation failed.", response.Errors);
            //}

            var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);
            var age = CalculateAge(dto.DateOfBirth);
            if (!age.success)
            {
                response.AddError(nameof(dto.DateOfBirth), age.error);
                return ResponseDTO<BeneficiaryResultDto>.Fail("Validation failed.", response.Errors);
            }

            // 3. Customer existence
            var customer = await _db.customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.MemberNo == dto.Memberno)
                .ConfigureAwait(false);


            if (customer == null)
            {
                return ResponseDTO<BeneficiaryResultDto>.Fail("Customer not found.");
            }

            // 4. Guardian rules
            var guardianRequired = age.age < 18;
            if (guardianRequired && dto.Guardian == null)
            {
                response.AddError(nameof(dto.Guardian), "Guardian is required for beneficiaries under 18.");
                return ResponseDTO<BeneficiaryResultDto>.Fail("Validation failed.", response.Errors);
            }

            if (dto.Guardian != null)
            {
                ValidateGuardian(dto.Guardian, response);
                if (response.HasErrors)
                {
                   
                    return ResponseDTO<BeneficiaryResultDto>.Fail("Validation failed.", response.Errors);
                }
            }
 var existingTotalForCustomer = await _db.Beneficiaries
                            .Where(b => b.CustomerId == customer.Id)
                            .SumAsync(b => (decimal?)b.Percentage)
                            .ConfigureAwait(false) ?? 0m;

                        var combinedPercentage = existingTotalForCustomer + dto.Percentage;
                        if (combinedPercentage > 100m)
                        {
                            return ResponseDTO<BeneficiaryResultDto>.Fail("Total beneficiary percentage cannot exceed 100.");
                        }
            // SERIALIZABLE transaction inside execution strategy (required when using SqlServerRetryingExecutionStrategy).
            var strategy = _db.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _db.Database
                    .BeginTransactionAsync(IsolationLevel.Serializable)
                    .ConfigureAwait(false);
                string beneficiaryCode = Guid.NewGuid().ToString();
                try
                {
                    Beneficiaries beneficiary;
                    GuardianCreateDTO? guardianEntity;
                    var isUpdate = !string.IsNullOrEmpty(dto.BeneficiaryCode);
                    // Check if the same MemberNo has a beneficiary with the same IdNumber
                    if (!string.IsNullOrWhiteSpace(dto.IdNumber))
                    {
                        var existingBeneficiary = await _db.Beneficiaries
                            .FirstOrDefaultAsync(b => b.MemberNo == dto.Memberno && b.IdNumber == dto.IdNumber)
                            .ConfigureAwait(false);

                        if (existingBeneficiary != null)
                        {
                            isUpdate = true;
                            dto.BeneficiaryCode = existingBeneficiary.PartnerBeneficiaryCode;
                        }
                    }


                    if (isUpdate)
                    {
                        beneficiary = await _db.Beneficiaries
                            .Include(b => b.Guardian)
                            .FirstOrDefaultAsync(b => b.PartnerBeneficiaryCode == dto.BeneficiaryCode && b.MemberNo==dto.Memberno)
                            .ConfigureAwait(false)
                            ?? throw new InvalidOperationException("Beneficiary not found for update.");

                        if (!string.Equals(beneficiary.CustomerId, customer.Id, StringComparison.Ordinal))
                        {
                            return ResponseDTO<BeneficiaryResultDto>.Fail("Cannot change CustomerId for an existing beneficiary.");
                        }

                        var existingTotalForCustomer = await _db.Beneficiaries
                            .Where(b => b.CustomerId == customer.Id && b.Id != beneficiary.Id)
                            .SumAsync(b => (decimal?)b.Percentage)
                            .ConfigureAwait(false) ?? 0m;

                        var combinedPercentage = existingTotalForCustomer + dto.Percentage;
                        if (combinedPercentage > 100m)
                        {
                            response.AddError(nameof(dto.Guardian), "Total beneficiary percentage cannot exceed 100.");
                        return ResponseDTO<BeneficiaryResultDto>.Fail("Validation failed.", response.Errors);
                           
                        }

                        beneficiary.Firstname = dto.Firstname;
                        beneficiary.OtherNames = dto.OtherNames;
                        beneficiary.Relationship = dto.Relationship;
                        beneficiary.Percentage = dto.Percentage;
                        beneficiary.Date_of_birthRaw = dto.DateOfBirth;
                        beneficiary.Date_of_birth = dto.DateOfBirth;

                        guardianEntity = await UpsertGuardianAsync(dto.Guardian, beneficiary, userId).ConfigureAwait(false);
                    }
                    else
                    {
                        var existingTotalForCustomer = await _db.Beneficiaries
                            .Where(b => b.CustomerId == customer.Id)
                            .SumAsync(b => (decimal?)b.Percentage)
                            .ConfigureAwait(false) ?? 0m;

                        var combinedPercentage = existingTotalForCustomer + dto.Percentage;
                        if (combinedPercentage > 100m)
                        {
                             response.AddError(nameof(dto.Guardian), "Total beneficiary percentage cannot exceed 100.");
                        return ResponseDTO<BeneficiaryResultDto>.Fail("Validation failed.", response.Errors);
                        }
                        // Validate if the same IdNumber exists for the same MemberNo or CustomerId
                        if (!string.IsNullOrWhiteSpace(dto.IdNumber))
                        {
                            var idExists = await _db.Beneficiaries
                                .AnyAsync(b =>
                                    b.IdNumber == dto.IdNumber &&
                                    (b.MemberNo == dto.Memberno)
                                ).ConfigureAwait(false);

                            if (idExists)
                            {
                                 response.AddError(nameof(dto.Guardian), "A beneficiary with the same ID number already exists for this member or customer.");
                        return ResponseDTO<BeneficiaryResultDto>.Fail("Validation failed.", response.Errors);
                              
                            }
                        }


                        beneficiary = new Beneficiaries
                        {
                            CustomerId = customer.Id,
                            MemberNo = dto.Memberno,
                            Firstname = dto.Firstname,
                            OtherNames = dto.OtherNames,
                            Relationship = dto.Relationship,
                            PartnerBeneficiaryCode = beneficiaryCode,
                            Percentage = dto.Percentage,
                            Date_of_birthRaw = dto.DateOfBirth,
                            Date_of_birth = dto.DateOfBirth,
                            Gender = dto.Gender,
                            IdNumber = dto.IdNumber,
                            Age = age.age,
                            CreatedDate = DateTime.UtcNow,
                            CreatedBy = userId.ToString(),
                            PartnerCode = partnerCode
                        };

                        await _db.Beneficiaries.AddAsync(beneficiary).ConfigureAwait(false);

                        guardianEntity = await UpsertGuardianAsync(dto.Guardian, beneficiary, userId).ConfigureAwait(false);
                    }

                    await _db.SaveChangesAsync().ConfigureAwait(false);
                    await transaction.CommitAsync().ConfigureAwait(false);

                    var result = new BeneficiaryResultDto
                    {
                        BeneficiaryCode = beneficiaryCode,
                        CustomerId = beneficiary.CustomerId,
                        FullName = BuildFullName(beneficiary.Firstname, beneficiary.OtherNames),
                        Percentage = beneficiary.Percentage ?? 0,
                        GuardianSaved = guardianEntity != null
                    };

                    return ResponseDTO<BeneficiaryResultDto>.Ok(result, isUpdate ? "Beneficiary updated successfully." : "Beneficiary created successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to add/update beneficiary for customer {CustomerId}.", dto.Memberno);
                    try
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                    }
                    catch (Exception rollbackEx)
                    {
                        _logger.LogError(rollbackEx, "Failed to rollback transaction for beneficiary operation.");
                    }

                    return ResponseDTO<BeneficiaryResultDto>.Fail("An unexpected error occurred while saving the beneficiary.");
                }
            }).ConfigureAwait(false);
        }

        #region Helper Methods

        private static void NormalizeDto(BeneficiaryCreateDTO dto)
        {
           
            dto.Firstname = Normalize(dto.Firstname);
            dto.OtherNames = Normalize(dto.OtherNames);
            dto.Relationship = Normalize(dto.Relationship);
            dto.DateOfBirth = Normalize(dto.DateOfBirth);

            if (dto.Guardian != null)
            {
                dto.Guardian.IdNumber = Normalize(dto.Guardian.IdNumber);
                dto.Guardian.PhoneNumber = Normalize(dto.Guardian.PhoneNumber);
                dto.Guardian.FirstName = Normalize(dto.Guardian.FirstName);
                dto.Guardian.OtherNames = Normalize(dto.Guardian.OtherNames);
                dto.Guardian.Relationship = Normalize(dto.Guardian.Relationship);
            }
        }

        private static string Normalize(string? value)
            => string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();

        private static void ValidateRequiredFields(BeneficiaryCreateDTO dto, ResponseDTO<BeneficiaryResultDto> response)
        {
            if (string.IsNullOrWhiteSpace(dto.Memberno) )
            {
                response.AddError(nameof(dto.Memberno), "Invalid Member No.");
            }

            if (string.IsNullOrWhiteSpace(dto.Firstname))
            {
                response.AddError(nameof(dto.Firstname), "FirstName is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.OtherNames))
            {
                response.AddError(nameof(dto.OtherNames), "OtherNames is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Relationship))
            {
                response.AddError(nameof(dto.Relationship), "Relationship is required.");
            }

            if (dto.Percentage <= 0m)
            {
                response.AddError(nameof(dto.Percentage), "Percentage must be greater than zero.");
            }

            if (string.IsNullOrWhiteSpace(dto.DateOfBirth))
            {
                response.AddError(nameof(dto.DateOfBirth), "DateOfBirth is required.");
            }
        }

        private static void ValidateGuardian(GuardianCreateDTO guardian, ResponseDTO<BeneficiaryResultDto> response)
        {
            if (string.IsNullOrWhiteSpace(guardian.IdNumber))
            {
                response.AddError(nameof(guardian.IdNumber), "Guardian IdNumber is required.");
            }
            
            var guardianage = CalculateAge(guardian.DateOfBirth);
            if (!guardianage.success)
            {
                response.AddError(nameof(guardian.DateOfBirth), guardianage.error);
            }
            if(guardianage.success && guardianage.age < 18)
            {
                response.AddError(nameof(guardian.DateOfBirth), "Guardian must be at least 18 years old.");
            }
            //if (string.IsNullOrWhiteSpace(guardian.PhoneNumber))
            //{
            //    response.AddError(nameof(guardian.PhoneNumber), "Guardian PhoneNumber is required.");
            //}

            var hasFirstName = !string.IsNullOrWhiteSpace(guardian.FirstName);
            var hasOtherNames = !string.IsNullOrWhiteSpace(guardian.OtherNames);
            if (!hasFirstName && !hasOtherNames)
            {
                response.AddError(nameof(guardian.FirstName), "Guardian must have at least a first name or other names.");
            }

            if (string.IsNullOrWhiteSpace(guardian.Relationship))
            {
                response.AddError(nameof(guardian.Relationship), "Guardian Relationship is required.");
            }
        }

        private static bool TryParseDob(string raw, out DateOnly? dob, out string? error)
        {
            dob = null;
            error = null;

            if (string.IsNullOrWhiteSpace(raw))
            {
                error = "DateOfBirth is required.";
                return false;
            }

            raw = raw.Trim();

            // Try invariant culture first
            if (DateTime.TryParseExact(raw, DateFormats,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var parsed))
            {
                dob = DateOnly.FromDateTime(parsed.Date);
                if (dob.Value > DateOnly.FromDateTime(DateTime.UtcNow.Date))
                {
                    error = "DateOfBirth cannot be in the future.";
                    dob = null;
                    return false;
                }

                return true;
            }

            // Fallback to en-US parsing
            if (DateTime.TryParseExact(raw, DateFormats,
                    new CultureInfo("en-US"),
                    DateTimeStyles.None,
                    out parsed))
            {
                dob = DateOnly.FromDateTime(parsed.Date);
                if (dob.Value > DateOnly.FromDateTime(DateTime.UtcNow.Date))
                {
                    error = "DateOfBirth cannot be in the future.";
                    dob = null;
                    return false;
                }

                return true;
            }

            error = "Invalid DateOfBirth format.";
            return false;
        }

        private static (bool success, int age, string? error) CalculateAgeDateTime(string? dateOfBirth)
        {
            if (string.IsNullOrWhiteSpace(dateOfBirth))
            {
                return (false, 0, "Date of birth is required.");
            }

            if (!TryParseDob(dateOfBirth, out var dob, out var error))
            {
                return (false, 0, error);
            }

            var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);
            var age = today.Year - dob.Value.Year;
            if (today < dob.Value.AddYears(age))
            {
                age--;
            }

            if (age < 0)
            {
                return (false, 0, "Date of birth cannot be in the future.");
            }

            return (true, age, null);
        }

        private static string BuildFullName(string firstName, string otherNames)
        {
            if (string.IsNullOrWhiteSpace(otherNames))
                return firstName;

            return $"{firstName} {otherNames}".Trim();
        }

        private async Task<GuardianCreateDTO?> UpsertGuardianAsync(GuardianCreateDTO? guardianDto, Beneficiaries beneficiary, Guid claim)
        {
            if (guardianDto == null)
            {
                // If guardian not provided, we leave existing guardian as-is.
                return null;
            }
            if (string.IsNullOrWhiteSpace(guardianDto.IdNumber))
                {
                    throw new ArgumentException("Guardian IdNumber is required.");
                }
                if (string.IsNullOrWhiteSpace(guardianDto.Relationship))
                {
                    throw new ArgumentException("Guardian Relationship is required.");
                }
                if (guardianDto.DateOfBirth == null)
                {
                    throw new ArgumentException("Guardian DateOfBirth is required to determine age.");
                }
                if (!DateTime.TryParse(guardianDto.DateOfBirth.ToString(), out var guardianDob))
                {
                    throw new ArgumentException("Guardian DateOfBirth format is invalid.");
                }
                var age = CalculateAge(guardianDto.DateOfBirth);

                if (!age.success)
                {
                    throw new ArgumentException("Guardian must be at least 18 years old.");
                }
            GuardianDTO guardianEntity;

            if (beneficiary.Guardian != null)
            {
                guardianEntity = guardianDto.Adapt<GuardianDTO>();
                // Validate Guardian data: ensure age > 18, IdNumber is provided, and Relationship is provided
                

                guardianEntity.IdNumber = guardianDto.IdNumber;
                guardianEntity.PhoneNumber = guardianDto.PhoneNumber;
                guardianEntity.FirstName = guardianDto.FirstName;
                guardianEntity.OtherNames = guardianDto.OtherNames;
                guardianEntity.Relationship = guardianDto.Relationship;
                guardianEntity.DateOfBirth = guardianDto.DateOfBirth;
                guardianEntity.BeneficiaryId = beneficiary.Id;
                guardianEntity.Email = guardianDto.Email;
                guardianEntity.IdType = guardianDto.IdType;
                guardianEntity.Surname = guardianDto.Surname;
                guardianEntity.Gender = guardianDto.Gender;
                guardianEntity.CustomerId =string.IsNullOrEmpty(beneficiary.CustomerId)? Guid.Empty: Guid.Parse(beneficiary.CustomerId);
                
                // We do not change CreatedOn for updates.
            }
            else
            {
                guardianEntity = new GuardianDTO
                {
                       BeneficiaryId= beneficiary.Id,
                    IdNumber = guardianDto.IdNumber,
                    PhoneNumber = guardianDto.PhoneNumber,
                    FirstName = guardianDto.FirstName,
                    OtherNames = guardianDto.OtherNames,
                    Relationship = guardianDto.Relationship,
                 DateOfBirth = guardianDto.DateOfBirth,
                 Gender = guardianDto.Gender,
                Email = guardianDto.Email,
                IdType = guardianDto.IdType,
                Surname = guardianDto.Surname
                };

                var guardian = guardianEntity.Adapt<Guardian>();
                await _db.Guardians.AddAsync(guardian).ConfigureAwait(false);
                beneficiary.Guardian = guardian;
            }

            // Ensure DTO is updated with correct BeneficiaryId
            guardianDto.BeneficiaryCode = beneficiary.PartnerBeneficiaryCode??"";

            return guardianEntity.Adapt<GuardianCreateDTO>();
        }

        #endregion
    }
}
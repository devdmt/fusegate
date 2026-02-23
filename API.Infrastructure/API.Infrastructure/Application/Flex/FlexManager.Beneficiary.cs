
using API.Infrastructure.Common.Exceptions;
using DAL.Model;
using DAL.ModelView;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Office.Interop.Excel;
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

        private void NormalizeDto(BeneficiaryCreateDTO dto)
        {
            if (dto == null) return;

            if (!string.IsNullOrWhiteSpace(dto.FullName))
                dto.FullName = dto.FullName.Trim();

            if (!string.IsNullOrWhiteSpace(dto.DateOfBirth))
                dto.DateOfBirth = dto.DateOfBirth.Trim();

            if (!string.IsNullOrWhiteSpace(dto.Memberno))
                dto.Memberno = dto.Memberno.Trim();
            if (!string.IsNullOrWhiteSpace(dto.Firstname))
                dto.Firstname = dto.Firstname.Trim();
            if (!string.IsNullOrWhiteSpace(dto.OtherNames))
                dto.OtherNames = dto.OtherNames.Trim();
            if (!string.IsNullOrWhiteSpace(dto.Surname))
                dto.Surname = dto.Surname.Trim();
            if (!string.IsNullOrWhiteSpace(dto.EmailAddress))
                dto.EmailAddress = dto.EmailAddress.Trim();
            if (!string.IsNullOrWhiteSpace(dto.Phone))
                dto.Phone = dto.Phone.Trim();
            if (!string.IsNullOrWhiteSpace(dto.Relationship))
                dto.Relationship = dto.Relationship.Trim();
            if (!string.IsNullOrWhiteSpace(dto.IdNumber))
                dto.IdNumber = dto.IdNumber.Trim();
            if (!string.IsNullOrWhiteSpace(dto.DateOfBirth))
                dto.DateOfBirth = dto.DateOfBirth.Trim();
            if (dto.Percentage != null)
                dto.Percentage = dto.Percentage.Value;
            if (dto.Gender != null)
                dto.Gender = dto.Gender.Value;
        }

        private void ValidateRequiredFields(BeneficiaryCreateDTO dto, ResponseDTO<BeneficiaryResultDto> response)
        {
            if (string.IsNullOrWhiteSpace(dto.Memberno))
                response.AddError(nameof(dto.Memberno), "Memberno is required.");
            if (string.IsNullOrWhiteSpace(dto.Firstname))
                response.AddError(nameof(dto.Firstname), "Firstname is required.");
            if (string.IsNullOrWhiteSpace(dto.OtherNames))
                response.AddError(nameof(dto.OtherNames), "OtherNames is required.");
            if (string.IsNullOrWhiteSpace(dto.Relationship))
                response.AddError(nameof(dto.Relationship), "Relationship is required.");
            if (string.IsNullOrWhiteSpace(dto.IdNumber))
                response.AddError(nameof(dto.IdNumber), "IdNumber is required.");
            if (string.IsNullOrWhiteSpace(dto.DateOfBirth))
                response.AddError(nameof(dto.DateOfBirth), "DateOfBirth is required.");
            if (dto.Percentage <= 0)
                response.AddError(nameof(dto.Percentage), "Percentage must be greater than zero.");
        }
     

        public async Task<ResponseDTO<BeneficiaryResultDto>> AddBeneficiaryAsync(BeneficiaryCreateDTO dto, Guid userId,string partnerCode)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (userId==Guid.Empty) throw new UnauthorizedException("unauthorised");

            var response = new ResponseDTO<BeneficiaryResultDto>();
            var errors = new List<ValidationErrorItem>();
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
                

        }

       
    }
}
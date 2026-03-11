using DAL.Model;
using DAL.ModelView;
using DAL.ModelView.Pension;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Infrastructure.Application.Flex
{
    internal partial class FlexManager
    {
        public async Task<ProductDTO> GetMyProducts(string Partner)
        {
            var response= new ProductDTO();

             var partnersProducts= await _db.GetPartnerProductsAsync(Partner);
            if(partnersProducts != null)
                {
                response.Id = partnersProducts.Id;
                response.Name = partnersProducts.Name;
                response.Description = partnersProducts.Description;
           
                response.Image = partnersProducts.Image;
                
            }
            return response;
        }
        public async Task<OnboardResponse> OnBoarding(CustomerBIODTO customer, string PartnerId)
        {
            try
            {
                // Assume _db is the database context injected in the constructor.  

                // Check if an existing customer for the same IDNumber (but not this partner/product)  
                var partnersProducts= await _db.GetPartnerProductsAsync(Productenum.flex,PartnerId);
             
                var customerProduct= await _db.customerProducts.Where(c=>c.Product==(int)Productenum.flex).Select(c=>c.CustomerId).ToListAsync();
                if (!string.IsNullOrWhiteSpace(customer.Email))
                {
                    try
                    {
                        var addr = new System.Net.Mail.MailAddress(customer.Email);
                        if (addr.Address != customer.Email)
                        {
                            return new OnboardResponse
                            {
                                Success = false,
                                ErrorMsg = "Invalid email address format."
                            };
                        }
                    }
                    catch
                    {
                        return new OnboardResponse
                        {
                            Success = false,
                            ErrorMsg = "Invalid email address format."
                        };
                    }
                }
                // Validate ID Number to international standard (ISO/IEC 7812 - usually numeric, length varies by country)
                if (!string.IsNullOrWhiteSpace(customer.IDNumber))
                {
                    // Example: For illustrative purposes, assume international standard accepts only digits, length between 6 and 20
                    var trimmedId = customer.IDNumber.Trim();
                    if (!trimmedId.All(char.IsDigit) || trimmedId.Length < 6 || trimmedId.Length > 20)
                    {
                        return new OnboardResponse
                        {
                            Success = false,
                            ErrorMsg = "Invalid ID Number format. Please enter a valid international/national ID (numeric, 6-20 digits)."
                        };
                    }
                }

                // Optionally, phone number validation to E.164 (international) format: starts with +, 10-15 digits after
                if (!string.IsNullOrWhiteSpace(customer.PhoneNumber))
                {
                    var digits = new string(customer.PhoneNumber.Where(char.IsDigit).ToArray());
                    if (digits.Length < 10 || digits.Length > 15)
                    {
                        return new OnboardResponse
                        {
                            Success = false,
                            ErrorMsg = "Invalid phone number format. Use international format (10-15 digits, include country code)."
                        };
                    }
                }


                
                if(partnersProducts == null)
                {
                    return new OnboardResponse
                    {
                        Success = false,
                        ErrorMsg = "No active product found for your account."
                    };
                }
                   var customerWithIdNumber = await _db.customers
                    .Where(c => c.IDNumber == customer.IDNumber && c.PartnerId== partnersProducts.PartnerId.ToString() && c.ProductId.ToString()==partnersProducts.ProductId.ToString())
                    .FirstOrDefaultAsync();
                if (customerWithIdNumber != null)
                {
                    // Check if a customer with matching PartnerId, ProductId and IDNumber exists  
                    var existingCustomer = await _db.Connection.ExecuteScalarAsync<int>(
                        "SELECT COUNT(1) FROM [dbo].[Customers] WHERE [PartnerId] = @PartnerId AND [ProductId] = @ProductId AND [IDNumber] = @IDNumber",
                        new { PartnerId = PartnerId, ProductId = partnersProducts.Id, IDNumber = customer.IDNumber }
                    ) > 0;

                    if (existingCustomer)
                    {
                        // Already exists for this partner/product  
                        return new OnboardResponse
                        {
                            Success = false,
                            ErrorMsg = "Customer details already exist for this partner and product."
                        };
                    }
                    // Update the customer record for new partner/product  
                    customerWithIdNumber.PartnerId = partnersProducts.PartnerId.ToString();
                    customerWithIdNumber.ProductId = Convert.ToInt32(partnersProducts.Id);
                    customerWithIdNumber.Firstname = customer.FirstName;
                    customerWithIdNumber.DateOfBirth = customer.DateOfBirth;
                    customerWithIdNumber.Gender = customer.Gender;
                    customerWithIdNumber.Email = customer.Email;
                    customerWithIdNumber.PhoneNumber = customer.PhoneNumber;
                    customerWithIdNumber.Occupation = customer.Occupation;
                    customerWithIdNumber.Nationality = customer.Nationality;
                    customerWithIdNumber.Residency = customer.Residency;
                    customerWithIdNumber.RequestDate = DateTime.Now.ToString("yyyy-MM-dd");
                    customerWithIdNumber.Processed = false;
                    customerWithIdNumber.Status = "Pending";
                    customerWithIdNumber.CreatedOn = DateTime.Now;
                    customerWithIdNumber.Validated = false;
                    customerWithIdNumber.OtherNames = customer.OtherNames;
                    customerWithIdNumber.Fullname = customer.FirstName +" "+ customer.OtherNames;
                    customerWithIdNumber.IdType = customer.IdType;
                    customerWithIdNumber.EmploymentTerms = customer.EmploymentTerms;
                    customerWithIdNumber.TaxIdNumber = customer.TaxIdNumber;
                    customerWithIdNumber.USAddress = customer.USAddress;
                    customerWithIdNumber.EmployerName = customer.EmployerName;
                    customerWithIdNumber.BusinessName = customer.BusinessName;
                    customerWithIdNumber.NatureOfBusiness = customer.NatureOfBusiness;
                    customerWithIdNumber.RoleInBusiness = customer.RoleInBusiness;
                    customerWithIdNumber.Role = customer.Role;
                    customerWithIdNumber.SourceOfIncome = customer.SourceOfIncome;
                    customerWithIdNumber.AdditionalSourceOfIncome = customer.AdditionalSourceOfIncome;
                    customerWithIdNumber.AverageIncome = customer.AverageIncome;
                    customerWithIdNumber.ValidationErrorCount = 0;
                    customerWithIdNumber.RetryCount = 0;
                    customerWithIdNumber.ValidationErrors = null;
                    customerWithIdNumber.ValidationDate = null;
                    customerWithIdNumber.Modified = true;
                    customerWithIdNumber.LastModified = DateTime.Now;
                    customerWithIdNumber.AdditionalSourceOfIncome = customer.AdditionalSourceOfIncome;

                    await _db.SaveChangesAsync();
                    await InsertCustomerProductAsync(customerWithIdNumber.Id,(int)Productenum.flex,Guid.NewGuid().ToString().Replace("-",""),0);
                    return new OnboardResponse { Success = true, ErrorMsg = "Customer information updated successfully." };
                }
                else
                {

                     string generatedMemberNo;
                   
                        
                        do
                        {
                            generatedMemberNo = DateTime.Now.ToString("yy") + "-" + _isettings.GenerateRadomCode(7);
                            var existing = await _db.Connection.QueryFirstOrDefaultAsync<string>(
                                "SELECT MemberNo FROM customers WHERE MemberNo = @MemberNo", 
                                new { MemberNo = generatedMemberNo });
                            if (existing == null)
                            {
                                break;
                            }
                        } while (true);
                    
                    // New customer, add to database  
                    var newCustomer = new DAL.Model.Customers
                    {
                        Id = Guid.NewGuid().ToString(),
                        PartnerId = partnersProducts.PartnerId.ToString(),
                        ProductId = partnersProducts.ProductId ?? 0,
                        Firstname = customer.FirstName,
                        DateOfBirth = customer.DateOfBirth,
                        IDNumber = customer.IDNumber,
                        Gender = customer.Gender,
                        Email = customer.Email,
                        PhoneNumber = customer.PhoneNumber,
                        Occupation = customer.Occupation,
                        Nationality = customer.Nationality,
                        Residency = customer.Residency,
                        RequestDate = DateTime.Now.ToString("yyyy-MM-dd"),
                        Processed = false,
                        Status = "Pending",
                        CreatedOn = DateTime.Now,
                        Validated = false,
                        Fullname = customer.FirstName + " " + customer.OtherNames,
                        OtherNames = customer.OtherNames ?? string.Empty,
                        IdType = customer.IdType,
                        EmploymentTerms = customer.EmploymentTerms,
                        TaxIdNumber = customer.TaxIdNumber,
                        USAddress = customer.USAddress,
                        EmployerName = customer.EmployerName,
                        Pin = customer.Pin,
                        Role = customer.Role,
                        SourceOfIncome = customer.SourceOfIncome,
                        AverageIncome = customer.AverageIncome,
                        ValidationErrorCount = 0,
                        RetryCount = 0,
                        ValidationErrors = null,
                        ValidationDate = null,
                        Modified = false,
                        LastModified = DateTime.Now,
                        
                        AdditionalSourceOfIncome = customer.AdditionalSourceOfIncome,
                        MemberNo =generatedMemberNo,// DateTime.Now.ToString("yy") + "-" + _isettings.GenerateRadomCode(6),
                        Complete = true
                    };

                    _db.customers.Add(newCustomer);
                    await _db.SaveChangesAsync();
                    string trnId = Guid.NewGuid().ToString().Replace("-", "");
                    await InsertCustomerProductAsync(newCustomer.Id,(int)Productenum.flex,trnId,0);
                    return new OnboardResponse { 
                        Success = true,
                        ErrorMsg = "Customer added successfully.",
                        MemberNo=newCustomer.MemberNo, 
                        ProductRef=trnId
                    };
                }
            }
            catch (Exception ex)
            {
                _isettings.LogRequests($"Error in OnBoarding: {ex.Message}","OnBoarding", Interface.RequestType.Error);
                // Log exception and return error response  
                return new OnboardResponse { Success = false, ErrorMsg = ex.Message };
            }
        }

    public async Task<ResponseDTO> InsertCustomerProductAsync(
        string customerId,
        int product,
        string refNo,     
        int? groupId =0
     )
    {
        try
        {
            var customerProduct = new CustomerProduct
            {
                CustomerId = customerId,
                Product = product,
                RefNo = refNo,
                Complete = false,
                Processed = false,
                IsPicked = false,
                MailSentOn = null,
                RequestCreatedOn = DateTime.Now,
                AgentId = null,
                PaymentComplete = false,
                PaymentCompletedOn = null,
                RequestSource = 0,
                Filelocation = null,
                Filebytes = null,
                GroupId = groupId
            };

            _db.customerProducts.Add(customerProduct);
            await _db.SaveChangesAsync();

            return new ResponseDTO { Success = true, ErrorMsg = "Customer product inserted successfully." };
        }
        catch (Exception ex)
        {
            _isettings.LogRequests($"Error in InsertCustomerProductAsync: {ex.Message}", "InsertCustomerProductAsync", Interface.RequestType.Error);
            return new ResponseDTO { Success = false, ErrorMsg = ex.Message };
        }
    }
    }



}

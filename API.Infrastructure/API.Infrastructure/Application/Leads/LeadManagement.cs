using API.Infrastructure.Interface;
using DAL;
using DAL.Model;
using DAL.ModelView;
using System;
using System.Threading.Tasks;

namespace API.Infrastructure.Application.Leads
{
    public class LeadManagement : ILeads
    {
        private readonly ApplicationDbContext _db;

        public LeadManagement(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<ResponseDTO> AddLeads(CustomerApiLeads customerLeads, string PartnerCode)
        {
            try
            {
                if (customerLeads == null)
                    return ResponseDTO.Fail("Lead payload is required.");

                if (string.IsNullOrWhiteSpace(customerLeads.Phone))
                {
                    var r = ResponseDTO.Fail("Validation failed.");
                    r.AddError(nameof(customerLeads.Phone), "Phone is required.");
                    return r;
                }

                if (string.IsNullOrWhiteSpace(customerLeads.Names))
                {
                    var r = ResponseDTO.Fail("Validation failed.");
                    r.AddError(nameof(customerLeads.Names), "Names are required.");
                    return r;
                }

                if (string.IsNullOrWhiteSpace(PartnerCode))
                {
                    var r = ResponseDTO.Fail("Validation failed.");
                    r.AddError(nameof(PartnerCode), "Partner code is required.");
                    return r;
                }

                var partner = await _db.GetPartnerAsync(PartnerCode);
                if (partner == null)
                {
                    var r = ResponseDTO.Fail("Validation failed.");
                    r.AddError(nameof(PartnerCode), "Partner not found.");
                    return r;
                }

                if (customerLeads.Id == Guid.Empty)
                    customerLeads.Id = Guid.NewGuid();

                customerLeads.CreatedOn ??= DateTime.UtcNow;
                customerLeads.PartnerCode = partner.PartnerCode;
                customerLeads.PartnerName = partner.PartnerName?.Trim();
                _db.CustomerApiLeads.Add(customerLeads);
                await _db.SaveChangesAsync();

                return ResponseDTO.Ok();
            }
            catch (Exception)
            {
                return ResponseDTO.Fail("Failed to add lead.");
            }
        }
    }
}

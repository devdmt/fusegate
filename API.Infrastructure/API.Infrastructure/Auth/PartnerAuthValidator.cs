using API.Infrastructure.Interface;
using DAL;
using DAL.Core.Interface;
using DAL.ModelView;
using DAL.ModelView.FlexiFuturePlus;

namespace API.Infrastructure.Auth;

public interface IPartnerAuthValidator : ITransientService
{
    Task<PartnerAuthResult> ValidateAsync(
        string? jwtPartnerCode,
        string? headerPartnerCode,
        string actionName,
        CancellationToken cancellationToken = default);
}

public sealed class PartnerAuthValidator : IPartnerAuthValidator
{
    private readonly ApplicationDbContext _db;
    private readonly Isettings _settings;

    public PartnerAuthValidator(ApplicationDbContext db, Isettings settings)
    {
        _db = db;
        _settings = settings;
    }

    public async Task<PartnerAuthResult> ValidateAsync(
        string? jwtPartnerCode,
        string? headerPartnerCode,
        string actionName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (jwtPartnerCode != headerPartnerCode)
            {
                _settings.LogRequests(
                    $"partner {jwtPartnerCode} --pcode {headerPartnerCode}",
                    actionName,
                    RequestType.Info);
                return new PartnerAuthResult
                {
                    Success = false,
                    ErrorMessage = "Invalid partner code",
                    IsInvalidPartnerCode = true
                };
            }

            var partner = await _db.GetPartnerAsync(headerPartnerCode!);
            if (partner == null)
            {
                return new PartnerAuthResult
                {
                    Success = false,
                    ErrorMessage = "Partner not found."
                };
            }

            return new PartnerAuthResult { Success = true, Partner = partner };
        }
        catch (Exception ex)
        {
            _settings.LogRequests(ex.Message, actionName, RequestType.Error);
            return new PartnerAuthResult { Success = false, ErrorMessage = "Unable to validate partner." };
        }
    }
}

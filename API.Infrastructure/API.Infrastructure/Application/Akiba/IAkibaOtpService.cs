using DAL.Core.Interface;

namespace API.Infrastructure.Application.Akiba;

public interface IAkibaOtpService : ITransientService
{
    /// <summary>
    /// Queues an OTP in Akiba <c>dbo.OTPs</c> (same behavior as PensionCore <c>CustomerService.AddOTP</c>).
    /// </summary>
    Task<Guid> AddOtpAsync(AkibaOtpAddRequest request, int expiresInMinutes = 10, CancellationToken cancellationToken = default);
}

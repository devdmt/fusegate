using DAL;
using DAL.Model;
using DAL.Model.Akiba;
using DAL.Model.Pensioner;

namespace API.Infrastructure.Application.Akiba;

public sealed class AkibaOtpService : IAkibaOtpService
{
    private readonly AkibappDbContext _akiba;

    public AkibaOtpService(AkibappDbContext akiba)
    {
        _akiba = akiba;
    }

    public async Task<Guid> AddOtpAsync(
        AkibaOtpAddRequest request,
        int expiresInMinutes = 10,
        CancellationToken cancellationToken = default)
    {
        var otp = new AkibaOtp
        {
            Id = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            Code = GenerateRandomAlphanumericCode(6),
            ExpiredAt = DateTime.Now.AddMinutes(expiresInMinutes),
            CreatedAt = DateTime.Now,
            UserId = request.UserId,
            CustomerType = CustomerType.Individual
        };

        switch (request.CommType)
        {
            case AkibaOtpCommType.SMS:
                otp.ISSMSSent = false;
                otp.notificationType = NotificationType.SMS;
                otp.isEmailPicked = true;
                otp.IsEmailSent = true;
                break;
            case AkibaOtpCommType.Email:
                otp.IsEmailSent = false;
                otp.notificationType = NotificationType.Email;
                otp.ISSMSSent = true;
                break;
            case AkibaOtpCommType.All:
                otp.ISSMSSent = false;
                otp.IsEmailSent = false;
                otp.notificationType = NotificationType.ALL;
                break;
        }

        otp.EmailTemplateType = 0;
        otp.ISsent = false;
        otp.Message = string.Format(
            "Hey {0}. Your OTP code is {1} and it expires in {2} minutes.Please do not share it with anyone.",
            request.Firstname,
            otp.Code,
            expiresInMinutes);
        otp.EmailDisplayName = request.EmailDisplayName ?? "Akiba Plus Team";

        var placeholders = new Dictionary<string, string>
        {
            { "{{otp}}", otp.Code },
            { "{{date}}", DateTime.Now.ToString("dd MMM yyyy") },
            { "{{firstname}}", request.Firstname },
            { "{{expire}} ", expiresInMinutes.ToString() }
        };
        otp.EmailPlaceHolder = string.Join(", ", placeholders.Select(kvp => $"{kvp.Key}: {kvp.Value}"));

        _akiba.Otp.Add(otp);
        await _akiba.SaveChangesAsync(cancellationToken);
        return otp.Id;
    }

    private static string GenerateRandomAlphanumericCode(int length)
    {
        const string chars = "ABCDEFGHIJKLMNPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNPQRSTUVWXYZ";
        var random = new Random();
        var codeChars = new char[length];
        for (var i = 0; i < length; i++)
            codeChars[i] = chars[random.Next(chars.Length)];
        return new string(codeChars);
    }
}

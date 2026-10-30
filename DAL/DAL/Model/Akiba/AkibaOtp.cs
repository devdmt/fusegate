using DAL.Model;
using DAL.Model.Pensioner;

namespace DAL.Model.Akiba;

/// <summary>
/// Akiba Plus <c>dbo.OTPs</c> row shape (aligned with PensionCore <see cref="DAL.Models.Customers.OTP"/>).
/// </summary>
public class AkibaOtp
{
    public Guid Id { get; set; }
    public Guid? CustomerId { get; set; }
    public string? UserId { get; set; }
    public Guid? GroupId { get; set; }
    public CustomerType CustomerType { get; set; } = CustomerType.Individual;
    public string Code { get; set; } = default!;
    public string? Message { get; set; }
    public string? EmailPlaceHolder { get; set; }
    public bool? ISSMSSent { get; set; } = false;
    public bool? IsEmailSent { get; set; } = false;
    public bool? isEmailPicked { get; set; } = false;
    public bool? IsUsed { get; set; } = false;
    public bool? ISsent { get; set; } = false;
    public bool? LinkGenerated { get; set; } = false;
    public DateTime? CreatedAt { get; set; }
    public DateTime? ExpiredAt { get; set; }
    public DateTime? UsedAt { get; set; }
    public string? UsedBy { get; set; }
    public NotificationType notificationType { get; set; }
    public string? Response { get; set; }
    public string? ErrorMessage { get; set; }
    public string? LinkCode { get; set; }
    public int? SendTrial { get; set; } = 0;
    public string? EmailDisplayName { get; set; }
    public int? EmailTemplateType { get; set; } = 0;
}

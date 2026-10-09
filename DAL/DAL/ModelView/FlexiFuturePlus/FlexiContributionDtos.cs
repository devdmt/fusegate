using System.Text.Json.Serialization;
using DAL.Model;
using DAL.ModelView.Settings;
using DAL.Model.FlexiContributions;

namespace DAL.ModelView.FlexiFuturePlus;

/// <summary>
/// Create payload for a FlexiFuture contribution.
/// </summary>
/// <remarks>
/// PaymentMode carries [JsonStringEnumConverter], so callers send "Bank" / "Mpesa" /
/// "Checkoff" / "Ratiba" rather than the underlying int. It is deliberately nullable so
/// an omitted value fails the mandatory check instead of binding silently to Mpesa (0).
/// </remarks>
public class AddFlexiContributionDTO
{
    public double Total_Contribution { get; set; }
    public string? Naration { get; set; }
    public string? Reference { get; set; }
    public Guid? FlexiFuturePolicyId { get; set; }
    public string? Bank { get; set; }
    public PaymentMode? PaymentMode { get; set; }
    public string? Branch { get; set; }

    /// <summary>
    /// MSISDN used for the M-Pesa STK prompt. Required when PaymentMode is Mpesa;
    /// ignored for Bank, Checkoff and Ratiba.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Signed Checkoff mandate. Required when PaymentMode is Checkoff; ignored otherwise.
    /// </summary>
    public FileUploadDTO? Document { get; set; }
}

/// <summary>
/// Attaches the signed Checkoff/Ratiba mandate to its pending payment instruction.
/// </summary>
public class UploadPaymentInstructionDTO
{
    public Guid FlexiFuturePolicyId { get; set; }
    public FileUploadDTO? Document { get; set; }
}

/// <summary>Which table the create call wrote to.</summary>
public static class FlexiContributionRecordTypes
{
    public const string FlexiContribution = "FlexiContribution";
    public const string PaymentInstruction = "PaymentInstruction";
}

public class FlexiContributionResultDto
{
    public Guid Id { get; set; }

    /// <summary>FlexiContribution (Bank/Mpesa) or PaymentInstruction (Checkoff/Ratiba).</summary>
    public string RecordType { get; set; } = string.Empty;

    public PaymentMode PaymentMode { get; set; }

    /// <summary>
    /// sTKPushMpesaTransactions identity for the M-Pesa flow - the id the client polls
    /// CheckMpesaStkResult with. Null for every other payment mode.
    /// </summary>
    public string? StkTransactionId { get; set; }

    /// <summary>Uploaded Checkoff file name once the mandate is stored on add.</summary>
    public string? DocumentName { get; set; }
}

public class DdiFormDto
{
    /// <summary>Blank DDI/Checkoff mandate as a data-URI base64 string.</summary>
    public string Document { get; set; } = string.Empty;

    public string DocumentName { get; set; } = string.Empty;
}

public class PaymentInstructionUploadResultDto
{
    public Guid Id { get; set; }
    public string ProcessingStatus { get; set; } = string.Empty;
    public string? UploadDocumentsName { get; set; }
}

public class PaymentInstructionListDto
{
    public Guid Id { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PaymentMode? PaymentType { get; set; }

    public double Amount { get; set; }
    public string? Narration { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PaymentInstructionStatus ProcessingStatus { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PaymentStatus? PaymentStatus { get; set; }

    public DateTimeOffset? Created { get; set; }
    public Guid? PolicyId { get; set; }
    public string? PolicyNo { get; set; }
    public Guid? FundId { get; set; }
    public string? DocumentName { get; set; }
}

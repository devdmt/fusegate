namespace DAL.Model.Shared;

/// <summary>
/// Share request log shared by every quoting product. An external worker polls this
/// table to render the PDF and email it, using <see cref="Context"/> to pick the
/// right quotation template.
/// </summary>
/// <remarks>
/// <see cref="QuoteId"/> carries no foreign key: one column cannot reference two parent
/// tables, so referential integrity is enforced by the quote services, which load and
/// validate the quote before writing the share row.
/// </remarks>
public class QuotesShare
{
    public Guid Id { get; set; }
    public string Context { get; set; } = QuoteShareContexts.FlexiFutureQuote;
    public Guid QuoteId { get; set; }
    public string ToEmail { get; set; } = string.Empty;
    public bool IsGenerated { get; set; }
    public bool IsSent { get; set; }
    public string? FilePath { get; set; }
    public DateTime? SentOn { get; set; }
    public string? IpAddress { get; set; }
    public string? Browser { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public static class QuoteShareContexts
{
    public const string FlexiFutureQuote = "FlexiFutureQuote";
    public const string IddQuote = "IddQuote";
}

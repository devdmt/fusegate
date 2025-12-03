namespace API.Infrastructure.OpenApi;
public class PartnerCodeHeaderAttribute : SwaggerHeaderAttribute
{
    public PartnerCodeHeaderAttribute()
        : base(
           PartnerCode,
            "Input your Partner Code to access this API",
            string.Empty,
            true)
    {
    }
    public const string PartnerCode = "PartnerCode";
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class SwaggerHeaderAttribute : Attribute
{
    public string HeaderName { get; }
    public string? Description { get; }
    public string? DefaultValue { get; }
    public bool IsRequired { get; }

    public SwaggerHeaderAttribute(string headerName, string? description = null, string? defaultValue = null, bool isRequired = false)
    {
        HeaderName = headerName;
        Description = description;
        DefaultValue = defaultValue;
        IsRequired = isRequired;
    }
}
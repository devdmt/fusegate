namespace DAL.ModelView.FlexiFuturePlus;

public sealed class FlexiFuturePlusResponse<T> where T : class
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int StatusCode { get; set; } = 200;
    public T? Data { get; set; }

    public static FlexiFuturePlusResponse<T> Ok(T data, string? message = null, int statusCode = 200) =>
        new() { Success = true, Data = data, Message = message ?? string.Empty, StatusCode = statusCode };

    public static FlexiFuturePlusResponse<T> Fail(string message, int statusCode = 400) =>
        new() { Success = false, Message = message, StatusCode = statusCode };
}

public sealed class FlexiFuturePlusResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int StatusCode { get; set; } = 200;

    public static FlexiFuturePlusResponse Ok(string? message = null, int statusCode = 200) =>
        new() { Success = true, Message = message ?? string.Empty, StatusCode = statusCode };

    public static FlexiFuturePlusResponse Fail(string message, int statusCode = 400) =>
        new() { Success = false, Message = message, StatusCode = statusCode };
}

public sealed record PartnerContext(string PartnerCode, int PartnerId, string PartnerName);

public sealed class PartnerAuthResult
{
    public bool Success { get; init; }
    public PartnerLookupDTO? Partner { get; init; }
    public string? ErrorMessage { get; init; }
    public bool IsInvalidPartnerCode { get; init; }
}

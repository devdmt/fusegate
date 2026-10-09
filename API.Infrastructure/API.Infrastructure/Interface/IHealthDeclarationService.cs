using DAL.ModelView;
using DAL.ModelView.HealthDeclaration;

namespace API.Infrastructure.Interface;

public interface IHealthDeclarationService : ITransientService
{
    Task<ApiResult<HealthDeclarationResponseDto>> UpsertAsync(string customerId, HealthDeclarationRequestDto request, CancellationToken cancellationToken = default);
}

/// <summary>
/// Result type for health declaration operations; carries either data or validation errors.
/// </summary>
public class ApiResult<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<ValidationError>? Errors { get; set; }

    public static ApiResult<T> Ok(T data, string message = "Health declaration captured") => new()
    {
        Success = true,
        Message = message,
        Data = data
    };

    public static ApiResult<T> ValidationFailed(string message, List<ValidationError> errors) => new()
    {
        Success = false,
        Message = message,
        Errors = errors
    };
}

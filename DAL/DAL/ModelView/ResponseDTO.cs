using System.Collections.Generic;

namespace DAL.ModelView
{
    public class ValidationErrorItem
    {
        public string Field { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<ValidationError>? Errors { get; set; }
    }
    public class ValidationError
    {
        public string Field { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
    }

    /// <summary>
    /// Single validation error item (field + message) for API error responses.
    /// </summary>
    
    public class ResponseDTO<T> where T : class
    {
        public string ErrorMsg { get; set; } = string.Empty;
        public bool Success { get; set; }
        public T? Result { get; set; }
        public List<ValidationErrorItem> Errors { get; } = new List<ValidationErrorItem>();
        public bool HasErrors => Errors.Count > 0;

        public void AddError(string field, string message)
        {
            Errors.Add(new ValidationErrorItem { Field = field, Message = message });
        }   
            
        public static ResponseDTO<T> Fail(string message, IEnumerable<ValidationErrorItem>? errors = null)
        {
            var response = new ResponseDTO<T>
            {
                Success = false,
                ErrorMsg = message
            };
            if (errors != null)
            {
                response.Errors.AddRange(errors);
            }
            return response;
        }

        public static ResponseDTO<T> Ok(T result, string? message = null)
        {
            return new ResponseDTO<T>
            {
                Success = true,
                ErrorMsg = message ?? string.Empty,
                Result = result
            };
        }
    }

    public class ResponseDTO
    {
        public string ErrorMsg { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string ProductRef { get; set; } = string.Empty;
        public string ResponseId { get; set; } = string.Empty;
        public List<ValidationErrorItem> Errors { get; } = new List<ValidationErrorItem>();
        public bool HasErrors => Errors.Count > 0;

        public void AddError(string field, string message)
        {
            Errors.Add(new ValidationErrorItem { Field = field, Message = message });
        }

        public static ResponseDTO Fail(string message, IEnumerable<ValidationErrorItem>? errors = null)
        {
            var response = new ResponseDTO
            {
                Success = false,
                ErrorMsg = message
            };
            if (errors != null)
            {
                response.Errors.AddRange(errors);
            }
            return response;
        }

        public static ResponseDTO Ok(string? message = null)
        {
            return new ResponseDTO
            {
                Success = true,
                ErrorMsg = message ?? string.Empty
            };
        }
    }
  
    public class OnboardingResponseDTO
    {
        public string ErrorMsg { get; set; }
        public bool Success { get; set; }
        public string TransactionId { get; set; }
        public string ResponseId { get; set; }
        public string CustomerId { get; set; }
    }

 
}

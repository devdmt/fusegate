using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Infrastructure.Middleware;

public class UnauthorizedResponse : IActionResult
{
    private readonly string _message;

    public UnauthorizedResponse(string message = "Unauthorized")
    {
        _message = message;
    }

    public async Task ExecuteResultAsync(ActionContext context)
    {
        var error = new ErrorResult
        {
            StatusCode = StatusCodes.Status401Unauthorized,
            Exception = _message
        };
        error.Messages.Add(_message);

        var objectResult = new ObjectResult(error)
        {
            StatusCode = StatusCodes.Status401Unauthorized
        };

        await objectResult.ExecuteResultAsync(context);
    }
}



using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API.Infrastructure.Application.FlexiFuturePlus;

public static class FlexiFutureErrorHandling
{
    public const string GenericClientMessage = "An unexpected error occurred. Please try again later.";
    public const string DatabaseClientMessage = "A database error occurred. Please try again later.";

    public static bool IsDatabaseException(Exception ex)
    {
        try
        {
            for (var current = ex; current != null; current = current.InnerException)
            {
                if (current is DbUpdateException or DbException)
                    return true;
            }

            return false;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public static string ToClientMessage(Exception ex, ILogger logger, string operation)
    {
        try
        {
            if (ex is InvalidOperationException && !IsDatabaseException(ex))
                return ex.Message;

            logger.LogError(ex, "FlexiFuture {Operation} failed: {Error}", operation, ex.ToString());

            return IsDatabaseException(ex) ? DatabaseClientMessage : GenericClientMessage;
        }
        catch (Exception)
        {
            throw;
        }
    }
}

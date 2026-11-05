using DAL.ModelView.FlexiFuturePlus;
using Dapper;

namespace API.Infrastructure.Application.FlexiFuturePlus;

public partial class FlexiFuturePlusManager
{
    private async Task<FlexiFuturePlusResponse<object>> PollMpesaStkResultInternalAsync(
        string stkTransactionId,
        PartnerContext partner,
        CancellationToken cancellationToken)
    {
        var response = new FlexiFuturePlusResponse<object>();
        try
        {
            _ = partner;
            if (string.IsNullOrWhiteSpace(stkTransactionId))
                throw new InvalidOperationException("stkTransactionId is required.");

            if (!decimal.TryParse(stkTransactionId, out var transactionId))
                throw new InvalidOperationException("stkTransactionId must be a valid transaction id.");

            const string sql = """
                SELECT TOP 1
                    CAST(Id AS nvarchar(50)) AS StkTransactionId,
                    CASE WHEN ISNULL(Finalized, '0') IN ('1', 'true', 'True') THEN 1 ELSE 0 END AS Finalized,
                    CASE WHEN ISNULL(Processed, '0') IN ('1', 'true', 'True') THEN 1 ELSE 0 END AS Processed,
                    ResponseCode,
                    ResultCode,
                    ResultDesc,
                    CAST(Amount AS nvarchar(50)) AS Amount,
                    MpesaReceiptNumber,
                    ContributionTrnId
                FROM dbo.sTKPushMpesaTransactions
                WHERE Id = @TransactionId
                """;

            if (_akiba.Connection.State != System.Data.ConnectionState.Open)
                _akiba.Connection.Open();

            var result = await _akiba.Connection.QueryFirstOrDefaultAsync<FlexiFuturePlusStkPollResultDto>(
                new CommandDefinition(sql, new { TransactionId = transactionId }, cancellationToken: cancellationToken));

            if (result == null)
            {
                response.Success = false;
                response.StatusCode = 404;
                response.Message = $"STK transaction '{stkTransactionId}' was not found.";
                return response;
            }

            response.Data = result;
            var paymentFailed = result.Finalized
                && !string.IsNullOrWhiteSpace(result.ResultCode)
                && result.ResultCode is not ("0" or "00");

            if (paymentFailed)
            {
                response.Success = false;
                response.StatusCode = 400;
                response.Message = string.IsNullOrWhiteSpace(result.ResultDesc)
                    ? "The M-Pesa payment was not completed."
                    : result.ResultDesc;
                return response;
            }

            response.Success = true;
            response.Message = result.Finalized
                ? "STK transaction finalized."
                : "STK transaction is still pending.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.StatusCode = 400;
            response.Message = FlexiFutureErrorHandling.ToClientMessage(ex, _logger, nameof(PollMpesaStkResultInternalAsync));
        }

        return response;
    }
}

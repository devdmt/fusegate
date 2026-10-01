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
                    CAST(Amount AS nvarchar(50)) AS Amount,
                    MpesaReceiptNumber,
                    ContributionTrnId
                FROM dbo.sTKPushMpesaTransactions
                WHERE Id = @TransactionId
                """;

            if (_db.Connection.State != System.Data.ConnectionState.Open)
                _db.Connection.Open();

            var result = await _db.Connection.QueryFirstOrDefaultAsync<FlexiFuturePlusStkPollResultDto>(
                new CommandDefinition(sql, new { TransactionId = transactionId }, cancellationToken: cancellationToken));

            if (result == null)
                throw new InvalidOperationException($"STK transaction '{stkTransactionId}' was not found.");

            response.Success = true;
            response.Message = result.Finalized
                ? "STK transaction finalized."
                : "STK transaction is still pending.";
            response.Data = result;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = FlexiFutureErrorHandling.ToClientMessage(ex, _logger, nameof(PollMpesaStkResultInternalAsync));
        }

        return response;
    }
}

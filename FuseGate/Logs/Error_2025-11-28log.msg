module: PensionOnboardingError  Error:  Conversion failed when converting date and/or time from character string.|   at Microsoft.Data.SqlClient.SqlConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at Microsoft.Data.SqlClient.SqlInternalConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at Microsoft.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, Boolean callerHasConnectionLock, Boolean asyncClose)
   at Microsoft.Data.SqlClient.TdsParser.TryRun(RunBehavior runBehavior, SqlCommand cmdHandler, SqlDataReader dataStream, BulkCopySimpleResultSet bulkCopyHandler, TdsParserStateObject stateObj, Boolean& dataReady)
   at Microsoft.Data.SqlClient.SqlDataReader.TryConsumeMetaData()
   at Microsoft.Data.SqlClient.SqlDataReader.get_MetaData()
   at Microsoft.Data.SqlClient.SqlCommand.FinishExecuteReader(SqlDataReader ds, RunBehavior runBehavior, String resetOptionsString, Boolean isInternal, Boolean forDescribeParameterEncryption, Boolean shouldCacheForAlwaysEncrypted)
   at Microsoft.Data.SqlClient.SqlCommand.CompleteAsyncExecuteReader(Boolean isInternal, Boolean forDescribeParameterEncryption)
   at Microsoft.Data.SqlClient.SqlCommand.InternalEndExecuteReader(IAsyncResult asyncResult, Boolean isInternal, String endMethod)
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteReaderInternal(IAsyncResult asyncResult)
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteReaderAsync(IAsyncResult asyncResult)
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteReaderAsyncCallback(IAsyncResult asyncResult)
   at System.Threading.Tasks.TaskFactory`1.FromAsyncCoreLogic(IAsyncResult iar, Func`2 endFunction, Action`1 endAction, Task`1 promise, Boolean requiresSynchronization)
--- End of stack trace from previous location ---
   at Dapper.SqlMapper.ExecuteScalarImplAsync[T](IDbConnection cnn, CommandDefinition command) in /_/Dapper/SqlMapper.Async.cs:line 1210
   at API.Infrastructure.Application.Setting.AddRequest(ApiRequestsDTO apiRequestsDTO) in D:\Playground\source\DMIT\IPP\FuseGate\API.Infrastructure\API.Infrastructure\Application\Setting.cs:line 61  28/11/2025 01:09:11
module: PensionOnboardingError  Error:  Must declare the scalar variable "@Response".|   at Microsoft.Data.SqlClient.SqlConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at Microsoft.Data.SqlClient.SqlInternalConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at Microsoft.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, Boolean callerHasConnectionLock, Boolean asyncClose)
   at Microsoft.Data.SqlClient.TdsParser.TryRun(RunBehavior runBehavior, SqlCommand cmdHandler, SqlDataReader dataStream, BulkCopySimpleResultSet bulkCopyHandler, TdsParserStateObject stateObj, Boolean& dataReady)
   at Microsoft.Data.SqlClient.SqlDataReader.TryConsumeMetaData()
   at Microsoft.Data.SqlClient.SqlDataReader.get_MetaData()
   at Microsoft.Data.SqlClient.SqlCommand.FinishExecuteReader(SqlDataReader ds, RunBehavior runBehavior, String resetOptionsString, Boolean isInternal, Boolean forDescribeParameterEncryption, Boolean shouldCacheForAlwaysEncrypted)
   at Microsoft.Data.SqlClient.SqlCommand.RunExecuteReaderTds(CommandBehavior cmdBehavior, RunBehavior runBehavior, Boolean returnStream, Boolean isAsync, Int32 timeout, Task& task, Boolean asyncWrite, Boolean inRetry, SqlDataReader ds, Boolean describeParameterEncryptionRequest)
   at Microsoft.Data.SqlClient.SqlCommand.RunExecuteReader(CommandBehavior cmdBehavior, RunBehavior runBehavior, Boolean returnStream, TaskCompletionSource`1 completion, Int32 timeout, Task& task, Boolean& usedCache, Boolean asyncWrite, Boolean inRetry, String method)
   at Microsoft.Data.SqlClient.SqlCommand.RunExecuteReader(CommandBehavior cmdBehavior, RunBehavior runBehavior, Boolean returnStream, String method)
   at Microsoft.Data.SqlClient.SqlCommand.ExecuteScalar()
   at Dapper.SqlMapper.ExecuteScalarImpl[T](IDbConnection cnn, CommandDefinition& command) in /_/Dapper/SqlMapper.cs:line 2877
   at Dapper.SqlMapper.ExecuteScalar(IDbConnection cnn, String sql, Object param, IDbTransaction transaction, Nullable`1 commandTimeout, Nullable`1 commandType) in /_/Dapper/SqlMapper.cs:line 476
   at API.Infrastructure.Application.Setting.UpdateRequest(UpdateRequestsDTO request) in D:\Playground\source\DMIT\IPP\FuseGate\API.Infrastructure\API.Infrastructure\Application\Setting.cs:line 34  28/11/2025 01:09:12
module: PensionOnboardingError  Error:  Missing or invalid default region.|   at PhoneNumbers.PhoneNumberUtil.ParseHelper(String numberToParse, String defaultRegion, Boolean keepRawInput, Boolean checkRegion, PhoneNumber phoneNumber)
   at PhoneNumbers.PhoneNumberUtil.Parse(String numberToParse, String defaultRegion)
   at API.Infrastructure.Application.Pension.PensionManager.OnboardingRequest(PensionOnboardingDTO request, String Ip, String PartnerId) in D:\Playground\source\DMIT\IPP\FuseGate\API.Infrastructure\API.Infrastructure\Application\Pension\PensionManager.cs:line 45  28/11/2025 01:09:12

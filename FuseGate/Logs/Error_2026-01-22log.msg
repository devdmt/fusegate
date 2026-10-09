module: Partner code AuthenticatePartner  Error:  LATBS9Y  22/01/2026 16:47:35
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
   at API.Infrastructure.Application.Setting.AddRequest(ApiRequestsDTO apiRequestsDTO) in D:\Playground\source\DMIT\IPP\FuseGate\API.Infrastructure\API.Infrastructure\Application\Setting.cs:line 61  22/01/2026 16:48:06
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
   at API.Infrastructure.Application.Setting.UpdateRequest(UpdateRequestsDTO request) in D:\Playground\source\DMIT\IPP\FuseGate\API.Infrastructure\API.Infrastructure\Application\Setting.cs:line 34  22/01/2026 16:48:19
module: PensionOnboardingError  Error:  Missing or invalid default region.|   at PhoneNumbers.PhoneNumberUtil.ParseHelper(String numberToParse, String defaultRegion, Boolean keepRawInput, Boolean checkRegion, PhoneNumber phoneNumber)
   at PhoneNumbers.PhoneNumberUtil.Parse(String numberToParse, String defaultRegion)
   at API.Infrastructure.Application.Pension.PensionManager.OnboardingRequest(PensionOnboardingDTO request, String Ip, String PartnerId) in D:\Playground\source\DMIT\IPP\FuseGate\API.Infrastructure\API.Infrastructure\Application\Pension\PensionManager.cs:line 45  22/01/2026 16:48:19
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
   at API.Infrastructure.Application.Setting.AddRequest(ApiRequestsDTO apiRequestsDTO) in D:\Playground\source\DMIT\IPP\FuseGate\API.Infrastructure\API.Infrastructure\Application\Setting.cs:line 61  22/01/2026 16:48:42
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
   at API.Infrastructure.Application.Setting.UpdateRequest(UpdateRequestsDTO request) in D:\Playground\source\DMIT\IPP\FuseGate\API.Infrastructure\API.Infrastructure\Application\Setting.cs:line 34  22/01/2026 16:48:50
module: PensionOnboardingError  Error:  Missing or invalid default region.|   at PhoneNumbers.PhoneNumberUtil.ParseHelper(String numberToParse, String defaultRegion, Boolean keepRawInput, Boolean checkRegion, PhoneNumber phoneNumber)
   at PhoneNumbers.PhoneNumberUtil.Parse(String numberToParse, String defaultRegion)
   at API.Infrastructure.Application.Pension.PensionManager.OnboardingRequest(PensionOnboardingDTO request, String Ip, String PartnerId) in D:\Playground\source\DMIT\IPP\FuseGate\API.Infrastructure\API.Infrastructure\Application\Pension\PensionManager.cs:line 45  22/01/2026 16:48:51
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
   at API.Infrastructure.Application.Setting.AddRequest(ApiRequestsDTO apiRequestsDTO) in D:\Playground\source\DMIT\IPP\FuseGate\API.Infrastructure\API.Infrastructure\Application\Setting.cs:line 61  22/01/2026 16:57:51
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
   at API.Infrastructure.Application.Setting.UpdateRequest(UpdateRequestsDTO request)  22/01/2026 16:58:10
module: PensionOnboardingError  Error:  Missing or invalid default region.|   at PhoneNumbers.PhoneNumberUtil.ParseHelper(String numberToParse, String defaultRegion, Boolean keepRawInput, Boolean checkRegion, PhoneNumber phoneNumber)
   at PhoneNumbers.PhoneNumberUtil.Parse(String numberToParse, String defaultRegion)
   at API.Infrastructure.Application.Pension.PensionManager.OnboardingRequest(PensionOnboardingDTO request, String Ip, String PartnerId)  22/01/2026 16:58:10
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
   at API.Infrastructure.Application.Setting.AddRequest(ApiRequestsDTO apiRequestsDTO) in D:\Playground\source\DMIT\IPP\FuseGate\API.Infrastructure\API.Infrastructure\Application\Setting.cs:line 61  22/01/2026 16:59:07
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
   at API.Infrastructure.Application.Setting.UpdateRequest(UpdateRequestsDTO request) in D:\Playground\source\DMIT\IPP\FuseGate\API.Infrastructure\API.Infrastructure\Application\Setting.cs:line 34  22/01/2026 16:59:12
module: PensionOnboardingError  Error:  Missing or invalid default region.|   at PhoneNumbers.PhoneNumberUtil.ParseHelper(String numberToParse, String defaultRegion, Boolean keepRawInput, Boolean checkRegion, PhoneNumber phoneNumber)
   at PhoneNumbers.PhoneNumberUtil.Parse(String numberToParse, String defaultRegion)
   at API.Infrastructure.Application.Pension.PensionManager.OnboardingRequest(PensionOnboardingDTO request, String Ip, String PartnerId) in D:\Playground\source\DMIT\IPP\FuseGate\API.Infrastructure\API.Infrastructure\Application\Pension\PensionManager.cs:line 45  22/01/2026 16:59:12
module: PensionOnboardingError  Error:  There are more columns in the INSERT statement than values specified in the VALUES clause. The number of values in the VALUES clause must match the number of columns specified in the INSERT statement.|   at Microsoft.Data.SqlClient.SqlConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
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
   at API.Infrastructure.Application.Setting.AddRequest(ApiRequestsDTO apiRequestsDTO) in D:\Playground\source\DMIT\IPP\FuseGate\API.Infrastructure\API.Infrastructure\Application\Setting.cs:line 64  22/01/2026 17:07:17
module: PensionOnboardingError  Error:  There are more columns in the INSERT statement than values specified in the VALUES clause. The number of values in the VALUES clause must match the number of columns specified in the INSERT statement.|   at Microsoft.Data.SqlClient.SqlConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
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
   at API.Infrastructure.Application.Setting.AddRequest(ApiRequestsDTO apiRequestsDTO)  22/01/2026 17:07:43
module: PensionOnboardingError  Error:  There are more columns in the INSERT statement than values specified in the VALUES clause. The number of values in the VALUES clause must match the number of columns specified in the INSERT statement.|   at Microsoft.Data.SqlClient.SqlConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
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
   at API.Infrastructure.Application.Setting.AddRequest(ApiRequestsDTO apiRequestsDTO)  22/01/2026 17:08:29
module: PensionOnboardingError  Error:  There are more columns in the INSERT statement than values specified in the VALUES clause. The number of values in the VALUES clause must match the number of columns specified in the INSERT statement.|   at Microsoft.Data.SqlClient.SqlConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
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
   at API.Infrastructure.Application.Setting.AddRequest(ApiRequestsDTO apiRequestsDTO)  22/01/2026 17:08:51
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
   at API.Infrastructure.Application.Setting.AddRequest(ApiRequestsDTO apiRequestsDTO)  22/01/2026 17:09:01
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
   at API.Infrastructure.Application.Setting.UpdateRequest(UpdateRequestsDTO request)  22/01/2026 17:56:49
module: PensionOnboardingError  Error:  Missing or invalid default region.|   at PhoneNumbers.PhoneNumberUtil.ParseHelper(String numberToParse, String defaultRegion, Boolean keepRawInput, Boolean checkRegion, PhoneNumber phoneNumber)
   at PhoneNumbers.PhoneNumberUtil.Parse(String numberToParse, String defaultRegion)
   at API.Infrastructure.Application.Pension.PensionManager.OnboardingRequest(PensionOnboardingDTO request, String Ip, String PartnerId)  22/01/2026 17:56:50
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
   at API.Infrastructure.Application.Setting.UpdateRequest(UpdateRequestsDTO request)  22/01/2026 17:57:13
module: PensionOnboardingError  Error:  Missing or invalid default region.|   at PhoneNumbers.PhoneNumberUtil.ParseHelper(String numberToParse, String defaultRegion, Boolean keepRawInput, Boolean checkRegion, PhoneNumber phoneNumber)
   at PhoneNumbers.PhoneNumberUtil.Parse(String numberToParse, String defaultRegion)
   at API.Infrastructure.Application.Pension.PensionManager.OnboardingRequest(PensionOnboardingDTO request, String Ip, String PartnerId)  22/01/2026 17:57:15
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
   at API.Infrastructure.Application.Setting.UpdateRequest(UpdateRequestsDTO request)  22/01/2026 18:01:05
module: PensionOnboardingError  Error:  Missing or invalid default region.|   at PhoneNumbers.PhoneNumberUtil.ParseHelper(String numberToParse, String defaultRegion, Boolean keepRawInput, Boolean checkRegion, PhoneNumber phoneNumber)
   at PhoneNumbers.PhoneNumberUtil.Parse(String numberToParse, String defaultRegion)
   at API.Infrastructure.Application.Pension.PensionManager.OnboardingRequest(PensionOnboardingDTO request, String Ip, String PartnerId)  22/01/2026 18:01:06
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
   at API.Infrastructure.Application.Setting.UpdateRequest(UpdateRequestsDTO request)  22/01/2026 18:01:40
module: PensionOnboardingError  Error:  Cannot open database "AkibaLve" requested by the login. The login failed.
Login failed for user 'MNDOGOH\mndog'.|   at Microsoft.Data.SqlClient.SqlInternalConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at Microsoft.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, Boolean callerHasConnectionLock, Boolean asyncClose)
   at Microsoft.Data.SqlClient.TdsParser.TryRun(RunBehavior runBehavior, SqlCommand cmdHandler, SqlDataReader dataStream, BulkCopySimpleResultSet bulkCopyHandler, TdsParserStateObject stateObj, Boolean& dataReady)
   at Microsoft.Data.SqlClient.TdsParser.Run(RunBehavior runBehavior, SqlCommand cmdHandler, SqlDataReader dataStream, BulkCopySimpleResultSet bulkCopyHandler, TdsParserStateObject stateObj)
   at Microsoft.Data.SqlClient.SqlInternalConnectionTds.CompleteLogin(Boolean enlistOK)
   at Microsoft.Data.SqlClient.SqlInternalConnectionTds.AttemptOneLogin(ServerInfo serverInfo, String newPassword, SecureString newSecurePassword, Boolean ignoreSniOpenTimeout, TimeoutTimer timeout, Boolean withFailover)
   at Microsoft.Data.SqlClient.SqlInternalConnectionTds.LoginNoFailover(ServerInfo serverInfo, String newPassword, SecureString newSecurePassword, Boolean redirectedUserInstance, SqlConnectionString connectionOptions, SqlCredential credential, TimeoutTimer timeout)
   at Microsoft.Data.SqlClient.SqlInternalConnectionTds.OpenLoginEnlist(TimeoutTimer timeout, SqlConnectionString connectionOptions, SqlCredential credential, String newPassword, SecureString newSecurePassword, Boolean redirectedUserInstance)
   at Microsoft.Data.SqlClient.SqlInternalConnectionTds..ctor(DbConnectionPoolIdentity identity, SqlConnectionString connectionOptions, SqlCredential credential, Object providerInfo, String newPassword, SecureString newSecurePassword, Boolean redirectedUserInstance, SqlConnectionString userConnectionOptions, SessionData reconnectSessionData, Boolean applyTransientFaultHandling, String accessToken, DbConnectionPool pool)
   at Microsoft.Data.SqlClient.SqlConnectionFactory.CreateConnection(DbConnectionOptions options, DbConnectionPoolKey poolKey, Object poolGroupProviderInfo, DbConnectionPool pool, DbConnection owningConnection, DbConnectionOptions userOptions)
   at Microsoft.Data.ProviderBase.DbConnectionFactory.CreatePooledConnection(DbConnectionPool pool, DbConnection owningObject, DbConnectionOptions options, DbConnectionPoolKey poolKey, DbConnectionOptions userOptions)
   at Microsoft.Data.ProviderBase.DbConnectionPool.CreateObject(DbConnection owningObject, DbConnectionOptions userOptions, DbConnectionInternal oldConnection)
   at Microsoft.Data.ProviderBase.DbConnectionPool.UserCreateRequest(DbConnection owningObject, DbConnectionOptions userOptions, DbConnectionInternal oldConnection)
   at Microsoft.Data.ProviderBase.DbConnectionPool.TryGetConnection(DbConnection owningObject, UInt32 waitForMultipleObjectsTimeout, Boolean allowCreate, Boolean onlyOneCheckConnection, DbConnectionOptions userOptions, DbConnectionInternal& connection)
   at Microsoft.Data.ProviderBase.DbConnectionPool.WaitForPendingOpen()
--- End of stack trace from previous location ---
   at Dapper.SqlMapper.ExecuteScalarImplAsync[T](IDbConnection cnn, CommandDefinition command) in /_/Dapper/SqlMapper.Async.cs:line 1209
   at API.Infrastructure.Application.Pension.PensionManager.OnboardingRequest(PensionOnboardingDTO request, String Ip, String PartnerId)  22/01/2026 18:01:40
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
   at API.Infrastructure.Application.Setting.UpdateRequest(UpdateRequestsDTO request) in D:\Playground\source\DMIT\IPP\FuseGate\API.Infrastructure\API.Infrastructure\Application\Setting.cs:line 34  22/01/2026 18:06:27
module: PensionOnboardingError  Error:  Must declare the scalar variable "@EmployementStatus".|   at Microsoft.Data.SqlClient.SqlConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at Microsoft.Data.SqlClient.SqlInternalConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at Microsoft.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, Boolean callerHasConnectionLock, Boolean asyncClose)
   at Microsoft.Data.SqlClient.TdsParser.TryRun(RunBehavior runBehavior, SqlCommand cmdHandler, SqlDataReader dataStream, BulkCopySimpleResultSet bulkCopyHandler, TdsParserStateObject stateObj, Boolean& dataReady)
   at Microsoft.Data.SqlClient.SqlCommand.FinishExecuteReader(SqlDataReader ds, RunBehavior runBehavior, String resetOptionsString, Boolean isInternal, Boolean forDescribeParameterEncryption, Boolean shouldCacheForAlwaysEncrypted)
   at Microsoft.Data.SqlClient.SqlCommand.CompleteAsyncExecuteReader(Boolean isInternal, Boolean forDescribeParameterEncryption)
   at Microsoft.Data.SqlClient.SqlCommand.InternalEndExecuteNonQuery(IAsyncResult asyncResult, Boolean isInternal, String endMethod)
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteNonQueryInternal(IAsyncResult asyncResult)
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteNonQueryAsync(IAsyncResult asyncResult)
   at System.Threading.Tasks.TaskFactory`1.FromAsyncCoreLogic(IAsyncResult iar, Func`2 endFunction, Action`1 endAction, Task`1 promise, Boolean requiresSynchronization)
--- End of stack trace from previous location ---
   at Dapper.SqlMapper.ExecuteImplAsync(IDbConnection cnn, CommandDefinition command, Object param) in /_/Dapper/SqlMapper.Async.cs:line 646
   at API.Infrastructure.Application.Pension.PensionManager.OnboardingRequest(PensionOnboardingDTO request, String Ip, String PartnerId) in D:\Playground\source\DMIT\IPP\FuseGate\API.Infrastructure\API.Infrastructure\Application\Pension\PensionManager.cs:line 138  22/01/2026 18:06:27
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
   at API.Infrastructure.Application.Setting.UpdateRequest(UpdateRequestsDTO request) in D:\Playground\source\DMIT\IPP\FuseGate\API.Infrastructure\API.Infrastructure\Application\Setting.cs:line 34  22/01/2026 18:07:27
module: PensionOnboardingError  Error:  Must declare the scalar variable "@EmployementStatus".|   at Microsoft.Data.SqlClient.SqlConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at Microsoft.Data.SqlClient.SqlInternalConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at Microsoft.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, Boolean callerHasConnectionLock, Boolean asyncClose)
   at Microsoft.Data.SqlClient.TdsParser.TryRun(RunBehavior runBehavior, SqlCommand cmdHandler, SqlDataReader dataStream, BulkCopySimpleResultSet bulkCopyHandler, TdsParserStateObject stateObj, Boolean& dataReady)
   at Microsoft.Data.SqlClient.SqlCommand.FinishExecuteReader(SqlDataReader ds, RunBehavior runBehavior, String resetOptionsString, Boolean isInternal, Boolean forDescribeParameterEncryption, Boolean shouldCacheForAlwaysEncrypted)
   at Microsoft.Data.SqlClient.SqlCommand.CompleteAsyncExecuteReader(Boolean isInternal, Boolean forDescribeParameterEncryption)
   at Microsoft.Data.SqlClient.SqlCommand.InternalEndExecuteNonQuery(IAsyncResult asyncResult, Boolean isInternal, String endMethod)
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteNonQueryInternal(IAsyncResult asyncResult)
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteNonQueryAsync(IAsyncResult asyncResult)
   at System.Threading.Tasks.TaskFactory`1.FromAsyncCoreLogic(IAsyncResult iar, Func`2 endFunction, Action`1 endAction, Task`1 promise, Boolean requiresSynchronization)
--- End of stack trace from previous location ---
   at Dapper.SqlMapper.ExecuteImplAsync(IDbConnection cnn, CommandDefinition command, Object param) in /_/Dapper/SqlMapper.Async.cs:line 646
   at API.Infrastructure.Application.Pension.PensionManager.OnboardingRequest(PensionOnboardingDTO request, String Ip, String PartnerId) in D:\Playground\source\DMIT\IPP\FuseGate\API.Infrastructure\API.Infrastructure\Application\Pension\PensionManager.cs:line 138  22/01/2026 18:07:27
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
   at API.Infrastructure.Application.Setting.UpdateRequest(UpdateRequestsDTO request) in D:\Playground\source\DMIT\IPP\FuseGate\API.Infrastructure\API.Infrastructure\Application\Setting.cs:line 34  22/01/2026 18:12:36
module: PensionOnboardingError  Error:  Incorrect syntax near ','.|   at Microsoft.Data.SqlClient.SqlConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at Microsoft.Data.SqlClient.SqlInternalConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at Microsoft.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, Boolean callerHasConnectionLock, Boolean asyncClose)
   at Microsoft.Data.SqlClient.TdsParser.TryRun(RunBehavior runBehavior, SqlCommand cmdHandler, SqlDataReader dataStream, BulkCopySimpleResultSet bulkCopyHandler, TdsParserStateObject stateObj, Boolean& dataReady)
   at Microsoft.Data.SqlClient.SqlCommand.FinishExecuteReader(SqlDataReader ds, RunBehavior runBehavior, String resetOptionsString, Boolean isInternal, Boolean forDescribeParameterEncryption, Boolean shouldCacheForAlwaysEncrypted)
   at Microsoft.Data.SqlClient.SqlCommand.CompleteAsyncExecuteReader(Boolean isInternal, Boolean forDescribeParameterEncryption)
   at Microsoft.Data.SqlClient.SqlCommand.InternalEndExecuteNonQuery(IAsyncResult asyncResult, Boolean isInternal, String endMethod)
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteNonQueryInternal(IAsyncResult asyncResult)
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteNonQueryAsync(IAsyncResult asyncResult)
   at System.Threading.Tasks.TaskFactory`1.FromAsyncCoreLogic(IAsyncResult iar, Func`2 endFunction, Action`1 endAction, Task`1 promise, Boolean requiresSynchronization)
--- End of stack trace from previous location ---
   at Dapper.SqlMapper.ExecuteImplAsync(IDbConnection cnn, CommandDefinition command, Object param) in /_/Dapper/SqlMapper.Async.cs:line 646
   at API.Infrastructure.Application.Pension.PensionManager.OnboardingRequest(PensionOnboardingDTO request, String Ip, String PartnerId) in D:\Playground\source\DMIT\IPP\FuseGate\API.Infrastructure\API.Infrastructure\Application\Pension\PensionManager.cs:line 137  22/01/2026 18:12:37
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
   at API.Infrastructure.Application.Setting.UpdateRequest(UpdateRequestsDTO request)  22/01/2026 18:16:50
module: PensionOnboardingError  Error:  Must declare the scalar variable "@MaritalStatus".|   at Microsoft.Data.SqlClient.SqlConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at Microsoft.Data.SqlClient.SqlInternalConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at Microsoft.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, Boolean callerHasConnectionLock, Boolean asyncClose)
   at Microsoft.Data.SqlClient.TdsParser.TryRun(RunBehavior runBehavior, SqlCommand cmdHandler, SqlDataReader dataStream, BulkCopySimpleResultSet bulkCopyHandler, TdsParserStateObject stateObj, Boolean& dataReady)
   at Microsoft.Data.SqlClient.SqlCommand.FinishExecuteReader(SqlDataReader ds, RunBehavior runBehavior, String resetOptionsString, Boolean isInternal, Boolean forDescribeParameterEncryption, Boolean shouldCacheForAlwaysEncrypted)
   at Microsoft.Data.SqlClient.SqlCommand.CompleteAsyncExecuteReader(Boolean isInternal, Boolean forDescribeParameterEncryption)
   at Microsoft.Data.SqlClient.SqlCommand.InternalEndExecuteNonQuery(IAsyncResult asyncResult, Boolean isInternal, String endMethod)
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteNonQueryInternal(IAsyncResult asyncResult)
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteNonQueryAsync(IAsyncResult asyncResult)
   at System.Threading.Tasks.TaskFactory`1.FromAsyncCoreLogic(IAsyncResult iar, Func`2 endFunction, Action`1 endAction, Task`1 promise, Boolean requiresSynchronization)
--- End of stack trace from previous location ---
   at Dapper.SqlMapper.ExecuteImplAsync(IDbConnection cnn, CommandDefinition command, Object param) in /_/Dapper/SqlMapper.Async.cs:line 646
   at API.Infrastructure.Application.Pension.PensionManager.OnboardingRequest(PensionOnboardingDTO request, String Ip, String PartnerId)  22/01/2026 18:16:50
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
   at API.Infrastructure.Application.Setting.UpdateRequest(UpdateRequestsDTO request)  22/01/2026 18:18:34
module: PensionOnboardingError  Error:  Must declare the scalar variable "@MaritalStatus".|   at Microsoft.Data.SqlClient.SqlConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at Microsoft.Data.SqlClient.SqlInternalConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at Microsoft.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, Boolean callerHasConnectionLock, Boolean asyncClose)
   at Microsoft.Data.SqlClient.TdsParser.TryRun(RunBehavior runBehavior, SqlCommand cmdHandler, SqlDataReader dataStream, BulkCopySimpleResultSet bulkCopyHandler, TdsParserStateObject stateObj, Boolean& dataReady)
   at Microsoft.Data.SqlClient.SqlCommand.FinishExecuteReader(SqlDataReader ds, RunBehavior runBehavior, String resetOptionsString, Boolean isInternal, Boolean forDescribeParameterEncryption, Boolean shouldCacheForAlwaysEncrypted)
   at Microsoft.Data.SqlClient.SqlCommand.CompleteAsyncExecuteReader(Boolean isInternal, Boolean forDescribeParameterEncryption)
   at Microsoft.Data.SqlClient.SqlCommand.InternalEndExecuteNonQuery(IAsyncResult asyncResult, Boolean isInternal, String endMethod)
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteNonQueryInternal(IAsyncResult asyncResult)
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteNonQueryAsync(IAsyncResult asyncResult)
   at System.Threading.Tasks.TaskFactory`1.FromAsyncCoreLogic(IAsyncResult iar, Func`2 endFunction, Action`1 endAction, Task`1 promise, Boolean requiresSynchronization)
--- End of stack trace from previous location ---
   at Dapper.SqlMapper.ExecuteImplAsync(IDbConnection cnn, CommandDefinition command, Object param) in /_/Dapper/SqlMapper.Async.cs:line 646
   at API.Infrastructure.Application.Pension.PensionManager.OnboardingRequest(PensionOnboardingDTO request, String Ip, String PartnerId)  22/01/2026 18:18:34
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
   at API.Infrastructure.Application.Setting.UpdateRequest(UpdateRequestsDTO request)  22/01/2026 18:20:52
module: PensionOnboardingError  Error:  Must declare the scalar variable "@MemberJoinDate".|   at Microsoft.Data.SqlClient.SqlConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at Microsoft.Data.SqlClient.SqlInternalConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at Microsoft.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, Boolean callerHasConnectionLock, Boolean asyncClose)
   at Microsoft.Data.SqlClient.TdsParser.TryRun(RunBehavior runBehavior, SqlCommand cmdHandler, SqlDataReader dataStream, BulkCopySimpleResultSet bulkCopyHandler, TdsParserStateObject stateObj, Boolean& dataReady)
   at Microsoft.Data.SqlClient.SqlCommand.FinishExecuteReader(SqlDataReader ds, RunBehavior runBehavior, String resetOptionsString, Boolean isInternal, Boolean forDescribeParameterEncryption, Boolean shouldCacheForAlwaysEncrypted)
   at Microsoft.Data.SqlClient.SqlCommand.CompleteAsyncExecuteReader(Boolean isInternal, Boolean forDescribeParameterEncryption)
   at Microsoft.Data.SqlClient.SqlCommand.InternalEndExecuteNonQuery(IAsyncResult asyncResult, Boolean isInternal, String endMethod)
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteNonQueryInternal(IAsyncResult asyncResult)
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteNonQueryAsync(IAsyncResult asyncResult)
   at System.Threading.Tasks.TaskFactory`1.FromAsyncCoreLogic(IAsyncResult iar, Func`2 endFunction, Action`1 endAction, Task`1 promise, Boolean requiresSynchronization)
--- End of stack trace from previous location ---
   at Dapper.SqlMapper.ExecuteImplAsync(IDbConnection cnn, CommandDefinition command, Object param) in /_/Dapper/SqlMapper.Async.cs:line 646
   at API.Infrastructure.Application.Pension.PensionManager.OnboardingRequest(PensionOnboardingDTO request, String Ip, String PartnerId)  22/01/2026 18:20:54
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
   at API.Infrastructure.Application.Setting.UpdateRequest(UpdateRequestsDTO request) in D:\Playground\source\DMIT\IPP\FuseGate\API.Infrastructure\API.Infrastructure\Application\Setting.cs:line 34  22/01/2026 18:24:39
module: PensionOnboardingError  Error:  Invalid column name 'PartnerId'.|   at Microsoft.Data.SqlClient.SqlConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at Microsoft.Data.SqlClient.SqlInternalConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at Microsoft.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, Boolean callerHasConnectionLock, Boolean asyncClose)
   at Microsoft.Data.SqlClient.TdsParser.TryRun(RunBehavior runBehavior, SqlCommand cmdHandler, SqlDataReader dataStream, BulkCopySimpleResultSet bulkCopyHandler, TdsParserStateObject stateObj, Boolean& dataReady)
   at Microsoft.Data.SqlClient.SqlCommand.FinishExecuteReader(SqlDataReader ds, RunBehavior runBehavior, String resetOptionsString, Boolean isInternal, Boolean forDescribeParameterEncryption, Boolean shouldCacheForAlwaysEncrypted)
   at Microsoft.Data.SqlClient.SqlCommand.CompleteAsyncExecuteReader(Boolean isInternal, Boolean forDescribeParameterEncryption)
   at Microsoft.Data.SqlClient.SqlCommand.InternalEndExecuteNonQuery(IAsyncResult asyncResult, Boolean isInternal, String endMethod)
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteNonQueryInternal(IAsyncResult asyncResult)
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteNonQueryAsync(IAsyncResult asyncResult)
   at System.Threading.Tasks.TaskFactory`1.FromAsyncCoreLogic(IAsyncResult iar, Func`2 endFunction, Action`1 endAction, Task`1 promise, Boolean requiresSynchronization)
--- End of stack trace from previous location ---
   at Dapper.SqlMapper.ExecuteImplAsync(IDbConnection cnn, CommandDefinition command, Object param) in /_/Dapper/SqlMapper.Async.cs:line 646
   at API.Infrastructure.Application.Pension.PensionManager.OnboardingRequest(PensionOnboardingDTO request, String Ip, String PartnerId) in D:\Playground\source\DMIT\IPP\FuseGate\API.Infrastructure\API.Infrastructure\Application\Pension\PensionManager.cs:line 146  22/01/2026 18:24:39
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
   at API.Infrastructure.Application.Setting.UpdateRequest(UpdateRequestsDTO request)  22/01/2026 18:27:35
module: PensionOnboardingError  Error:  Invalid column name 'PartnerId'.|   at Microsoft.Data.SqlClient.SqlConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at Microsoft.Data.SqlClient.SqlInternalConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at Microsoft.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, Boolean callerHasConnectionLock, Boolean asyncClose)
   at Microsoft.Data.SqlClient.TdsParser.TryRun(RunBehavior runBehavior, SqlCommand cmdHandler, SqlDataReader dataStream, BulkCopySimpleResultSet bulkCopyHandler, TdsParserStateObject stateObj, Boolean& dataReady)
   at Microsoft.Data.SqlClient.SqlCommand.FinishExecuteReader(SqlDataReader ds, RunBehavior runBehavior, String resetOptionsString, Boolean isInternal, Boolean forDescribeParameterEncryption, Boolean shouldCacheForAlwaysEncrypted)
   at Microsoft.Data.SqlClient.SqlCommand.CompleteAsyncExecuteReader(Boolean isInternal, Boolean forDescribeParameterEncryption)
   at Microsoft.Data.SqlClient.SqlCommand.InternalEndExecuteNonQuery(IAsyncResult asyncResult, Boolean isInternal, String endMethod)
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteNonQueryInternal(IAsyncResult asyncResult)
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteNonQueryAsync(IAsyncResult asyncResult)
   at System.Threading.Tasks.TaskFactory`1.FromAsyncCoreLogic(IAsyncResult iar, Func`2 endFunction, Action`1 endAction, Task`1 promise, Boolean requiresSynchronization)
--- End of stack trace from previous location ---
   at Dapper.SqlMapper.ExecuteImplAsync(IDbConnection cnn, CommandDefinition command, Object param) in /_/Dapper/SqlMapper.Async.cs:line 646
   at API.Infrastructure.Application.Pension.PensionManager.OnboardingRequest(PensionOnboardingDTO request, String Ip, String PartnerId)  22/01/2026 18:27:35
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
   at API.Infrastructure.Application.Setting.UpdateRequest(UpdateRequestsDTO request)  22/01/2026 18:35:09
module: PensionOnboardingError  Error:  Invalid column name 'PartnerId'.|   at Microsoft.Data.SqlClient.SqlConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at Microsoft.Data.SqlClient.SqlInternalConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at Microsoft.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, Boolean callerHasConnectionLock, Boolean asyncClose)
   at Microsoft.Data.SqlClient.TdsParser.TryRun(RunBehavior runBehavior, SqlCommand cmdHandler, SqlDataReader dataStream, BulkCopySimpleResultSet bulkCopyHandler, TdsParserStateObject stateObj, Boolean& dataReady)
   at Microsoft.Data.SqlClient.SqlCommand.FinishExecuteReader(SqlDataReader ds, RunBehavior runBehavior, String resetOptionsString, Boolean isInternal, Boolean forDescribeParameterEncryption, Boolean shouldCacheForAlwaysEncrypted)
   at Microsoft.Data.SqlClient.SqlCommand.CompleteAsyncExecuteReader(Boolean isInternal, Boolean forDescribeParameterEncryption)
   at Microsoft.Data.SqlClient.SqlCommand.InternalEndExecuteNonQuery(IAsyncResult asyncResult, Boolean isInternal, String endMethod)
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteNonQueryInternal(IAsyncResult asyncResult)
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteNonQueryAsync(IAsyncResult asyncResult)
   at System.Threading.Tasks.TaskFactory`1.FromAsyncCoreLogic(IAsyncResult iar, Func`2 endFunction, Action`1 endAction, Task`1 promise, Boolean requiresSynchronization)
--- End of stack trace from previous location ---
   at Dapper.SqlMapper.ExecuteImplAsync(IDbConnection cnn, CommandDefinition command, Object param) in /_/Dapper/SqlMapper.Async.cs:line 646
   at API.Infrastructure.Application.Pension.PensionManager.OnboardingRequest(PensionOnboardingDTO request, String Ip, String PartnerId)  22/01/2026 18:35:09
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
   at API.Infrastructure.Application.Setting.UpdateRequest(UpdateRequestsDTO request)  22/01/2026 18:54:52
module: PensionOnboardingError  Error:  Invalid column name 'PartnerId'.|   at Microsoft.Data.SqlClient.SqlConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at Microsoft.Data.SqlClient.SqlInternalConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at Microsoft.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, Boolean callerHasConnectionLock, Boolean asyncClose)
   at Microsoft.Data.SqlClient.TdsParser.TryRun(RunBehavior runBehavior, SqlCommand cmdHandler, SqlDataReader dataStream, BulkCopySimpleResultSet bulkCopyHandler, TdsParserStateObject stateObj, Boolean& dataReady)
   at Microsoft.Data.SqlClient.SqlCommand.FinishExecuteReader(SqlDataReader ds, RunBehavior runBehavior, String resetOptionsString, Boolean isInternal, Boolean forDescribeParameterEncryption, Boolean shouldCacheForAlwaysEncrypted)
   at Microsoft.Data.SqlClient.SqlCommand.CompleteAsyncExecuteReader(Boolean isInternal, Boolean forDescribeParameterEncryption)
   at Microsoft.Data.SqlClient.SqlCommand.InternalEndExecuteNonQuery(IAsyncResult asyncResult, Boolean isInternal, String endMethod)
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteNonQueryInternal(IAsyncResult asyncResult)
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteNonQueryAsync(IAsyncResult asyncResult)
   at System.Threading.Tasks.TaskFactory`1.FromAsyncCoreLogic(IAsyncResult iar, Func`2 endFunction, Action`1 endAction, Task`1 promise, Boolean requiresSynchronization)
--- End of stack trace from previous location ---
   at Dapper.SqlMapper.ExecuteImplAsync(IDbConnection cnn, CommandDefinition command, Object param) in /_/Dapper/SqlMapper.Async.cs:line 646
   at API.Infrastructure.Application.Pension.PensionManager.OnboardingRequest(PensionOnboardingDTO request, String Ip, String PartnerId)  22/01/2026 18:54:53
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
   at API.Infrastructure.Application.Setting.UpdateRequest(UpdateRequestsDTO request)  22/01/2026 18:55:24
module: PensionOnboardingError  Error:  Invalid column name 'PartnerId'.
There are more columns in the INSERT statement than values specified in the VALUES clause. The number of values in the VALUES clause must match the number of columns specified in the INSERT statement.|   at Microsoft.Data.SqlClient.SqlConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at Microsoft.Data.SqlClient.SqlInternalConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at Microsoft.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, Boolean callerHasConnectionLock, Boolean asyncClose)
   at Microsoft.Data.SqlClient.TdsParser.TryRun(RunBehavior runBehavior, SqlCommand cmdHandler, SqlDataReader dataStream, BulkCopySimpleResultSet bulkCopyHandler, TdsParserStateObject stateObj, Boolean& dataReady)
   at Microsoft.Data.SqlClient.SqlCommand.FinishExecuteReader(SqlDataReader ds, RunBehavior runBehavior, String resetOptionsString, Boolean isInternal, Boolean forDescribeParameterEncryption, Boolean shouldCacheForAlwaysEncrypted)
   at Microsoft.Data.SqlClient.SqlCommand.CompleteAsyncExecuteReader(Boolean isInternal, Boolean forDescribeParameterEncryption)
   at Microsoft.Data.SqlClient.SqlCommand.InternalEndExecuteNonQuery(IAsyncResult asyncResult, Boolean isInternal, String endMethod)
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteNonQueryInternal(IAsyncResult asyncResult)
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteNonQueryAsync(IAsyncResult asyncResult)
   at System.Threading.Tasks.TaskFactory`1.FromAsyncCoreLogic(IAsyncResult iar, Func`2 endFunction, Action`1 endAction, Task`1 promise, Boolean requiresSynchronization)
--- End of stack trace from previous location ---
   at Dapper.SqlMapper.ExecuteImplAsync(IDbConnection cnn, CommandDefinition command, Object param) in /_/Dapper/SqlMapper.Async.cs:line 646
   at API.Infrastructure.Application.Pension.PensionManager.OnboardingRequest(PensionOnboardingDTO request, String Ip, String PartnerId)  22/01/2026 18:55:25
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
   at API.Infrastructure.Application.Setting.UpdateRequest(UpdateRequestsDTO request)  22/01/2026 19:00:14
module: PensionOnboardingError  Error:  Must declare the scalar variable "@EmployementStatus".|   at Microsoft.Data.SqlClient.SqlConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at Microsoft.Data.SqlClient.SqlInternalConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at Microsoft.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, Boolean callerHasConnectionLock, Boolean asyncClose)
   at Microsoft.Data.SqlClient.TdsParser.TryRun(RunBehavior runBehavior, SqlCommand cmdHandler, SqlDataReader dataStream, BulkCopySimpleResultSet bulkCopyHandler, TdsParserStateObject stateObj, Boolean& dataReady)
   at Microsoft.Data.SqlClient.SqlCommand.FinishExecuteReader(SqlDataReader ds, RunBehavior runBehavior, String resetOptionsString, Boolean isInternal, Boolean forDescribeParameterEncryption, Boolean shouldCacheForAlwaysEncrypted)
   at Microsoft.Data.SqlClient.SqlCommand.CompleteAsyncExecuteReader(Boolean isInternal, Boolean forDescribeParameterEncryption)
   at Microsoft.Data.SqlClient.SqlCommand.InternalEndExecuteNonQuery(IAsyncResult asyncResult, Boolean isInternal, String endMethod)
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteNonQueryInternal(IAsyncResult asyncResult)
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteNonQueryAsync(IAsyncResult asyncResult)
   at System.Threading.Tasks.TaskFactory`1.FromAsyncCoreLogic(IAsyncResult iar, Func`2 endFunction, Action`1 endAction, Task`1 promise, Boolean requiresSynchronization)
--- End of stack trace from previous location ---
   at Dapper.SqlMapper.ExecuteImplAsync(IDbConnection cnn, CommandDefinition command, Object param) in /_/Dapper/SqlMapper.Async.cs:line 646
   at API.Infrastructure.Application.Pension.PensionManager.OnboardingRequest(PensionOnboardingDTO request, String Ip, String PartnerId)  22/01/2026 19:00:19
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
   at API.Infrastructure.Application.Setting.UpdateRequest(UpdateRequestsDTO request)  22/01/2026 19:00:37
module: PensionOnboardingError  Error:  Object reference not set to an instance of an object.|   at System.Collections.Generic.List`1.Enumerator.MoveNext()
   at API.Infrastructure.Application.Pension.PensionManager.OnboardingRequest(PensionOnboardingDTO request, String Ip, String PartnerId)  22/01/2026 19:00:38
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
   at API.Infrastructure.Application.Setting.UpdateRequest(UpdateRequestsDTO request)  22/01/2026 19:09:36

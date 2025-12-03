module: PensionOnboardingError  Error:  Incorrect syntax near 'Created'.|   at Microsoft.Data.SqlClient.SqlConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at Microsoft.Data.SqlClient.SqlInternalConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at Microsoft.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, Boolean callerHasConnectionLock, Boolean asyncClose)
   at Microsoft.Data.SqlClient.TdsParser.TryRun(RunBehavior runBehavior, SqlCommand cmdHandler, SqlDataReader dataStream, BulkCopySimpleResultSet bulkCopyHandler, TdsParserStateObject stateObj, Boolean& dataReady)
   at Microsoft.Data.SqlClient.SqlCommand.InternalEndExecuteNonQuery(IAsyncResult asyncResult, Boolean isInternal, String endMethod)
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteNonQueryInternal(IAsyncResult asyncResult)
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteNonQueryAsync(IAsyncResult asyncResult)
   at System.Threading.Tasks.TaskFactory`1.FromAsyncCoreLogic(IAsyncResult iar, Func`2 endFunction, Action`1 endAction, Task`1 promise, Boolean requiresSynchronization)
--- End of stack trace from previous location ---
   at Dapper.SqlMapper.ExecuteImplAsync(IDbConnection cnn, CommandDefinition command, Object param) in /_/Dapper/SqlMapper.Async.cs:line 646
   at API.Infrastructure.Application.Pension.PensionManager.OnboardingRequest(PensionOnboardingDTO request, String Ip)  21/10/2025 10:28:17
module: PensionOnboardingError  Error:  Conversion failed when converting the varchar value 'API' to data type int.|   at Microsoft.Data.SqlClient.SqlConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at Microsoft.Data.SqlClient.SqlInternalConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at Microsoft.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, Boolean callerHasConnectionLock, Boolean asyncClose)
   at Microsoft.Data.SqlClient.TdsParser.TryRun(RunBehavior runBehavior, SqlCommand cmdHandler, SqlDataReader dataStream, BulkCopySimpleResultSet bulkCopyHandler, TdsParserStateObject stateObj, Boolean& dataReady)
   at Microsoft.Data.SqlClient.SqlCommand.InternalEndExecuteNonQuery(IAsyncResult asyncResult, Boolean isInternal, String endMethod)
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteNonQueryInternal(IAsyncResult asyncResult)
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteNonQueryAsync(IAsyncResult asyncResult)
   at System.Threading.Tasks.TaskFactory`1.FromAsyncCoreLogic(IAsyncResult iar, Func`2 endFunction, Action`1 endAction, Task`1 promise, Boolean requiresSynchronization)
--- End of stack trace from previous location ---
   at Dapper.SqlMapper.ExecuteImplAsync(IDbConnection cnn, CommandDefinition command, Object param) in /_/Dapper/SqlMapper.Async.cs:line 646
   at API.Infrastructure.Application.Pension.PensionManager.OnboardingRequest(PensionOnboardingDTO request, String Ip)  21/10/2025 10:28:49
module: PensionOnboardingError  Error:  Invalid object name 'dbo.AkibaCustomers'.|   at Microsoft.Data.SqlClient.SqlConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at Microsoft.Data.SqlClient.SqlInternalConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at Microsoft.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, Boolean callerHasConnectionLock, Boolean asyncClose)
   at Microsoft.Data.SqlClient.TdsParser.TryRun(RunBehavior runBehavior, SqlCommand cmdHandler, SqlDataReader dataStream, BulkCopySimpleResultSet bulkCopyHandler, TdsParserStateObject stateObj, Boolean& dataReady)
   at Microsoft.Data.SqlClient.SqlCommand.InternalEndExecuteNonQuery(IAsyncResult asyncResult, Boolean isInternal, String endMethod)
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteNonQueryInternal(IAsyncResult asyncResult)
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteNonQueryAsync(IAsyncResult asyncResult)
   at System.Threading.Tasks.TaskFactory`1.FromAsyncCoreLogic(IAsyncResult iar, Func`2 endFunction, Action`1 endAction, Task`1 promise, Boolean requiresSynchronization)
--- End of stack trace from previous location ---
   at Dapper.SqlMapper.ExecuteImplAsync(IDbConnection cnn, CommandDefinition command, Object param) in /_/Dapper/SqlMapper.Async.cs:line 646
   at API.Infrastructure.Application.Pension.PensionManager.OnboardingRequest(PensionOnboardingDTO request, String Ip) in D:\Playground\source\DMIT\IPP\FuseGate\API.Infrastructure\API.Infrastructure\Application\Pension\PensionManager.cs:line 97  21/10/2025 11:13:43
module: PensionOnboardingError  Error:  No mapping exists from DbType UInt64 to a known SqlDbType.|   at Microsoft.Data.SqlClient.MetaType.GetMetaTypeFromDbType(DbType target)
   at Microsoft.Data.SqlClient.SqlParameter.set_DbType(DbType value)
   at ParamInfo3d5c0ba7-8afe-4e84-b714-baab722a05e4(IDbCommand, Object)
   at Dapper.CommandDefinition.SetupCommand(IDbConnection cnn, Action`2 paramReader)
   at Dapper.SqlMapper.ExecuteScalarImpl[T](IDbConnection cnn, CommandDefinition& command)
   at Dapper.SqlMapper.ExecuteScalar[T](IDbConnection cnn, String sql, Object param, IDbTransaction transaction, Nullable`1 commandTimeout, Nullable`1 commandType)
   at API.Infrastructure.Application.Pension.PensionManager.OnboardingRequest(PensionOnboardingDTO request, String Ip)  21/10/2025 11:53:22
module: PensionOnboardingError  Error:  No mapping exists from DbType UInt64 to a known SqlDbType.|   at Microsoft.Data.SqlClient.MetaType.GetMetaTypeFromDbType(DbType target)
   at Microsoft.Data.SqlClient.SqlParameter.set_DbType(DbType value)
   at ParamInfo3d5c0ba7-8afe-4e84-b714-baab722a05e4(IDbCommand, Object)
   at Dapper.CommandDefinition.SetupCommand(IDbConnection cnn, Action`2 paramReader) in /_/Dapper/CommandDefinition.cs:line 128
   at Dapper.SqlMapper.ExecuteScalarImpl[T](IDbConnection cnn, CommandDefinition& command) in /_/Dapper/SqlMapper.cs:line 2875
   at Dapper.SqlMapper.ExecuteScalar[T](IDbConnection cnn, String sql, Object param, IDbTransaction transaction, Nullable`1 commandTimeout, Nullable`1 commandType) in /_/Dapper/SqlMapper.cs:line 493
   at API.Infrastructure.Application.Pension.PensionManager.OnboardingRequest(PensionOnboardingDTO request, String Ip)  21/10/2025 11:54:10
module: PensionOnboardingError  Error:  No mapping exists from DbType UInt64 to a known SqlDbType.|   at Microsoft.Data.SqlClient.MetaType.GetMetaTypeFromDbType(DbType target)
   at Microsoft.Data.SqlClient.SqlParameter.set_DbType(DbType value)
   at ParamInfodae256e2-8402-4591-a038-2db05a42e9e2(IDbCommand, Object)
   at Dapper.CommandDefinition.SetupCommand(IDbConnection cnn, Action`2 paramReader) in /_/Dapper/CommandDefinition.cs:line 128
   at Dapper.SqlMapper.ExecuteScalarImpl[T](IDbConnection cnn, CommandDefinition& command) in /_/Dapper/SqlMapper.cs:line 2875
   at Dapper.SqlMapper.ExecuteScalar[T](IDbConnection cnn, String sql, Object param, IDbTransaction transaction, Nullable`1 commandTimeout, Nullable`1 commandType) in /_/Dapper/SqlMapper.cs:line 493
   at API.Infrastructure.Application.Pension.PensionManager.OnboardingRequest(PensionOnboardingDTO request, String Ip)  21/10/2025 11:54:22
module: PensionOnboardingError  Error:  No mapping exists from DbType UInt64 to a known SqlDbType.|   at Microsoft.Data.SqlClient.MetaType.GetMetaTypeFromDbType(DbType target)
   at Microsoft.Data.SqlClient.SqlParameter.set_DbType(DbType value)
   at ParamInfo3d2ecdc5-e26e-4d3d-94b3-0a406b6caeeb(IDbCommand, Object)
   at Dapper.CommandDefinition.SetupCommand(IDbConnection cnn, Action`2 paramReader) in /_/Dapper/CommandDefinition.cs:line 128
   at Dapper.SqlMapper.ExecuteScalarImpl[T](IDbConnection cnn, CommandDefinition& command) in /_/Dapper/SqlMapper.cs:line 2875
   at Dapper.SqlMapper.ExecuteScalar[T](IDbConnection cnn, String sql, Object param, IDbTransaction transaction, Nullable`1 commandTimeout, Nullable`1 commandType) in /_/Dapper/SqlMapper.cs:line 493
   at API.Infrastructure.Application.Pension.PensionManager.OnboardingRequest(PensionOnboardingDTO request, String Ip)  21/10/2025 11:55:42
module: PensionOnboardingError  Error:  No mapping exists from DbType UInt64 to a known SqlDbType.|   at Microsoft.Data.SqlClient.MetaType.GetMetaTypeFromDbType(DbType target)
   at Microsoft.Data.SqlClient.SqlParameter.set_DbType(DbType value)
   at ParamInfo3d2ecdc5-e26e-4d3d-94b3-0a406b6caeeb(IDbCommand, Object)
   at Dapper.CommandDefinition.SetupCommand(IDbConnection cnn, Action`2 paramReader) in /_/Dapper/CommandDefinition.cs:line 128
   at Dapper.SqlMapper.ExecuteScalarImpl[T](IDbConnection cnn, CommandDefinition& command) in /_/Dapper/SqlMapper.cs:line 2875
   at Dapper.SqlMapper.ExecuteScalar[T](IDbConnection cnn, String sql, Object param, IDbTransaction transaction, Nullable`1 commandTimeout, Nullable`1 commandType) in /_/Dapper/SqlMapper.cs:line 493
   at API.Infrastructure.Application.Pension.PensionManager.OnboardingRequest(PensionOnboardingDTO request, String Ip)  21/10/2025 11:55:55
module: PensionOnboardingError  Error:  No mapping exists from DbType UInt64 to a known SqlDbType.|   at Microsoft.Data.SqlClient.MetaType.GetMetaTypeFromDbType(DbType target)
   at Microsoft.Data.SqlClient.SqlParameter.set_DbType(DbType value)
   at ParamInfo3d2ecdc5-e26e-4d3d-94b3-0a406b6caeeb(IDbCommand, Object)
   at Dapper.CommandDefinition.SetupCommand(IDbConnection cnn, Action`2 paramReader) in /_/Dapper/CommandDefinition.cs:line 128
   at Dapper.SqlMapper.ExecuteScalarImpl[T](IDbConnection cnn, CommandDefinition& command) in /_/Dapper/SqlMapper.cs:line 2875
   at Dapper.SqlMapper.ExecuteScalar[T](IDbConnection cnn, String sql, Object param, IDbTransaction transaction, Nullable`1 commandTimeout, Nullable`1 commandType) in /_/Dapper/SqlMapper.cs:line 493
   at API.Infrastructure.Application.Pension.PensionManager.OnboardingRequest(PensionOnboardingDTO request, String Ip)  21/10/2025 11:59:18
module: PensionOnboardingError  Error:  Cannot insert the value NULL into column 'FundStatus', table 'AkibaCore.dbo.PensionerFund'; column does not allow nulls. INSERT fails.
The statement has been terminated.|   at Microsoft.Data.SqlClient.SqlConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
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
   at API.Infrastructure.Application.Pension.PensionManager.OnboardingRequest(PensionOnboardingDTO request, String Ip)  21/10/2025 12:01:40

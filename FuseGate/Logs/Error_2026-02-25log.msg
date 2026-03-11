module: SendSMSOTPs  Error:  startIndex ('-3') must be a non-negative value. (Parameter 'startIndex')
Actual value was -3.  25/02/2026 20:44:43
module: SendSMSOTPs  Error:     at System.ArgumentOutOfRangeException.ThrowNegative[T](T value, String paramName)
   at System.String.ThrowSubstringArgumentOutOfRange(Int32 startIndex, Int32 length)
   at System.String.Substring(Int32 startIndex)
   at API.Infrastructure.Application.Communication.Communication.ProcessSMS() in D:\Playground\source\DMIT\IPP\FuseGate\API.Infrastructure\API.Infrastructure\Application\Communication\Communication.cs:line 72  25/02/2026 20:44:43
module: SendSMSOTPs  Error:  startIndex ('-3') must be a non-negative value. (Parameter 'startIndex')
Actual value was -3.  25/02/2026 21:26:12
module: SendSMSOTPs  Error:     at System.ArgumentOutOfRangeException.ThrowNegative[T](T value, String paramName)
   at System.String.ThrowSubstringArgumentOutOfRange(Int32 startIndex, Int32 length)
   at System.String.Substring(Int32 startIndex)
   at API.Infrastructure.Application.Communication.Communication.ProcessSMS() in D:\Playground\source\DMIT\IPP\FuseGate\API.Infrastructure\API.Infrastructure\Application\Communication\Communication.cs:line 67  25/02/2026 21:26:12
module: SendSMSOTPs  Error:  startIndex ('-3') must be a non-negative value. (Parameter 'startIndex')
Actual value was -3.  25/02/2026 21:26:35
module: SendSMSOTPs  Error:     at System.ArgumentOutOfRangeException.ThrowNegative[T](T value, String paramName)
   at System.String.ThrowSubstringArgumentOutOfRange(Int32 startIndex, Int32 length)
   at System.String.Substring(Int32 startIndex)
   at API.Infrastructure.Application.Communication.Communication.ProcessSMS() in D:\Playground\source\DMIT\IPP\FuseGate\API.Infrastructure\API.Infrastructure\Application\Communication\Communication.cs:line 67  25/02/2026 21:26:35
module: SendSMSOTPs  Error:  startIndex ('-3') must be a non-negative value. (Parameter 'startIndex')
Actual value was -3.  25/02/2026 21:26:36
module: SendSMSOTPs  Error:     at System.ArgumentOutOfRangeException.ThrowNegative[T](T value, String paramName)
   at System.String.ThrowSubstringArgumentOutOfRange(Int32 startIndex, Int32 length)
   at System.String.Substring(Int32 startIndex)
   at API.Infrastructure.Application.Communication.Communication.ProcessSMS() in D:\Playground\source\DMIT\IPP\FuseGate\API.Infrastructure\API.Infrastructure\Application\Communication\Communication.cs:line 67  25/02/2026 21:26:36
module: SendSMSOTPs  Error:  startIndex ('-3') must be a non-negative value. (Parameter 'startIndex')
Actual value was -3.  25/02/2026 21:26:37
module: SendSMSOTPs  Error:     at System.ArgumentOutOfRangeException.ThrowNegative[T](T value, String paramName)
   at System.String.ThrowSubstringArgumentOutOfRange(Int32 startIndex, Int32 length)
   at System.String.Substring(Int32 startIndex)
   at API.Infrastructure.Application.Communication.Communication.ProcessSMS() in D:\Playground\source\DMIT\IPP\FuseGate\API.Infrastructure\API.Infrastructure\Application\Communication\Communication.cs:line 67  25/02/2026 21:26:37
module: SendSMSOTPs  Error:  Invalid operation. The connection is closed.  25/02/2026 21:27:18
module: SendSMSOTPs  Error:     at Microsoft.Data.SqlClient.SqlCommand.<>c.<ExecuteDbDataReaderAsync>b__208_0(Task`1 result)
   at System.Threading.Tasks.ContinuationResultTaskFromResultTask`2.InnerInvoke()
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Dapper.SqlMapper.QueryRowAsync[T](IDbConnection cnn, Row row, Type effectiveType, CommandDefinition command) in /_/Dapper/SqlMapper.Async.cs:line 473
   at API.Infrastructure.Application.Communication.Communication.ProcessSMS()  25/02/2026 21:27:18
module: SendSMSOTPs  Error:  Cannot access a disposed context instance. A common cause of this error is disposing a context instance that was resolved from dependency injection and then later trying to use the same context instance elsewhere in your application. This may occur if you are calling 'Dispose' on the context instance, or wrapping it in a using statement. If you are using dependency injection, you should let the dependency injection container take care of disposing context instances.
Object name: 'ApplicationDbContext'.  25/02/2026 21:27:18
module: SendSMSOTPs  Error:  startIndex ('-3') must be a non-negative value. (Parameter 'startIndex')
Actual value was -3.  25/02/2026 21:27:28
module: SendSMSOTPs  Error:     at System.ArgumentOutOfRangeException.ThrowNegative[T](T value, String paramName)
   at System.String.ThrowSubstringArgumentOutOfRange(Int32 startIndex, Int32 length)
   at System.String.Substring(Int32 startIndex)
   at API.Infrastructure.Application.Communication.Communication.ProcessSMS()  25/02/2026 21:27:28
module: SendSMSOTPs  Error:  startIndex ('-3') must be a non-negative value. (Parameter 'startIndex')
Actual value was -3.  25/02/2026 21:27:38
module: SendSMSOTPs  Error:     at System.ArgumentOutOfRangeException.ThrowNegative[T](T value, String paramName)
   at System.String.ThrowSubstringArgumentOutOfRange(Int32 startIndex, Int32 length)
   at System.String.Substring(Int32 startIndex)
   at API.Infrastructure.Application.Communication.Communication.ProcessSMS()  25/02/2026 21:27:38
module: SendSMSOTPs  Error:  startIndex ('-3') must be a non-negative value. (Parameter 'startIndex')
Actual value was -3.  25/02/2026 21:27:48
module: SendSMSOTPs  Error:     at System.ArgumentOutOfRangeException.ThrowNegative[T](T value, String paramName)
   at System.String.ThrowSubstringArgumentOutOfRange(Int32 startIndex, Int32 length)
   at System.String.Substring(Int32 startIndex)
   at API.Infrastructure.Application.Communication.Communication.ProcessSMS()  25/02/2026 21:27:48
module: SendSMSOTPs  Error:  startIndex ('-3') must be a non-negative value. (Parameter 'startIndex')
Actual value was -3.  25/02/2026 21:27:59
module: SendSMSOTPs  Error:     at System.ArgumentOutOfRangeException.ThrowNegative[T](T value, String paramName)
   at System.String.ThrowSubstringArgumentOutOfRange(Int32 startIndex, Int32 length)
   at System.String.Substring(Int32 startIndex)
   at API.Infrastructure.Application.Communication.Communication.ProcessSMS()  25/02/2026 21:27:59
module: Partner code AuthenticatePartner  Error:  LATBS9Y  25/02/2026 21:28:05
module: SendSMSOTPs  Error:  startIndex ('-3') must be a non-negative value. (Parameter 'startIndex')
Actual value was -3.  25/02/2026 21:28:09
module: SendSMSOTPs  Error:     at System.ArgumentOutOfRangeException.ThrowNegative[T](T value, String paramName)
   at System.String.ThrowSubstringArgumentOutOfRange(Int32 startIndex, Int32 length)
   at System.String.Substring(Int32 startIndex)
   at API.Infrastructure.Application.Communication.Communication.ProcessSMS()  25/02/2026 21:28:09
module: SendSMSOTPs  Error:  startIndex ('-3') must be a non-negative value. (Parameter 'startIndex')
Actual value was -3.  25/02/2026 21:28:19
module: SendSMSOTPs  Error:     at System.ArgumentOutOfRangeException.ThrowNegative[T](T value, String paramName)
   at System.String.ThrowSubstringArgumentOutOfRange(Int32 startIndex, Int32 length)
   at System.String.Substring(Int32 startIndex)
   at API.Infrastructure.Application.Communication.Communication.ProcessSMS()  25/02/2026 21:28:19
module: SendSMSOTPs  Error:  startIndex ('-3') must be a non-negative value. (Parameter 'startIndex')
Actual value was -3.  25/02/2026 21:28:29
module: SendSMSOTPs  Error:     at System.ArgumentOutOfRangeException.ThrowNegative[T](T value, String paramName)
   at System.String.ThrowSubstringArgumentOutOfRange(Int32 startIndex, Int32 length)
   at System.String.Substring(Int32 startIndex)
   at API.Infrastructure.Application.Communication.Communication.ProcessSMS()  25/02/2026 21:28:29
module: SendSMSOTPs  Error:  startIndex ('-3') must be a non-negative value. (Parameter 'startIndex')
Actual value was -3.  25/02/2026 21:28:39
module: SendSMSOTPs  Error:     at System.ArgumentOutOfRangeException.ThrowNegative[T](T value, String paramName)
   at System.String.ThrowSubstringArgumentOutOfRange(Int32 startIndex, Int32 length)
   at System.String.Substring(Int32 startIndex)
   at API.Infrastructure.Application.Communication.Communication.ProcessSMS()  25/02/2026 21:28:39
module: SendSMSOTPs  Error:  startIndex ('-3') must be a non-negative value. (Parameter 'startIndex')
Actual value was -3.  25/02/2026 21:28:49
module: SendSMSOTPs  Error:     at System.ArgumentOutOfRangeException.ThrowNegative[T](T value, String paramName)
   at System.String.ThrowSubstringArgumentOutOfRange(Int32 startIndex, Int32 length)
   at System.String.Substring(Int32 startIndex)
   at API.Infrastructure.Application.Communication.Communication.ProcessSMS()  25/02/2026 21:28:49
module: SendSMSOTPs  Error:  startIndex ('-3') must be a non-negative value. (Parameter 'startIndex')
Actual value was -3.  25/02/2026 21:28:59
module: SendSMSOTPs  Error:     at System.ArgumentOutOfRangeException.ThrowNegative[T](T value, String paramName)
   at System.String.ThrowSubstringArgumentOutOfRange(Int32 startIndex, Int32 length)
   at System.String.Substring(Int32 startIndex)
   at API.Infrastructure.Application.Communication.Communication.ProcessSMS()  25/02/2026 21:28:59
module: SendSMSOTPs  Error:  startIndex ('-3') must be a non-negative value. (Parameter 'startIndex')
Actual value was -3.  25/02/2026 21:29:09
module: SendSMSOTPs  Error:     at System.ArgumentOutOfRangeException.ThrowNegative[T](T value, String paramName)
   at System.String.ThrowSubstringArgumentOutOfRange(Int32 startIndex, Int32 length)
   at System.String.Substring(Int32 startIndex)
   at API.Infrastructure.Application.Communication.Communication.ProcessSMS()  25/02/2026 21:29:09
module: SendSMSOTPs  Error:  startIndex ('-3') must be a non-negative value. (Parameter 'startIndex')
Actual value was -3.  25/02/2026 21:29:19
module: SendSMSOTPs  Error:     at System.ArgumentOutOfRangeException.ThrowNegative[T](T value, String paramName)
   at System.String.ThrowSubstringArgumentOutOfRange(Int32 startIndex, Int32 length)
   at System.String.Substring(Int32 startIndex)
   at API.Infrastructure.Application.Communication.Communication.ProcessSMS()  25/02/2026 21:29:19
module: SendSMSOTPs  Error:  startIndex ('-3') must be a non-negative value. (Parameter 'startIndex')
Actual value was -3.  25/02/2026 21:29:29
module: SendSMSOTPs  Error:     at System.ArgumentOutOfRangeException.ThrowNegative[T](T value, String paramName)
   at System.String.ThrowSubstringArgumentOutOfRange(Int32 startIndex, Int32 length)
   at System.String.Substring(Int32 startIndex)
   at API.Infrastructure.Application.Communication.Communication.ProcessSMS()  25/02/2026 21:29:29
module: SendSMSOTPs  Error:  startIndex ('-3') must be a non-negative value. (Parameter 'startIndex')
Actual value was -3.  25/02/2026 21:29:39
module: SendSMSOTPs  Error:     at System.ArgumentOutOfRangeException.ThrowNegative[T](T value, String paramName)
   at System.String.ThrowSubstringArgumentOutOfRange(Int32 startIndex, Int32 length)
   at System.String.Substring(Int32 startIndex)
   at API.Infrastructure.Application.Communication.Communication.ProcessSMS()  25/02/2026 21:29:39
module: InsertCustomerProductAsync  Error:  Error in InsertCustomerProductAsync: An error occurred while saving the entity changes. See the inner exception for details.  25/02/2026 21:29:46
module: SendSMSOTPs  Error:  startIndex ('-3') must be a non-negative value. (Parameter 'startIndex')
Actual value was -3.  25/02/2026 21:29:49
module: SendSMSOTPs  Error:     at System.ArgumentOutOfRangeException.ThrowNegative[T](T value, String paramName)
   at System.String.ThrowSubstringArgumentOutOfRange(Int32 startIndex, Int32 length)
   at System.String.Substring(Int32 startIndex)
   at API.Infrastructure.Application.Communication.Communication.ProcessSMS()  25/02/2026 21:29:49
module: InsertCustomerProductAsync  Error:  Error in InsertCustomerProductAsync: An error occurred while saving the entity changes. See the inner exception for details.  25/02/2026 21:33:08
module: OnBoarding  Error:  Error in OnBoarding: An error occurred while saving the entity changes. See the inner exception for details.  25/02/2026 21:35:18
module: InsertCustomerProductAsync  Error:  Error in InsertCustomerProductAsync: An error occurred while saving the entity changes. See the inner exception for details.  25/02/2026 21:35:34
module: InsertCustomerProductAsync  Error:  Error in InsertCustomerProductAsync: An error occurred while saving the entity changes. See the inner exception for details.  25/02/2026 21:35:38
module: InsertCustomerProductAsync  Error:  Error in InsertCustomerProductAsync: An error occurred while saving the entity changes. See the inner exception for details.  25/02/2026 21:36:46
module: GetCreditwaveToken  Error:  An invalid request URI was provided. Either the request URI must be an absolute URI or BaseAddress must be set.  25/02/2026 21:40:51
module: SendCreditwaveSMSAsync  Error:  An invalid request URI was provided. Either the request URI must be an absolute URI or BaseAddress must be set.  25/02/2026 21:40:51
module: GetCreditwaveToken  Error:  An invalid request URI was provided. Either the request URI must be an absolute URI or BaseAddress must be set.  25/02/2026 21:49:30
module: SendCreditwaveSMSAsync  Error:  An invalid request URI was provided. Either the request URI must be an absolute URI or BaseAddress must be set.  25/02/2026 21:49:31
module: GetCreditwaveToken  Error:  An invalid request URI was provided. Either the request URI must be an absolute URI or BaseAddress must be set.  25/02/2026 21:50:01
module: SendCreditwaveSMSAsync  Error:  An invalid request URI was provided. Either the request URI must be an absolute URI or BaseAddress must be set.  25/02/2026 21:50:28

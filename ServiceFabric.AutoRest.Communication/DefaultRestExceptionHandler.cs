using System.Net;
using Microsoft.Rest;
using Microsoft.ServiceFabric.Services.Communication.Client;

namespace ServiceFabric.AutoRest.Communication
{
    public class DefaultRestExceptionHandler : IExceptionHandler
    {
        public bool TryHandleException(ExceptionInformation exceptionInformation, OperationRetrySettings retrySettings,
            out ExceptionHandlingResult result)
        {
            var ex = exceptionInformation.Exception as HttpOperationException;
            if (ex != null && ex.Response.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                result = new ExceptionHandlingRetryResult(ex, false, retrySettings, retrySettings.DefaultMaxRetryCount);
                return true;
            }

            result = null;
            return false;
        }
    }
}
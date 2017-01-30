using System.Linq;
using System.Net;
using Microsoft.Rest;
using Microsoft.ServiceFabric.Services.Communication.Client;

namespace ServiceFabric.AutoRest.Communication.Client
{
    public class HttpOperationExceptionHandler : IExceptionHandler
    {
        public bool TryHandleException(ExceptionInformation exceptionInformation, OperationRetrySettings retrySettings, out ExceptionHandlingResult result)
        {
            var ex = exceptionInformation.Exception as HttpOperationException;

            if (ex?.Response != null)
            {
                switch (ex.Response.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        if (HasResourceNotFoundHeader(ex.Response))
                            break;
                        result = new ExceptionHandlingRetryResult(ex, false, retrySettings, retrySettings.DefaultMaxRetryCount);
                        return true;
                    case HttpStatusCode.ServiceUnavailable:
                        result = new ExceptionHandlingRetryResult(ex, false, retrySettings, retrySettings.DefaultMaxRetryCount);
                        return true;
                    default:
                        result = null;
                        return false;
                }
            }

            result = null;
            return false;
        }

        private bool HasResourceNotFoundHeader(HttpResponseMessageWrapper response)
        {
            return response.Headers.ContainsKey("X-ServiceFabric") &&
                   response.Headers["X-ServiceFabric"].Contains("ResourceNotFound");
        }
    }
}
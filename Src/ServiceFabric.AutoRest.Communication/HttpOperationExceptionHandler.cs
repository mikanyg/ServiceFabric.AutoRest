using System.Linq;
using System.Net;
using Microsoft.Rest;
using Microsoft.ServiceFabric.Services.Communication.Client;
using System.Diagnostics;

namespace ServiceFabric.AutoRest.Communication.Client
{
    public class HttpOperationExceptionHandler : IExceptionHandler
    {
        private const string SfCustomHeader = "X-ServiceFabric";
        private const string Status404HeaderValue = "ResourceNotFound";

        public bool TryHandleException(ExceptionInformation exceptionInformation, OperationRetrySettings retrySettings, out ExceptionHandlingResult result)
        {
            TraceMessage("Determining exception handling strategy.");

            var ex = exceptionInformation.Exception as HttpOperationException;            

            if (ex?.Response != null)
            {
                TraceMessage($"Response status code is '{ex.Response.StatusCode}'");

                switch (ex.Response.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        if (HasResourceNotFoundHeader(ex.Response))
                        {
                            TraceMessage($"'{SfCustomHeader}:{Status404HeaderValue}' is present in response header, indicating a RESTfull 404 response.");
                            break;
                        }
                        TraceMessage($"'{SfCustomHeader}:{Status404HeaderValue}' is not present in response header, indicating a service has been moved.");                        
                        result = new ExceptionHandlingRetryResult(ex, false, retrySettings, retrySettings.DefaultMaxRetryCount);
                        return true;
                    case HttpStatusCode.ServiceUnavailable:
                        result = new ExceptionHandlingRetryResult(ex, false, retrySettings, retrySettings.DefaultMaxRetryCount);
                        return true;
                    default:
                        break;
                }
            }

            TraceMessage("No exception handling strategy was applied.");

            result = null;
            return false;
        }

        private bool HasResourceNotFoundHeader(HttpResponseMessageWrapper response)
        {
            return response.Headers.ContainsKey(SfCustomHeader) &&
                   response.Headers[SfCustomHeader].Contains(Status404HeaderValue);
        }

        private static void TraceMessage(string message)
        {
            Trace.TraceInformation($"ServiceFabric.AutoRest, {nameof(HttpOperationExceptionHandler)}: {message}");
        }
    }
}
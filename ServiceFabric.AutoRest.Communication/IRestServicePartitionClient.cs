using Microsoft.ServiceFabric.Services.Communication.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Rest;

namespace ServiceFabric.AutoRest.Communication
{
    public interface IRestServicePartitionClient<TClient> where TClient : ServiceClient<TClient>
    {
        void InvokeWithRetry(Action<AutoRestCommunicationClient<TClient>> func, params Type[] doNotRetryExceptionTypes);
        TResult InvokeWithRetry<TResult>(Func<AutoRestCommunicationClient<TClient>, TResult> func, params Type[] doNotRetryExceptionTypes);
        Task InvokeWithRetryAsync(Func<AutoRestCommunicationClient<TClient>, Task> func, params Type[] doNotRetryExceptionTypes);
        Task InvokeWithRetryAsync(Func<AutoRestCommunicationClient<TClient>, Task> func, CancellationToken cancellationToken, params Type[] doNotRetryExceptionTypes);
        Task<TResult> InvokeWithRetryAsync<TResult>(Func<AutoRestCommunicationClient<TClient>, Task<TResult>> func, params Type[] doNotRetryExceptionTypes);
        Task<TResult> InvokeWithRetryAsync<TResult>(Func<AutoRestCommunicationClient<TClient>, Task<TResult>> func, CancellationToken cancellationToken, params Type[] doNotRetryExceptionTypes);
    }
}

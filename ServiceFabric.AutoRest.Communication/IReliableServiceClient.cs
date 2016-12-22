using Microsoft.ServiceFabric.Services.Communication.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceFabric.AutoRest.Communication
{
    public interface IReliableServiceClient<TCommunicationClient> where TCommunicationClient : ICommunicationClient
    {
        void InvokeWithRetry(Action<TCommunicationClient> func, params Type[] doNotRetryExceptionTypes);
        TResult InvokeWithRetry<TResult>(Func<TCommunicationClient, TResult> func, params Type[] doNotRetryExceptionTypes);
        Task InvokeWithRetryAsync(Func<TCommunicationClient, Task> func, params Type[] doNotRetryExceptionTypes);
        Task InvokeWithRetryAsync(Func<TCommunicationClient, Task> func, CancellationToken cancellationToken, params Type[] doNotRetryExceptionTypes);
        Task<TResult> InvokeWithRetryAsync<TResult>(Func<TCommunicationClient, Task<TResult>> func, params Type[] doNotRetryExceptionTypes);
        Task<TResult> InvokeWithRetryAsync<TResult>(Func<TCommunicationClient, Task<TResult>> func, CancellationToken cancellationToken, params Type[] doNotRetryExceptionTypes);
    }
}

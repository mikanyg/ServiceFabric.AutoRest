﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Rest;

namespace ServiceFabric.AutoRest.Communication.Client
{
    public interface IRestServicePartitionClient<TClient> where TClient : ServiceClient<TClient>
    {
        void InvokeWithRetry(Action<RestCommunicationClient<TClient>> func, params Type[] doNotRetryExceptionTypes);
        TResult InvokeWithRetry<TResult>(Func<RestCommunicationClient<TClient>, TResult> func, params Type[] doNotRetryExceptionTypes);
        Task InvokeWithRetryAsync(Func<RestCommunicationClient<TClient>, Task> func, params Type[] doNotRetryExceptionTypes);
        Task InvokeWithRetryAsync(Func<RestCommunicationClient<TClient>, Task> func, CancellationToken cancellationToken, params Type[] doNotRetryExceptionTypes);
        Task<TResult> InvokeWithRetryAsync<TResult>(Func<RestCommunicationClient<TClient>, Task<TResult>> func, params Type[] doNotRetryExceptionTypes);
        Task<TResult> InvokeWithRetryAsync<TResult>(Func<RestCommunicationClient<TClient>, Task<TResult>> func, CancellationToken cancellationToken, params Type[] doNotRetryExceptionTypes);
    }
}

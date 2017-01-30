using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Rest;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;

namespace ServiceFabric.AutoRest.Communication.Client
{
    public class RestCommunicationClientFactory<TServiceClient> : CommunicationClientFactoryBase<RestCommunicationClient<TServiceClient>>
        where TServiceClient : ServiceClient<TServiceClient>
    {
        private readonly Func<IEnumerable<DelegatingHandler>> delegatingHandlers;
        private readonly ServiceClientCredentials credentials;
        private readonly bool clientSupportsCredentials;

        public RestCommunicationClientFactory(
            IServicePartitionResolver resolver = null,
            IEnumerable<IExceptionHandler> exceptionHandlers = null,
            Func<IEnumerable<DelegatingHandler>> delegatingHandlers = null,
            ServiceClientCredentials credentials = null)
            : base(resolver, CreateExceptionHandlers(exceptionHandlers))
        {
            this.delegatingHandlers = delegatingHandlers;
            this.credentials = credentials;
            this.clientSupportsCredentials = typeof(TServiceClient).HasCredentialsSupport();
        }

        protected override void AbortClient(RestCommunicationClient<TServiceClient> client)
        {            
            // HTTP clients don't hold persistent connections, so no action is taken.
        }

        protected override Task<RestCommunicationClient<TServiceClient>> CreateClientAsync(string endpoint, CancellationToken cancellationToken)
        {
            var baseUri = new Uri(endpoint);
            var handlers = delegatingHandlers?.Invoke()?.ToArray() ?? new DelegatingHandler[0];

            TServiceClient client;
            try
            {
                if (clientSupportsCredentials)
                {
                    client = (TServiceClient) Activator.CreateInstance(typeof(TServiceClient), baseUri, credentials, handlers);
                }
                else
                {
                    client = (TServiceClient) Activator.CreateInstance(typeof(TServiceClient), baseUri, handlers);
                }
            }
            catch (MissingMethodException ex)
            {
                throw new NotSupportedException($"Unable to find suitable contructor to initialize {typeof(TServiceClient).FullName}", ex);
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException is ArgumentException)
                {
                    // Keeps the stacktrace from inner exception
                    ExceptionDispatchInfo.Capture(ex.InnerException).Throw(); 
                }

                throw;
            }
            
            // disabling AutoRest retry policy since Service Fabric has own retry logic.
            client.SetRetryPolicy(null);

            return Task.FromResult(new RestCommunicationClient<TServiceClient>(client));                        
        }

        protected override bool ValidateClient(RestCommunicationClient<TServiceClient> client)
        {            
            // HTTP clients don't hold persistent connections, so no validation needs to be done.
            return true;
        }

        protected override bool ValidateClient(string endpoint, RestCommunicationClient<TServiceClient> client)
        {            
            // HTTP clients don't hold persistent connections, so no validation needs to be done.
            return true;
        }

        private static IEnumerable<IExceptionHandler> CreateExceptionHandlers(IEnumerable<IExceptionHandler> userDefinedHandlers)
        {
            var list = new List<IExceptionHandler>();

            if (userDefinedHandlers != null)
            {
                list.AddRange(userDefinedHandlers);
            }

            return list.Union(new[] {new HttpOperationExceptionHandler()});
        }
    }
}

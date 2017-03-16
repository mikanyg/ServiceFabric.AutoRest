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
using System.Diagnostics;

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
            this.clientSupportsCredentials = typeof(TServiceClient).HasCredentialsSupport();

            if (clientSupportsCredentials && credentials == null)
                throw new ArgumentException($"Credentials are required for type: {typeof(TServiceClient).FullName}", nameof(credentials));

            this.credentials = credentials;
        }

        protected override void AbortClient(RestCommunicationClient<TServiceClient> client)
        {            
            // HTTP clients don't hold persistent connections, so no action is taken.
        }

        protected override Task<RestCommunicationClient<TServiceClient>> CreateClientAsync(string endpoint, CancellationToken cancellationToken)
        {
            TraceMessage($"Creating {typeof(RestCommunicationClient<TServiceClient>)} with internal service endpoint located at '{endpoint}'.");

            var baseUri = new Uri(endpoint);
            var handlers = delegatingHandlers?.Invoke()?.ToArray() ?? new DelegatingHandler[0];            

            TServiceClient client;
            try
            {
                if (clientSupportsCredentials)
                {
                    client = (TServiceClient) Activator.CreateInstance(typeof(TServiceClient), baseUri, credentials, handlers);
                    TraceMessage($"Created {typeof(TServiceClient)} with credentials support.");
                }
                else
                {
                    client = (TServiceClient) Activator.CreateInstance(typeof(TServiceClient), baseUri, handlers);
                    TraceMessage($"Created {typeof(TServiceClient)} without credentials support.");
                }
            }
            catch (MissingMethodException ex)
            {
                throw new NotSupportedException($"Unable to find suitable contructor to initialize {typeof(TServiceClient)}", ex);
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

            TraceMessage("Disabling AutoRest default retry policy.");
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

        private static void TraceMessage(string message)
        {
            Trace.TraceInformation($"ServiceFabric.AutoRest, {nameof(RestCommunicationClientFactory<TServiceClient>)}: {message}");
        }
    }
}

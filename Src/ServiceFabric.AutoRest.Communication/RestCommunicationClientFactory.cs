using Microsoft.Rest;
using Microsoft.Rest.TransientFaultHandling;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceFabric.AutoRest.Communication.Client
{
    public class RestCommunicationClientFactory<TServiceClient> : CommunicationClientFactoryBase<RestCommunicationClient<TServiceClient>>
        where TServiceClient : ServiceClient<TServiceClient>
    {
        private readonly Func<IEnumerable<DelegatingHandler>> delegatingHandlers;
        private readonly ICredentialsManager credentialsManager;
        private readonly ServiceClientCredentials credentials;
        private readonly bool clientSupportsCredentials;

        public RestCommunicationClientFactory(
            IServicePartitionResolver resolver = null,
            IEnumerable<IExceptionHandler> exceptionHandlers = null,
            Func<IEnumerable<DelegatingHandler>> delegatingHandlers = null,
            ServiceClientCredentials credentials = null,
            ICredentialsManager credentialsManager = null)
            : base(resolver, CreateExceptionHandlers(exceptionHandlers))
        {
            this.delegatingHandlers = delegatingHandlers;            
            this.clientSupportsCredentials = typeof(TServiceClient).HasCredentialsSupport();

            if (clientSupportsCredentials)
            {
                if (credentials == null && credentialsManager == null)
                    throw new ArgumentException($"Type: {typeof(TServiceClient).FullName} was generated with authentication support, and requires either '{nameof(credentials)}' or '{nameof(credentialsManager)}' parameter.");

                this.credentials = credentials;
                this.credentialsManager = credentialsManager;
            }
            else
            {
                if (credentials != null || credentialsManager != null)
                    throw new ArgumentException($"Type: {typeof(TServiceClient).FullName} was generated without authentication support, do not set '{nameof(credentials)}' or '{nameof(credentialsManager)}' parameter.");
            }
        }

        /// <summary>
        /// ClientCreated event is fired when the communication client has been created.
        /// </summary>
        public event EventHandler<CommunicationClientEventArgs<RestCommunicationClient<TServiceClient>>> ClientCreated;

        /// <summary>
        /// ClientValidating event is fired when the communication client is being validated. 
        /// Acts as an extension point to determine whether a cached communication client is still valid.
        /// </summary>
        public event EventHandler<CommunicationClientValidatingEventArgs<RestCommunicationClient<TServiceClient>>> ClientValidating;

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
                    var creds = GetServiceCredentialsAsync().GetAwaiter().GetResult();
                    client = (TServiceClient) Activator.CreateInstance(typeof(TServiceClient), baseUri, creds, handlers);
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

            client.SetRetryPolicy(new RetryPolicy<TransientErrorIgnoreStrategy>(0));
            TraceMessage("Disabled AutoRest default retry policy.");

            var communicationClient = new RestCommunicationClient<TServiceClient>(client);

            if (ClientCreated != null)
            {
                ClientCreated(this, new CommunicationClientEventArgs<RestCommunicationClient<TServiceClient>>
                {
                    Client = communicationClient
                });
                TraceMessage($"{nameof(ClientCreated)} event handlers invoked.");
            }

            return Task.FromResult(communicationClient);
        }

        protected override bool ValidateClient(RestCommunicationClient<TServiceClient> client)
        {
            TraceMessage($"Validating {typeof(RestCommunicationClient<TServiceClient>)}.");
            return OnValidateClient(client);
        }

        protected override bool ValidateClient(string endpoint, RestCommunicationClient<TServiceClient> client)
        {
            TraceMessage($"Validating {typeof(RestCommunicationClient<TServiceClient>)} with expected endpoint located at '{endpoint}'.");
            return OnValidateClient(client);
        }

        private bool OnValidateClient(RestCommunicationClient<TServiceClient> client)
        {
            bool clientIsValid = true;

            if (ClientValidating != null)
            {
                var args = new CommunicationClientValidatingEventArgs<RestCommunicationClient<TServiceClient>>
                {
                    Client = client,
                    IsValid = true
                };

                ClientValidating(this, args);
                TraceMessage($"{nameof(ClientValidating)} event handlers invoked.");

                clientIsValid = args.IsValid;
            }

            TraceMessage($"Client is valid: {clientIsValid}");

            return clientIsValid;
        }

        private Task<ServiceClientCredentials> GetServiceCredentialsAsync()
        {
            if(credentials != null)
            {
                TraceMessage($"Using credentials as {nameof(ServiceClientCredentials)}.");
                return Task.FromResult(credentials);
            }
            else
            {
                TraceMessage($"Using credentials manager to get {nameof(ServiceClientCredentials)}.");
                return credentialsManager.GetCredentialsAsync();
            }            
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

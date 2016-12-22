using Microsoft.Rest;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceFabric.AutoRest.Communication
{
    public class AutoRestCommunicationClientFactory<TServiceClient> : CommunicationClientFactoryBase<AutoRestCommunicationClient<TServiceClient>>
        where TServiceClient : ServiceClient<TServiceClient>
    {
        private readonly DelegatingHandler[] delegatingHandlers;

        public AutoRestCommunicationClientFactory(
            IServicePartitionResolver resolver = null,
            IEnumerable<DelegatingHandler> delegatingHandlers = null,
            bool useDefaultExceptionHandler = true,
            IEnumerable<IExceptionHandler> exceptionHandlers = null)
            : base(resolver, CreateExceptionHandlers(useDefaultExceptionHandler, exceptionHandlers))
        {
            this.delegatingHandlers = delegatingHandlers?.ToArray() ?? new DelegatingHandler[0];
        }

        protected override void AbortClient(AutoRestCommunicationClient<TServiceClient> client)
        {            
            // HTTP clients don't hold persistent connections, so no action is taken.
        }

        protected override Task<AutoRestCommunicationClient<TServiceClient>> CreateClientAsync(string endpoint, CancellationToken cancellationToken)
        {
            var baseUri = new Uri(endpoint);
                        
            var client = (TServiceClient)Activator.CreateInstance(typeof(TServiceClient), baseUri, delegatingHandlers);
            // disabling AutoRest retry policy since Service Fabric has own retry logic.
            client.SetRetryPolicy(null);
            return Task.FromResult(new AutoRestCommunicationClient<TServiceClient>(client));                        
        }

        protected override bool ValidateClient(AutoRestCommunicationClient<TServiceClient> client)
        {            
            // HTTP clients don't hold persistent connections, so no validation needs to be done.
            return true;
        }

        protected override bool ValidateClient(string endpoint, AutoRestCommunicationClient<TServiceClient> client)
        {            
            // HTTP clients don't hold persistent connections, so no validation needs to be done.
            return true;
        }

        private static IEnumerable<IExceptionHandler> CreateExceptionHandlers(bool useDefault, IEnumerable<IExceptionHandler> additionalHandlers)
        {
            var handlers = new List<IExceptionHandler>();

            if (useDefault)
            {
                handlers.Add(new DefaultHttpExceptionHandler());
            }
            else if(additionalHandlers == null)
            {
                throw new Exception("When disabling the default exception handler, additional exception handlers must be specified.");
            }

            handlers.AddRange(additionalHandlers ?? Enumerable.Empty<IExceptionHandler>());

            return handlers;
        }
    }    
}

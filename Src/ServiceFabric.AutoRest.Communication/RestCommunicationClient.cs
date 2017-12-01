using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Fabric;
using Microsoft.Rest;
using Microsoft.ServiceFabric.Services.Communication.Client;

namespace ServiceFabric.AutoRest.Communication.Client
{
    public class RestCommunicationClient<TServiceClient> : ICommunicationClient
        where TServiceClient : ServiceClient<TServiceClient>
    {
        public RestCommunicationClient(TServiceClient client)
        {
            RestApi = client ?? throw new ArgumentNullException(nameof(client));
            Properties = new ConcurrentDictionary<string, object>();
        }

        public TServiceClient RestApi { get; }

        /// <summary>
        /// Generic properties bag for the client.
        /// </summary>
        public IDictionary<string, object> Properties { get; }

        ResolvedServiceEndpoint ICommunicationClient.Endpoint { get; set; }

        string ICommunicationClient.ListenerName { get; set; }

        ResolvedServicePartition ICommunicationClient.ResolvedServicePartition { get; set; }
    }    
}
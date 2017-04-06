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
            if (client == null) throw new ArgumentNullException(nameof(client));

            RestApi = client;
            Properties = new ConcurrentDictionary<string, object>();
        }

        public ResolvedServiceEndpoint Endpoint { get; set; }        

        public string ListenerName { get; set; }

        /// <summary>
        /// Generic properties bag for the client.
        /// </summary>
        public IDictionary<string, object> Properties { get; }

        public ResolvedServicePartition ResolvedServicePartition { get; set; }

        public TServiceClient RestApi { get; }
        
    }    
}
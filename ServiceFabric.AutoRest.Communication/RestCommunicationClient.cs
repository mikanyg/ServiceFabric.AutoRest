using Microsoft.Rest;
using Microsoft.ServiceFabric.Services.Communication.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Fabric;

namespace ServiceFabric.AutoRest.Communication
{
    public class RestCommunicationClient<TServiceClient> : ICommunicationClient
        where TServiceClient : ServiceClient<TServiceClient>
    {
        public RestCommunicationClient(TServiceClient client)
        {
            ServiceClient = client;            
        }

        public ResolvedServiceEndpoint Endpoint { get; set; }        

        public string ListenerName { get; set; }

        public ResolvedServicePartition ResolvedServicePartition { get; set; }

        public TServiceClient ServiceClient { get; }
    }    
}
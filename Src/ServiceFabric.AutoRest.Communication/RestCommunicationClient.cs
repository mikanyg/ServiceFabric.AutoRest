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
            RestApi = client;            
        }

        public ResolvedServiceEndpoint Endpoint { get; set; }        

        public string ListenerName { get; set; }

        public ResolvedServicePartition ResolvedServicePartition { get; set; }

        public TServiceClient RestApi { get; }
    }    
}
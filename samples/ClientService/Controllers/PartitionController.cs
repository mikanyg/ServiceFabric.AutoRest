using ClientService.Handlers;
using Microsoft.Rest;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using ServiceFabric.AutoRest.Communication.Client;
using System;
using System.Web.Http;
using WebApi.AutoRest.Client;
using WebApiStateful.AutoRest.Client;

namespace ClientService.Controllers
{
    [ServiceRequestActionFilter]
    public class PartitionController : ApiController
    {        
        private static readonly Uri serviceUri = new Uri("fabric:/ServiceFabric.AutoRest.Sample/WebApiStateful");        
        private static readonly IRestServicePartitionClientFactory<StatefullClient> partitionClientFactory;

        static PartitionController()
        {            
            var communicationFactory = new RestCommunicationClientFactory<StatefullClient>(delegatingHandlers: () => new[] { new MyHandler() },
                credentials: new TokenCredentials("5682a964-2615-4150-8d3b-851a702b30ad", "Bearer"));
            partitionClientFactory = new RestServicePartitionClientFactory<StatefullClient>(communicationFactory, serviceUri, TargetReplicaSelector.RandomReplica);
        }

        // GET api/partition/{id} 
        public string Get(int id)
        {
            var partitionClient = partitionClientFactory.Create(new ServicePartitionKey(id));

            var result = partitionClient.InvokeWithRetry(c => c.RestApi.Values.Get(id));

            return result;
        }        
    }
}

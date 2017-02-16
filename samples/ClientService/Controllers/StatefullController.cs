using ClientService.Handlers;
using Microsoft.Rest;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using ServiceFabric.AutoRest.Communication.Client;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using WebApi.AutoRest.Client;
using WebApiStateful.AutoRest.Client;

namespace ClientService.Controllers
{
    [ServiceRequestActionFilter]
    public class StatefullController : ApiController
    {        
        private static readonly Uri serviceUri = new Uri("fabric:/ServiceFabric.AutoRest.Sample/WebApiStateful");        
        private static readonly RestCommunicationClientFactory<StatefullClient> communicationFactory = 
            new RestCommunicationClientFactory<StatefullClient>(delegatingHandlers: () => new[] { new MyHandler() }, 
                credentials: new TokenCredentials("5682a964-2615-4150-8d3b-851a702b30ad", "Bearer"));

        private ServicePartitionKey partitionKey = new ServicePartitionKey(long.MinValue);

        // GET api/statefull/{id}
        public async Task<string> Get(int id)
        {
            var partitionClient = new ServicePartitionClient<RestCommunicationClient<StatefullClient>>(communicationFactory,
                    serviceUri, partitionKey, TargetReplicaSelector.RandomReplica);

            var result = await partitionClient.InvokeWithRetryAsync(
                async c => await c.RestApi.Values.GetAsync(id));

            return result;
        }
    }
}

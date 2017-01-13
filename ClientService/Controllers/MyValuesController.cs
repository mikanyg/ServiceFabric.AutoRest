using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using ServiceFabric.AutoRest.Communication;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using WebApi.AutoRest.Client;
using WebApiStateful.AutoRest.Client;

namespace ClientService.Controllers
{
    [ServiceRequestActionFilter]
    public class MyValuesController : ApiController
    {
        private static readonly Uri statelessServiceUri = new Uri("fabric:/ServiceFabric.AutoRest.Clients/WebApi");
        private static readonly Uri statefulServiceUri = new Uri("fabric:/ServiceFabric.AutoRest.Clients/WebApiStateful");        
        private static readonly IRestServicePartitionClientFactory<WebApiClient> statelessClientFactory;
        private static readonly IRestServicePartitionClientFactory<WebApi2> statefullClientFactory;

        static MyValuesController()
        {
            var statelessCommunicationFactory =
                new RestCommunicationClientFactory<WebApiClient>();
            var statefullCommunicationFactory =
                new RestCommunicationClientFactory<WebApi2>(delegatingHandlers: () => new[] {new MyHandler()});

            statelessClientFactory = new RestServicePartitionClientFactory<WebApiClient>(statelessCommunicationFactory,
                statefulServiceUri);

            statefullClientFactory = new RestServicePartitionClientFactory<WebApi2>(statefullCommunicationFactory,
                statefulServiceUri, TargetReplicaSelector.RandomReplica);
        }    

        // GET api/values 
        public async Task<IEnumerable<string>> Get()
        {
            var partitionClient = statelessClientFactory.Create();

            var result = await partitionClient.InvokeWithRetryAsync(
                async c => await c.ServiceClient.Values.GetAllAsync());

            return result;
        }

        // GET api/values/5 
        public async Task<string> Get(int id)
        {
            var partitionClient = statefullClientFactory.Create(new ServicePartitionKey(1));

            var result = await partitionClient.InvokeWithRetryAsync(
                async c => await c.ServiceClient.Values.GetAsync(id));

            return result;
        }

        // POST api/values 
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5 
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5 
        public void Delete(int id)
        {
        }
    }
}

using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using ServiceFabric.AutoRest.Communication;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Rest;
using WebApi.AutoRest.Client;
using WebApiStateful.AutoRest.Client;
using ServiceFabric.AutoRest.Communication.Client;

namespace ClientService.Controllers
{
    [ServiceRequestActionFilter]
    public class MyValuesController : ApiController
    {
        private static readonly Uri statelessServiceUri = new Uri("fabric:/ServiceFabric.AutoRest.Clients/WebApi");
        private static readonly Uri statefulServiceUri = new Uri("fabric:/ServiceFabric.AutoRest.Clients/WebApiStateful");        
        private static readonly IRestServicePartitionClientFactory<StatelessClient> statelessClientFactory;
        private static readonly IRestServicePartitionClientFactory<StatefullClient> statefullClientFactory;

        static MyValuesController()
        {
            var statelessCommunicationFactory =
                new RestCommunicationClientFactory<StatelessClient>();
            var statefullCommunicationFactory =
                new RestCommunicationClientFactory<StatefullClient>(delegatingHandlers: () => new[] {new MyHandler()},
                    credentials: new BasicAuthenticationCredentials { UserName = "user", Password = "password" });

            statelessClientFactory = new RestServicePartitionClientFactory<StatelessClient>(statelessCommunicationFactory,
                statelessServiceUri);

            statefullClientFactory = new RestServicePartitionClientFactory<StatefullClient>(statefullCommunicationFactory,
                statefulServiceUri, TargetReplicaSelector.RandomReplica);
        }    

        // GET api/values 
        public async Task<IEnumerable<string>> Get()
        {
            var partitionClient = statelessClientFactory.Create();

            var result = await partitionClient.InvokeWithRetryAsync(
                async c => await c.RestApi.Values.GetAllAsync());

            return result;
        }

        // GET api/values/5 
        public string Get(int id)
        {
            var partitionClient = statefullClientFactory.Create(new ServicePartitionKey(1));

            var result = partitionClient.InvokeWithRetry(c => c.RestApi.Values.Get(id));

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

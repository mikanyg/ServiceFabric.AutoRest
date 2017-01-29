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

namespace ClientService.Controllers
{
    [ServiceRequestActionFilter]
    public class ValuesController : ApiController
    {
        private static readonly Uri statelessServiceUri = new Uri("fabric:/ServiceFabric.AutoRest.Clients/WebApi");
        private static readonly Uri statefulServiceUri = new Uri("fabric:/ServiceFabric.AutoRest.Clients/WebApiStateful");        
        private static readonly RestCommunicationClientFactory<StatelessClient> statelessCommunicationFactory;
        private static readonly RestCommunicationClientFactory<StatefullClient> statefullCommunicationFactory;

        static ValuesController()
        {
            statelessCommunicationFactory =
                new RestCommunicationClientFactory<StatelessClient>();
            statefullCommunicationFactory =
                new RestCommunicationClientFactory<StatefullClient>(delegatingHandlers: () => new[] {new MyHandler()},
                    credentials: new BasicAuthenticationCredentials {UserName = "user", Password = "password"});
        }    

        // GET api/values 
        public async Task<IEnumerable<string>> Get()
        {
            IRestServicePartitionClient<StatelessClient> partitionClient =
                new RestServicePartitionClient<StatelessClient>(statelessCommunicationFactory, statelessServiceUri,
                    ServicePartitionKey.Singleton);

            var result = await partitionClient.InvokeWithRetryAsync(
                async c => await c.RestApi.Values.GetAllAsync());

            return result;
        }

        // GET api/values/5 
        public async Task<string> Get(int id)
        {
            var partitionClient =
                new ServicePartitionClient<RestCommunicationClient<StatefullClient>>(statefullCommunicationFactory,
                    statefulServiceUri, new ServicePartitionKey(1), TargetReplicaSelector.RandomReplica);

            var result = await partitionClient.InvokeWithRetryAsync(
                async c => await c.RestApi.Values.GetAsync(id));

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

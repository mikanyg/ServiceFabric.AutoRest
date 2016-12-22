using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using ServiceFabric.AutoRest.Communication;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading.Tasks;
using System.Web.Http;
using WebApi.AutoRest.Client;

namespace ClientService.Controllers
{
    [ServiceRequestActionFilter]
    public class ValuesController : ApiController
    {
        private static readonly Uri serviceUri = new Uri("fabric:/ServiceFabric.AutoRest.Clients/WebApi");
        private static AutoRestCommunicationClientFactory<WebApiClient> communicationFactory;

        static ValuesController()
        {
            communicationFactory = new AutoRestCommunicationClientFactory<WebApiClient>();
        }    

        // GET api/values 
        public async Task<IEnumerable<string>> Get()
        {            
            var partitionClient = new ServicePartitionClient<AutoRestCommunicationClient<WebApiClient>>(communicationFactory, serviceUri, ServicePartitionKey.Singleton);

            var result = await partitionClient.InvokeWithRetryAsync(
                async c => await c.ServiceClient.Values.GetAllAsync());

            return result;
        }

        // GET api/values/5 
        public string Get(int id)
        {
            return "value";
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

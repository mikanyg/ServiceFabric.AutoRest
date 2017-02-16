using ServiceFabric.AutoRest.Communication.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using WebApi.AutoRest.Client;
using WebApiStateful.AutoRest.Client;

namespace ClientService.Controllers
{
    [ServiceRequestActionFilter]
    public class StatelessController : ApiController
    {
        private static readonly Uri serviceUri = new Uri("fabric:/ServiceFabric.AutoRest.Sample/WebApi");
        private static readonly RestCommunicationClientFactory<StatelessClient> communicationFactory = new RestCommunicationClientFactory<StatelessClient>();        

        // GET api/stateless
        public async Task<IEnumerable<string>> Get()
        {
            var partitionClient = new RestServicePartitionClient<StatelessClient>(communicationFactory, serviceUri);

            var result = await partitionClient.InvokeWithRetryAsync(
                async c => await c.RestApi.Values.GetAllAsync());

            return result;
        }                
    }
}

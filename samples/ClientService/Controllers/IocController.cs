using Microsoft.ServiceFabric.Services.Client;
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
    public class IocController : ApiController
    {        
        private IRestServicePartitionClient<StatelessClient> partitionClient;

        public IocController(IRestServicePartitionClient<StatelessClient> partitionClient)
        {
            this.partitionClient = partitionClient;
        }

        // GET api/stateless
        public async Task<IEnumerable<string>> Get()
        {
            var result = await partitionClient.InvokeWithRetryAsync(
                async c => await c.RestApi.Values.GetAllAsync());

            return result;
        }                
    }
}

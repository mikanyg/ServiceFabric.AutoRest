using Microsoft.Rest;
using Microsoft.ServiceFabric.Services.Client;

namespace ServiceFabric.AutoRest.Communication
{
    public interface IRestServicePartitionClientFactory<TClient> where TClient : ServiceClient<TClient>
    {
        IRestServicePartitionClient<TClient> Create(ServicePartitionKey partitionKey = null);
    }
}
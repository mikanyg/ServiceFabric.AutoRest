using System;
using Microsoft.Rest;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;

namespace ServiceFabric.AutoRest.Communication.Client
{
    public class RestServicePartitionClient<TClient> : ServicePartitionClient<RestCommunicationClient<TClient>>, IRestServicePartitionClient<TClient>
        where TClient : ServiceClient<TClient>
    {
        public RestServicePartitionClient(
            ICommunicationClientFactory<RestCommunicationClient<TClient>> communicationClientFactory, 
            Uri serviceUri,
            ServicePartitionKey partitionKey = null, 
            TargetReplicaSelector targetReplicaSelector = TargetReplicaSelector.Default, 
            string listenerName = null, 
            OperationRetrySettings retrySettings = null)
            : base(communicationClientFactory, serviceUri, partitionKey, targetReplicaSelector, listenerName, retrySettings)
        {
            if (communicationClientFactory == null) throw new ArgumentNullException(nameof(communicationClientFactory));
            if (serviceUri == null) throw new ArgumentNullException(nameof(serviceUri));
        }
    }
}

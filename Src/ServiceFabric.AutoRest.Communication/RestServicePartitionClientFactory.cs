using System;
using Microsoft.Rest;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;

namespace ServiceFabric.AutoRest.Communication.Client
{
    public class RestServicePartitionClientFactory<TClient> : IRestServicePartitionClientFactory<TClient>
        where TClient : ServiceClient<TClient>
    {
        private readonly ICommunicationClientFactory<RestCommunicationClient<TClient>> communicationClientFactory;
        private readonly Uri serviceUri;
        private readonly TargetReplicaSelector targetReplicaSelector;
        private readonly string listenerName;
        private readonly OperationRetrySettings retrySettings;

        public RestServicePartitionClientFactory(
            ICommunicationClientFactory<RestCommunicationClient<TClient>> communicationClientFactory, 
            Uri serviceUri, 
            TargetReplicaSelector targetReplicaSelector = TargetReplicaSelector.Default,
            string listenerName = null, 
            OperationRetrySettings retrySettings = null)
        {
            this.communicationClientFactory = communicationClientFactory;
            this.serviceUri = serviceUri;
            this.targetReplicaSelector = targetReplicaSelector;
            this.listenerName = listenerName;
            this.retrySettings = retrySettings;
        }

        public IRestServicePartitionClient<TClient> Create(ServicePartitionKey partitionKey = null)
        {
            return new RestServicePartitionClient<TClient>(communicationClientFactory, serviceUri, partitionKey,
                targetReplicaSelector, listenerName, retrySettings);
        }
    }
}
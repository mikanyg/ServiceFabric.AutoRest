using Microsoft.ServiceFabric.Services.Communication.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.Rest;

namespace ServiceFabric.AutoRest.Communication
{
    public class AutoRestServicePartitionClient<TClient> : ServicePartitionClient<AutoRestCommunicationClient<TClient>>, IRestServicePartitionClient<TClient>
        where TClient : ServiceClient<TClient>
    {
        public AutoRestServicePartitionClient(
            ICommunicationClientFactory<AutoRestCommunicationClient<TClient>> communicationClientFactory, Uri serviceUri,
            ServicePartitionKey partitionKey = null, TargetReplicaSelector targetReplicaSelector = TargetReplicaSelector.Default, 
            string listenerName = null, OperationRetrySettings retrySettings = null)
            : base(communicationClientFactory, serviceUri, partitionKey, targetReplicaSelector, listenerName, retrySettings)
        {
        }
    }
}

using Microsoft.ServiceFabric.Services.Communication.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Client;

namespace ServiceFabric.AutoRest.Communication
{
    public class ReliableServiceClient<TCommunicationClient> : ServicePartitionClient<TCommunicationClient>, IReliableServiceClient<TCommunicationClient>
        where TCommunicationClient : ICommunicationClient
    {
        public ReliableServiceClient(
            ICommunicationClientFactory<TCommunicationClient> communicationClientFactory, 
            Uri serviceUri, 
            ServicePartitionKey partitionKey = null, 
            TargetReplicaSelector targetReplicaSelector = TargetReplicaSelector.Default, 
            string listenerName = null, 
            OperationRetrySettings retrySettings = null) : base(communicationClientFactory, serviceUri, partitionKey, targetReplicaSelector, listenerName, retrySettings)
        {
        }
    }
}

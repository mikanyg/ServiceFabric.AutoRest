using Microsoft.ServiceFabric.Services.Communication.Client;

namespace ServiceFabric.AutoRest.Communication.Client
{
    public class CommunicationClientValidatingEventArgs<TCommunicationClient> : CommunicationClientEventArgs<TCommunicationClient>
        where TCommunicationClient : ICommunicationClient
    {
        public bool IsValid { get; set; }
    }
}
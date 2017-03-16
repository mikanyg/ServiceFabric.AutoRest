using Microsoft.Rest;
using ServiceFabric.AutoRest.Communication.Client;
using System.Threading.Tasks;

namespace ServiceFabric.AutoRest.Communication.Test.Customization
{
    class CustomCredentialsManager : ICredentialsManager
    {
        public Task<ServiceClientCredentials> GetCredentialsAsync()
        {
            return Task.FromResult(new BasicAuthenticationCredentials() as ServiceClientCredentials);
        }
    }
}

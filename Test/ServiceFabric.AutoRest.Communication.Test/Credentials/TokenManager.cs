using System;
using System.Threading.Tasks;
using Microsoft.Rest;
using ServiceFabric.AutoRest.Communication.Client;
using ServiceFabric.AutoRest.TestClient.Auth;

namespace ServiceFabric.AutoRest.Communication.Test.Credentials
{
    internal class TokenManager : ICredentialsManager
    {
        private TokenResponse response;

        public bool ExpireTokenImmediately { get; set; }
        
        public async Task<ServiceClientCredentials> GetCredentialsAsync()
        {
            response = await GetTokenFromSomewhereAsync(); 
            return new TokenCredentials(response.Token, "Bearer");
        }

        internal void SubscribeToEvents(RestCommunicationClientFactory<WebApi2> factory)
        {
            factory.ClientCreated += (sender, args) =>
            {
                if (response != null)
                {
                    args.Client.Properties.Add("ExpiresAt", response.Expiration);
                }
            };

            factory.ClientValidating += (sender, args) =>
            {
                if (!args.IsValid) return; // shortcut if another event handler has invalidated the client

                if (args.Client.Properties.ContainsKey("ExpiresAt"))
                {
                    args.IsValid = !ExpireTokenImmediately;
                }
            };
        }

        private Task<TokenResponse> GetTokenFromSomewhereAsync()
        {
            var obj = new TokenResponse
            {
                Token = Guid.NewGuid().ToString(),
                Expiration = DateTime.UtcNow.AddHours(1)
            };
            return Task.FromResult(obj);
        }
    }
}
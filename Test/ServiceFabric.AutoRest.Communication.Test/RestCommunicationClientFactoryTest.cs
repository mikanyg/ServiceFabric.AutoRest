using Microsoft.Rest;
using Microsoft.ServiceFabric.Services.Communication.Client;
using NUnit.Framework;
using ServiceFabric.AutoRest.Communication.Client;
using ServiceFabric.AutoRest.Communication.Test.Customization;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AuthWebApi = ServiceFabric.AutoRest.TestClient.Auth.WebApi2;
using NoAuthWebApi = ServiceFabric.AutoRest.TestClient.NoAuth.WebApi2;

namespace ServiceFabric.AutoRest.Communication.Test
{
    [TestFixture]
    public class RestCommunicationClientFactoryTest
    {
        class TestableFactory<T> : RestCommunicationClientFactory<T> where T : ServiceClient<T>
        {
            public TestableFactory(ServiceClientCredentials creds = null, ICredentialsManager manager = null) : base(credentials: creds, credentialsManager: manager) { }

            public new Task<RestCommunicationClient<T>> CreateClientAsync(string endpoint, CancellationToken cancellationToken)
            {
                return base.CreateClientAsync(endpoint, cancellationToken);
            }
        }

        private IEnumerable<IExceptionHandler> CustomExceptionHandlers => new List<IExceptionHandler> { new CustomExceptionHandler() };

        [Test]
        public void NewInstance_NoAuthRequired_IsNotNull()
        {
            var sut = new RestCommunicationClientFactory<NoAuthWebApi>();
            sut.ShouldNotBeNull();
        }

        [TestCase("username", "password")]
        public void NewInstance_AuthRequired_WithBasicAuthentication_IsNotNull(string username, string password)
        {
            var basicAuth = new BasicAuthenticationCredentials() { UserName = username, Password = password };
            var sut = new RestCommunicationClientFactory<AuthWebApi>(credentials: basicAuth);
            sut.ShouldNotBeNull();
        }

        [TestCase("###token###")]
        public void NewInstance_AuthRequired_WithTokenCredentials_IsNotNull(string token)
        {
            var tokenCreds = new TokenCredentials(token);
            var sut = new RestCommunicationClientFactory<AuthWebApi>(credentials: tokenCreds);
            sut.ShouldNotBeNull();
        }

        [Test]
        public void NewInstance_AuthRequired_WithCredentialsManager_IsNotNull()
        {
            var sut = new RestCommunicationClientFactory<AuthWebApi>(credentialsManager: new CustomCredentialsManager());
            sut.ShouldNotBeNull();
        }

        [Test]
        public void NewInstance_AuthRequired_NoAuthentication_ThrowsArgumentException()
        {
            Should.Throw<ArgumentException>(() => new RestCommunicationClientFactory<AuthWebApi>());
        }

        [Test]
        public void NewInstance_NoCustomExceptionHandling_HttpOperationExceptionHandlerIncluded()
        {
            var sut = new RestCommunicationClientFactory<NoAuthWebApi>();
            sut.ExceptionHandlers.ShouldContain(handler => handler.GetType() == typeof(HttpOperationExceptionHandler));
        }

        [Test]
        public void NewInstance_WithCustomExceptionHandling_HttpOperationExceptionHandlerIncluded()
        {
            var sut = new RestCommunicationClientFactory<NoAuthWebApi>(exceptionHandlers: CustomExceptionHandlers);
            sut.ExceptionHandlers.ShouldContain(handler => handler.GetType() == typeof(HttpOperationExceptionHandler));
        }

        [Test]
        public void NewInstance_WithCustomExceptionHandling_CustomExceptionHandlerIncluded()
        {
            var sut = new RestCommunicationClientFactory<NoAuthWebApi>(exceptionHandlers: CustomExceptionHandlers);
            sut.ExceptionHandlers.ShouldContain(handler => handler.GetType() == typeof(CustomExceptionHandler));
        }

        [Test]
        public void NewInstance_WithCustomExceptionHandling_CustomExceptionHandlerBeforeDefaultExceptionHandler()
        {
            var sut = new RestCommunicationClientFactory<NoAuthWebApi>(exceptionHandlers: CustomExceptionHandlers);

            sut.ExceptionHandlers.Count().ShouldBe(2);
            sut.ExceptionHandlers.First().ShouldBeOfType<CustomExceptionHandler>();
        }

        [TestCase("http://localhost/serviceendpoint")]
        public async Task CreateClientAsync_WithCredentialsManager_ClientIsNotNull(string endpoint)
        {
            var sut = new TestableFactory<AuthWebApi>(manager: new CustomCredentialsManager());
            var client = await sut.CreateClientAsync(endpoint, CancellationToken.None);
            client.ShouldNotBeNull();
        }

        [TestCase("http://localhost/serviceendpoint", "###token###")]
        public async Task CreateClientAsync_WithTokenCredentials_ClientIsNotNull(string endpoint, string token)
        {
            var sut = new TestableFactory<AuthWebApi>(new TokenCredentials(token));
            var client = await sut.CreateClientAsync(endpoint, CancellationToken.None);
            client.ShouldNotBeNull();
        }

        [TestCase("http://localhost/serviceendpoint")]
        public async Task CreateClientAsync_NoAuthentication_ClientIsNotNull(string endpoint)
        {
            var sut = new TestableFactory<NoAuthWebApi>();
            var client = await sut.CreateClientAsync(endpoint, CancellationToken.None);
            client.ShouldNotBeNull();
        }
    }         
}

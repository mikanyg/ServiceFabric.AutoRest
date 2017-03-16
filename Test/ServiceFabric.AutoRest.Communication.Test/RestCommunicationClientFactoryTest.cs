using Microsoft.Rest;
using Microsoft.ServiceFabric.Services.Communication.Client;
using NUnit.Framework;
using ServiceFabric.AutoRest.Communication.Client;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using AuthWebApi = ServiceFabric.AutoRest.TestClient.Auth.WebApi2;
using NoAuthWebApi = ServiceFabric.AutoRest.TestClient.NoAuth.WebApi2;

namespace ServiceFabric.AutoRest.Communication.Test
{
    class CustomExceptionHandler : IExceptionHandler
    {
        public bool TryHandleException(ExceptionInformation exceptionInformation, OperationRetrySettings retrySettings, out ExceptionHandlingResult result)
        {
            result = new ExceptionHandlingThrowResult();
            return true;
        }
    }

    [TestFixture]
    public class RestCommunicationClientFactoryTest
    {
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
    }    
}

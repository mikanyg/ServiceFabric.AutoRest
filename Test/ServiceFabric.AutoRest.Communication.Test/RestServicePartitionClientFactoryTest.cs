using Microsoft.ServiceFabric.Services.Client;
using NUnit.Framework;
using ServiceFabric.AutoRest.Communication.Client;
using Shouldly;
using System;
using NoAuthWebApi = ServiceFabric.AutoRest.TestClient.NoAuth.WebApi2;

namespace ServiceFabric.AutoRest.Communication.Test
{
    [TestFixture]
    public class RestServicePartitionClientFactoryTest
    {
        private RestServicePartitionClientFactory<NoAuthWebApi> sut;

        [SetUp]
        public void Setup()
        {
            sut = new RestServicePartitionClientFactory<NoAuthWebApi>(new RestCommunicationClientFactory<NoAuthWebApi>(), new Uri("fabric:/app/service"));
        }
                
        [Test]
        public void Create_NoPartitionKey_ClientNotNull()
        {
            var client = sut.Create();
            client.ShouldNotBeNull();
        }

        [Test]
        public void Create_SingletonPartitionKey_ClientNotNull()
        {
            var client = sut.Create(ServicePartitionKey.Singleton);
            client.ShouldNotBeNull();
        }

        [Test]
        public void Create_LongPartitionKey_ClientNotNull()
        {
            var client = sut.Create(new ServicePartitionKey(long.MinValue));
            client.ShouldNotBeNull();
        }

        [Test]
        public void Create_StringPartitionKey_ClientNotNull()
        {
            var client = sut.Create(new ServicePartitionKey("partitionKey"));
            client.ShouldNotBeNull();
        }
    }
}

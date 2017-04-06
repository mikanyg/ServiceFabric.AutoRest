using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ServiceFabric.AutoRest.Communication.Client;
using ServiceFabric.AutoRest.TestClient.NoAuth;
using Shouldly;

namespace ServiceFabric.AutoRest.Communication.Test.Clients
{
    [TestFixture]
    public class RestCommunicationClientTest
    {
        private RestCommunicationClient<WebApi2> sut;

        [SetUp]
        public void Setup()
        {
            sut = new RestCommunicationClient<WebApi2>(new WebApi2());
        }

        [Test]
        public void NewInstance_NullPassedAsTypedClientInstance_ThrowsArgumentNullException()
        {
            Should.Throw<ArgumentNullException>(() => sut = new RestCommunicationClient<WebApi2>(null));
        }

        [Test]
        public void NewInstance_TypedClientInstancePassed_AvailableThroughProperty()
        {
            var typedClient = new WebApi2();
            sut = new RestCommunicationClient<WebApi2>(typedClient);

            sut.RestApi.ShouldBe(typedClient);
        }

        [Test]
        public void NewInstance_PropertiesAreEmpty()
        {
            sut.Properties.ShouldBeEmpty();
        }
    }
}

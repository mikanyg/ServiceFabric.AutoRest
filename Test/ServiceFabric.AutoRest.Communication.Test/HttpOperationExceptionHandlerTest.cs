using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Rest;
using Microsoft.ServiceFabric.Services.Communication.Client;
using NUnit.Framework;
using Shouldly;

namespace ServiceFabric.AutoRest.Communication.Test
{
    [TestFixture]
    public class HttpOperationExceptionHandlerTest
    {
        private HttpOperationExceptionHandler sut;

        [SetUp]
        public void Setup()
        {
            sut = new HttpOperationExceptionHandler();
        }

        [Test]
        public void TryHandleException_404ResourceNotFoundHasResponseHeader_ExceptionIsNotHandled()
        {
            var ex = CreateHttpOperationException(HttpStatusCode.NotFound);
            ex.Response.Headers.Add("X-ServiceFabric", new[] {"ResourceNotFound"});

            ExceptionHandlingResult result;
            bool isHandled = sut.TryHandleException(new ExceptionInformation(ex), new OperationRetrySettings(), out result);

            isHandled.ShouldBeFalse();
        }

        [Test]
        public void TryHandleException_404ResourceNotFoundNoResponseHeader_ExceptionIsHandledAndShouldBeRetried()
        {
            var ex = CreateHttpOperationException(HttpStatusCode.NotFound);

            ExceptionHandlingResult result;
            bool isHandled = sut.TryHandleException(new ExceptionInformation(ex), new OperationRetrySettings(), out result);

            isHandled.ShouldBeTrue();
            result.ShouldBeOfType<ExceptionHandlingRetryResult>();
        }

        [Test]
        public void TryHandleException_503ServiceUnavailable_ExceptionIsHandledAndShouldBeRetried()
        {
            var ex = CreateHttpOperationException(HttpStatusCode.ServiceUnavailable);

            ExceptionHandlingResult result;
            bool isHandled = sut.TryHandleException(new ExceptionInformation(ex), new OperationRetrySettings(), out result);

            isHandled.ShouldBeTrue();
            result.ShouldBeOfType<ExceptionHandlingRetryResult>();
        }

        [Test]
        public void TryHandleException_400BadRequest_ExceptionIsNotHandled()
        {
            var ex = CreateHttpOperationException(HttpStatusCode.BadRequest);

            ExceptionHandlingResult result;
            bool isHandled = sut.TryHandleException(new ExceptionInformation(ex), new OperationRetrySettings(), out result);

            isHandled.ShouldBeFalse();
        }

        [Test]
        public void TryHandleException_IsNonHttpOperationException_ExceptionIsNotHandled()
        {
            var ex = new HttpListenerException();

            ExceptionHandlingResult result;
            bool isHandled = sut.TryHandleException(new ExceptionInformation(ex), new OperationRetrySettings(), out result);

            isHandled.ShouldBeFalse();
        }

        private static HttpOperationException CreateHttpOperationException(HttpStatusCode statusCode)
        {
            return new HttpOperationException
            {
                Response = new HttpResponseMessageWrapper(new HttpResponseMessage(statusCode), string.Empty)
            };
        }
    }
}

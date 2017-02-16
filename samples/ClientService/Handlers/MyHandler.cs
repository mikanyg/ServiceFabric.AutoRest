using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ClientService.Handlers
{
    public class MyHandler : DelegatingHandler
    {
        private static int instanceNumber = 0;

        public MyHandler()
        {
            instanceNumber++;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var watch = new Stopwatch();
            watch.Start();

            ServiceEventSource.Current.Message($"[MyHandler output] Request header {nameof(request.Headers.Authorization)}: {request.Headers.Authorization}");

            var response = await base.SendAsync(request, cancellationToken);

            watch.Stop();

            ServiceEventSource.Current.Message($"[MyHandler output] Time ellapsed for request: {watch.ElapsedMilliseconds} ms");

            return response;
        }
    }
}
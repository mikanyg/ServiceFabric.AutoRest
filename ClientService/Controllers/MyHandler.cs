using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ClientService.Controllers
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

            var response = await base.SendAsync(request, cancellationToken);

            watch.Stop();

            ServiceEventSource.Current.Message($"Time ellapsed for request: {watch.ElapsedMilliseconds} ms");

            return response;
        }
    }
}
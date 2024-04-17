using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TVScanner.Shared.Tests
{
    public class TestHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            switch (request.RequestUri.AbsoluteUri)
            {
                case TestHelper.ScanUrl + "crypto/scan":
                    return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = new StringContent(TestHelper.GetScanResult.ToJson()) });
                case TestHelper.NotificationUrl:
                    return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK));
            }

            return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError));
        }
    }
}

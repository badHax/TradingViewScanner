using Microsoft.Extensions.Options;
using TVScanner.Shared.Configuration;
using TVScanner.Shared.Scanner;

namespace TVScanner.Shared.Notifications
{
    public class NotificationService
    {
        private const string _notificationTitle = "{0} Criteria Matched!";
        private const string _notificationAdded = "{0} is on the move";
        private readonly HttpClient _httpClient;
        private readonly string _urlBase;

        public NotificationService(IOptions<AppConfig> config, IHttpClientFactory httpClient)
        {
            var options = config.Value.NotificationConfig;
            _urlBase = $"{options.PushUrlBase.Replace("{_apiKey}", options.ApiKey).Replace("{_deviceId}", options.DeviceId)}";

            _httpClient = httpClient.CreateClient(nameof(NotificationService)) ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task SendPushNotification(object data, ScanType type)
        {
            // add title, body and link to _pushUrlBase
            var url = _urlBase + $"&title={string.Format(_notificationTitle, type.ToString())}&text={string.Format(_notificationAdded, data)}&url=https://www.binance.com/en/futures/{data}";

            var result = await _httpClient.GetAsync(url);
            result.EnsureSuccessStatusCode();
        }
    }
}

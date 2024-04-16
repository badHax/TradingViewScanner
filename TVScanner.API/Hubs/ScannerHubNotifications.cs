using Microsoft.AspNetCore.SignalR;
using TVScanner.Shared.Scanner;
using TVScanner.Shared;

namespace TVScanner.API.Hubs
{
    public class ScannerHubNotifications : HubNotifications<ScannerHub>
    {
        public ScannerHubNotifications(IHubContext<ScannerHub> hubContext, ICache _cache)
            : base(hubContext, _cache)
        {
        }

        protected override void Subscribe()
        {
            _cache.Subscribe<IEnumerable<ScanRecord>>(Constants.MessageListeners.RelativeVolume, async (message) =>
            {
                await _hubContext.Clients.All.SendAsync(Constants.MessageListeners.RelativeVolume, message);
            });

            _cache.Subscribe<IEnumerable<ScanRecord>>(Constants.MessageListeners.HighOfDay, async (message) =>
            {
                await _hubContext.Clients.All.SendAsync(Constants.MessageListeners.RelativeVolume, message);
            });
        }

        public static void StartListening(IHubContext<ScannerHub> hubContext, ICache cache)
        {
            Instance = new ScannerHubNotifications(hubContext, cache);
        }
    }
}

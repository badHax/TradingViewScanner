using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.SignalR;
using TVScanner.Shared;
using TVScanner.Shared.Scanner;

namespace TVScanner.API.Hubs
{
    [Authorize]
    public class ScannerHub : MessageHubBase<IScanUpdater>
    {
        private readonly ICache _cache;

        public ScannerHub(ICache cache)
        {
            _cache = cache;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            var relativeVolume = _cache.Get<List<ScanRecord>>(Constants.MessageListeners.RelativeVolume) ?? new List<ScanRecord>();
            var highOfDay = _cache.Get<List<ScanRecord>>(Constants.MessageListeners.HighOfDay) ?? new List<ScanRecord>();

            if (relativeVolume.Any())
            {
                await Clients.Caller.realtiveVolume(relativeVolume);
            }

            if (highOfDay.Any())
            {
                await Clients.Caller.highOfDay(highOfDay);
            }
        }
    }
}

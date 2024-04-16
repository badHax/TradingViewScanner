using Microsoft.AspNetCore.SignalR;
using TVScanner.Shared;
using TVScanner.Shared.Scanner;

namespace TVScanner.API.Hubs
{
    public abstract class HubNotifications<THub> where THub : Hub<IScanUpdater>
    {
        protected readonly IHubContext<THub> _hubContext;
        protected readonly ICache _cache;
        protected static HubNotifications<THub> Instance;

        public HubNotifications(IHubContext<THub> hubContext, ICache cache)
        {
            _hubContext = hubContext;
            _cache = cache;
            Subscribe();
        }

        protected abstract void Subscribe();
    }
}

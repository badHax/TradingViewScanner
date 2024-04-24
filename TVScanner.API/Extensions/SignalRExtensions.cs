using Microsoft.AspNetCore.SignalR;
using TVScanner.API.Hubs;
using TVScanner.Shared;

namespace TVScanner.API.Extensions
{
    public static class SignalRExtensions
    {
        public static IServiceCollection AddSignalRMessages(this IServiceCollection services)
        {
            services.AddSignalR();
            services.AddSingleton<SignalRMessageBuilder>();
            return services;
        }

        public static IServiceCollection AddSignalRMessages(this IServiceCollection services, Action<HubOptions> options)
        {
            services.AddSignalR(options);
            services.AddSingleton<SignalRMessageBuilder>();
            return services;
        }

        public class SignalRMessageBuilder
        {
            public SignalRMessageBuilder(IHubContext<ScannerHub> scannerHub, ICache cache)
            {
                ScannerHubNotifications.StartListening(scannerHub, cache);
            }
        }
    }
}

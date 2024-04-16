using Majorsoft.Blazor.Components.Notifications;
using Microsoft.AspNetCore.Components;

namespace TVScanner.Services
{
    public class NotificationService(IHtmlNotificationService htmlNotificationService, NavigationManager navigationManager) : IAsyncDisposable
    {
        private string _notificationIcon = "apple-icon.png";
        public async Task ShowDesktopNotification(string title, string body, object data)
        {
            var js = "notification-worker.js";
            var uri = navigationManager.BaseUri + js;
            var options = new HtmlServiceWorkerNotificationOptions(title, uri)
            {
                Body = body,
                Icon = _notificationIcon,
                Vibrate = new int[] { 100, 200, 100 },
                RequireInteraction = true,
                //events
                Actions =
                [
                   new NotificationAction() { Action = "openLinkInNewTab", Title = "View on Charts" },
                ],
                Data = new { Url = $"https://www.binance.com/en/futures/{data}" }
            };

            await htmlNotificationService.ShowsWithActionsAsync(options);
        }

        public async ValueTask DisposeAsync()
        {
            if (htmlNotificationService is not null)
            {
                await htmlNotificationService.DisposeAsync();
            }
        }
    }
}

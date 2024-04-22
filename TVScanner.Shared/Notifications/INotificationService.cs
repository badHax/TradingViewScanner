using TVScanner.Shared.Scanner;

namespace TVScanner.Shared.Notifications
{
    public interface INotificationService
    {
        Task SendPushNotification(object data, ScanType type);
    }
}
namespace TVScanner.Shared.Configuration
{
    using System.ComponentModel.DataAnnotations;

    public class ScannerConfig
    {
        [Required(AllowEmptyStrings = false)]
        public required string Url { get; set; }
        [Required(AllowEmptyStrings = false)]
        public int Interval { get; set; }
        [Required(AllowEmptyStrings = false)]
        public required int PurgeAfterDays { get; set; }
        [Required(AllowEmptyStrings = false)]
        public required string PurgeSchedule { get; set; }
    }

    public class NotificationConfg
    {
        [Required(AllowEmptyStrings = false)]
        public required string DeviceId { get; set; }
        [Required(AllowEmptyStrings = false)]
        public required string ApiKey { get; set; }
        [Required(AllowEmptyStrings = false)]
        public required string PushUrlBase { get; set; }
    }

    public class AppConfig
    {
        [Required(AllowEmptyStrings = false)]
        public ScannerConfig ScannerConfig { get; set; }
        [Required(AllowEmptyStrings = false)]
        public NotificationConfg NotificationConfig { get; set; }
    }

}

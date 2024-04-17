using System.Collections.Generic;
using TVScanner.Shared.Scanner;

namespace TVScanner.Shared.Tests
{
    public static class TestHelper
    {
        public const string ScanUrl = "https://api.scan.com/";
        public const string NotificationUrl = "https://api.notification.com/";
        public static ScanResult GetScanResult => new ScanResult
        {
            Data = new DataItem[]
            {
                new DataItem
                {
                    D = new List<object>
                    {
                        "BaseCurrencyLogoId",
                        "CurrencyLogoId",
                        "BTCUSDT",
                        1.0f,
                        2.0f,
                        3.0f,
                        4.0f,
                        "Description",
                        "Type",
                        "Subtype",
                        "UpdateMode",
                        "Exchange",
                        5.0f,
                        6.0f,
                        true,
                        7.0f,
                    }
                }
            }
        };
    }
}

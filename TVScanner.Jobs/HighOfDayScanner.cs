using Microsoft.Extensions.DependencyInjection;
using TVScanner.Shared;
using TVScanner.Shared.Scanner;

namespace TVScanner.Jobs
{
    public class HighOfDayScanner : Scanner
    {
        public HighOfDayScanner(IServiceScopeFactory scopeFactory)
            : base(
                  scopeFactory,
                  new ScanFilter(ScanType.HighOfDay),
                  Constants.MessageListeners.HighOfDay)
        {
        }

        protected override string ScannerName => nameof(HighOfDayScanner);
    }
}

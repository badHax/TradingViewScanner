using Microsoft.Extensions.DependencyInjection;
using TVScanner.Shared;
using TVScanner.Shared.Scanner;

namespace TVScanner.Jobs
{
    public class RelativeVolumeScanner : Scanner
    {
        public RelativeVolumeScanner(IServiceScopeFactory scopeFactory)
            : base(
                  scopeFactory,
                  new ScanFilter(ScanType.RelativeVolume),
                  Constants.MessageListeners.RelativeVolume)
        {
        }

        protected override string ScannerName => nameof(RelativeVolumeScanner);
    }
}

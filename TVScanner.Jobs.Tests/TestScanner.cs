using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Threading;
using TVScanner.Shared.Scanner;

namespace TVScanner.Jobs.Tests
{
    public class TestScanner : Scanner
    {
        public TestScanner(
        IServiceScopeFactory scopeFactory,
        ScanFilter scanFilter,
        string messageType) : base(scopeFactory, scanFilter, messageType)
        {
        }

        public Task PublicExecuteAsync(CancellationToken stoppingToken)
        {
            return base.ExecuteAsync(stoppingToken);
        }

        public Task<ScanRecord[]> PublicGetScanResult()
        {
            return base.GetScanResult();
        }

        public string PublicScannerName => this.ScannerName;

        protected override string ScannerName { get; }
    }
}

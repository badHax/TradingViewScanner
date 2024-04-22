
namespace TVScanner.Shared.Scanner
{
    public interface IScanService
    {
        Task<ScanRecord[]> GetFilteredResult(ScanFilter filter);
    }
}
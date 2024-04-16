
namespace TVScanner.Shared.Scanner
{
    public interface IScanRecordManager
    {
        IEnumerable<ScanRecord> GetHistoricalRecords(HistoricalPeriod period, ScanType scanType);
        IEnumerable<ScanRecord> GetRecentRecords(ScanType scanType);
        Task PurgeOldRecords();
        Task UpdateRecords(IEnumerable<ScanRecord> records);
    }
}
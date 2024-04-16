namespace TVScanner.Shared.Scanner
{
    public interface IScanUpdater
    {
        Task realtiveVolume(IEnumerable<ScanRecord> message);
        Task highOfDay(IEnumerable<ScanRecord> message);
    }
}

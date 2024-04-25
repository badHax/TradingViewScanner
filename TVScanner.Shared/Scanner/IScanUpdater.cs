namespace TVScanner.Shared.Scanner
{
    public interface IScanUpdater
    {
        Task relativeVolume(IEnumerable<ScanRecord> message);
        Task highOfDay(IEnumerable<ScanRecord> message);
    }
}

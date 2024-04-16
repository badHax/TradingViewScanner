namespace TVScanner.Shared.Scanner
{
    public class ScanFilter
    {
        private readonly string _filter;
        private ScanType _scanType;

        public ScanFilter(ScanType scanType)
        {
            _scanType = scanType;

            var filterFile = scanType switch
            {
                ScanType.HighOfDay => "high-of-day-filter.json",
                ScanType.RelativeVolume => "relative-volume-filter.json",
                _ => throw new NotImplementedException()
            };

            var allResources = typeof(ScanService).Assembly.GetManifestResourceNames();
            var filterName = typeof(ScanService).Assembly.GetManifestResourceNames().FirstOrDefault(f => f.EndsWith(filterFile));
            using (var stream = typeof(ScanService).Assembly.GetManifestResourceStream(filterName!))
            using (var reader = new StreamReader(stream!))
            {
                _filter = reader.ReadToEnd();
            }
        }

        public string AsJson { get { return _filter; } }
        public ScanType Type { get { return _scanType; } }
    }
}

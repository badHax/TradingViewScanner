using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TVScanner.Shared.Configuration;

namespace TVScanner.Shared.Scanner
{
    public class ScanRecordManager : IScanRecordManager
    {
        private readonly DbSet<ScanRecord> _set;
        private readonly IRepositoryContext _context;
        private readonly int _purgeAfterDays;

        public ScanRecordManager(IRepositoryContext context, IOptions<AppConfig> config)
        {
            _set = context.Set<ScanRecord>();
            _context = context;
            _purgeAfterDays = config.Value.ScannerConfig.PurgeAfterDays;
        }

        public async Task PurgeOldRecords()
        {
            var cutoff = DateTime.UtcNow.AddDays(-_purgeAfterDays);
            var records = await _set.Where(r => r.LastUpdated < cutoff).ToListAsync();

            _set.RemoveRange(records);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRecords(IEnumerable<ScanRecord> records)
        {
            foreach (var record in records)
            {
                var existing =
                    _set.AsNoTracking().FirstOrDefault(r => r.Name == record.Name);

                if (existing == null)
                {
                    _set.Add(record);
                }
                else
                {
                    _set.Update(record);
                }
            }

            await _context.SaveChangesAsync();
        }

        public IEnumerable<ScanRecord> GetHistoricalRecords(HistoricalPeriod period, ScanType scanType)
        {
            var date = DateTime.UtcNow;
            var query = _set.AsNoTracking().Where(r => r.ScanFilter == scanType);
            switch (period)
            {
                case HistoricalPeriod.PastHour:
                    // Get hourly records
                    return query.Where(r => r.LastUpdated > date.AddHours(-1))?.ToList() ?? new List<ScanRecord>();
                case HistoricalPeriod.Today:
                    // Get daily records
                    return query.Where(r => r.LastUpdated > date.Date)?.ToList() ?? new List<ScanRecord>();
                case HistoricalPeriod.Yesterday:
                    // Get weekly records
                    return query.Where(r => r.LastUpdated > date.Date.AddDays(-1))?.ToList() ?? new List<ScanRecord>();
                case HistoricalPeriod.ThisWeek:
                    // Get weekly records
                    return query.Where(r => r.LastUpdated > date.Date.AddDays(-7))?.ToList() ?? new List<ScanRecord>();
                default:
                    throw new ArgumentOutOfRangeException(nameof(period), period, null);
            }
        }

        public IEnumerable<ScanRecord> GetRecentRecords(ScanType scanType)
        {
            var date = DateTime.UtcNow;
            return _set.AsNoTracking().Where(r => r.ScanFilter == scanType && r.LastUpdated > date.AddMinutes(-5))?.ToList() ?? new List<ScanRecord>();
        }
    }
}

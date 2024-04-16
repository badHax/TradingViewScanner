using System.Text.Json.Serialization;

namespace TVScanner.Shared.Scanner
{
    public class ScanRecord
    {
        [JsonPropertyName("base_currency_logoid")]
        public string BaseCurrencyLogoId { get; set; } = string.Empty;

        [JsonPropertyName("currency_logoid")]
        public string CurrencyLogoId { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("close|5")]
        public float Close5 { set { Close = value; } }
        public float Close { get; set; }

        [JsonPropertyName("change|5")]
        public float Change5 { set { Change = value; } }
        public float Change { get; set; }

        [JsonPropertyName("ATR|5")]
        public float ATR5 { set { ATR = value; } }
        public float ATR { get; set; }

        [JsonPropertyName("relative_volume_10d_calc|5")]
        public float RelativeVolume10DCalc5 { set { RelativeVolume10DCalc = value; } }
        public float RelativeVolume10DCalc { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("subtype")]
        public string Subtype { get; set; } = string.Empty;

        [JsonPropertyName("update_mode|5")]
        public string UpdateMode5 { set { UpdateMode = value; } }
        public string UpdateMode { get; set; } = string.Empty;

        [JsonPropertyName("exchange")]
        public string Exchange { get; set; } = string.Empty;

        [JsonPropertyName("pricescale")]
        public float Pricescale { get; set; }

        [JsonPropertyName("minmov")]
        public float Minmov { get; set; }

        [JsonPropertyName("fractional")]
        public bool Fractional { get; set; }

        [JsonPropertyName("minmove2")]
        public float Minmove2 { get; set; }

        public DateTime LastUpdated { get; set; }
        public ScanType ScanFilter { get; set; }
    }

    public class ScanResult
    {
        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }
        [JsonPropertyName("data")]
        public DataItem[] Data { get; set; } = Array.Empty<DataItem>();
    }

    public class DataItem
    {
        [JsonPropertyName("s")]
        public string S { get; set; }
        [JsonPropertyName("d")]
        public List<object> D { get; set; }
    }

    public class ScanRecordEqualityComparer : IEqualityComparer<ScanRecord>
    {
        public bool Equals(ScanRecord x, ScanRecord y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            return x?.Name == y?.Name;
        }

        public int GetHashCode(ScanRecord obj)
        {
            return obj.GetHashCode();
        }
    }

    public static class ScanRecordExtensions
    {
        public static bool EqualBySymbolNames(this IEnumerable<ScanRecord> first, IEnumerable<ScanRecord> second)
        {

            first = first.OrderBy(x => x.Name);
            second = second.OrderBy(x => x.Name);

            return first?.SequenceEqual(second, new ScanRecordEqualityComparer()) ?? false;
        }

        public static IEnumerable<ScanRecord> Except(this IEnumerable<ScanRecord> first, IEnumerable<ScanRecord> second, IEqualityComparer<ScanRecord> comparer)
        {
            return first.Where(x => !second.Contains(x, comparer));
        }
    }
}

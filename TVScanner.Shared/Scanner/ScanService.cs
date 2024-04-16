﻿using Microsoft.Extensions.Options;
using System.Text.Json;
using TVScanner.Shared.Configuration;

namespace TVScanner.Shared.Scanner
{
    public class ScanService
    {
        private readonly string BaseUrl;
        private readonly HttpClient _httpClient;

        public ScanService(IOptions<AppConfig> config)
        {
            BaseUrl = config.Value.ScannerConfig.Url;
            _httpClient = new HttpClient();
        }

        public async Task<ScanRecord[]> GetFilteredResult(ScanFilter filter)
        {
            var result = await _httpClient.PostAsync($"{BaseUrl}/crypto/scan", new StringContent(filter.AsJson));
            var content = await result.Content.ReadAsStringAsync();

            var scanResult = JsonSerializer.Deserialize<ScanResult>(content);
            var data = scanResult?.Data ?? Array.Empty<DataItem>();

            return data.Select(x => ParseDataItemToScanRecord(x, filter)).ToArray();
        }

        private ScanRecord ParseDataItemToScanRecord(DataItem item, ScanFilter filter)
        {
            var record = new ScanRecord();

            if (item.D.Count >= 16) // Ensure there are enough elements
            {
                record.BaseCurrencyLogoId = SafeToString(item.D[0]);
                record.CurrencyLogoId = SafeToString(item.D[1]);
                record.Name = SafeToString(item.D[2]).Replace(".P", "");

                // Directly set the primary properties instead of using the removed setters
                record.Close = SafeToFloat(item.D[3]);
                record.Change = SafeToFloat(item.D[4]);
                record.ATR = SafeToFloat(item.D[5]);
                record.RelativeVolume10DCalc = SafeToFloat(item.D[6]);

                record.Description = SafeToString(item.D[7]);
                record.Type = SafeToString(item.D[8]);
                record.Subtype = SafeToString(item.D[9]);
                record.UpdateMode = SafeToString(item.D[10]);
                record.Exchange = SafeToString(item.D[11]);
                record.Pricescale = SafeToFloat(item.D[12]);
                record.Minmov = SafeToFloat(item.D[13]);
                record.Fractional = SafeToBool(item.D[14]);
                record.Minmove2 = SafeToFloat(item.D[15]);
                record.LastUpdated = DateTime.Now;
                record.ScanFilter = filter.Type;
            }

            return record;
        }

        private string SafeToString(object obj) => obj?.ToString() ?? string.Empty;

        private float SafeToFloat(object obj) => float.TryParse(obj?.ToString(), out float value) ? value : 0;

        private bool SafeToBool(object obj) => bool.TryParse(obj?.ToString(), out bool value) && value;
    }
}

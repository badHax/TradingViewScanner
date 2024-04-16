using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TVScanner.Shared.Scanner;

namespace TVScanner.API.Controllers
{
    [Authorize]
    [Produces("application/json", Type = typeof(IEnumerable<ScanRecord>))]
    [Route("api/[controller]")]
    [ApiController]
    public class ScanRecords(IScanRecordManager scanRecordManager) : ControllerBase
    {
        [HttpGet]
        public IActionResult Historical(HistoricalPeriod period, ScanType scanType)
        {
            return Ok(scanRecordManager.GetHistoricalRecords(period, scanType));
        }
    }
}

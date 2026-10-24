using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PMS.Data;

namespace PMS.Controllers
{
    [ApiController]
    [Route("api/stats")]
    public class StatsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public StatsController(AppDbContext db) => _db = db;

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var today = DateTime.UtcNow.Date;
            var monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            var stats = new
            {
                TotalUsers             = await _db.Users.CountAsync(u => u.Status == "Active"),
                ActiveCases            = await _db.Cases.CountAsync(c => c.Status != "Closed"),
                ClosedCases            = await _db.Cases.CountAsync(c => c.Status == "Closed"),
                UrgentCases            = await _db.Cases.CountAsync(c => c.Status == "Urgent"),
                TotalEvidenceItems     = await _db.Evidences.CountAsync(),
                PendingForensicReports = await _db.ForensicReports.CountAsync(r => r.Status == "In Progress"),
                FinesIssuedToday       = await _db.Fines.CountAsync(f => f.IssuedAt >= today),
                FinesCollectedThisMonth = await _db.Fines
                    .Where(f => f.IssuedAt >= monthStart && f.Status == "Paid")
                    .SumAsync(f => (decimal?)f.Amount) ?? 0m,
                TotalArrests           = await _db.Arrests.CountAsync(),
                TotalSuspects          = await _db.Suspects.CountAsync(),
                TotalCitizenReports    = await _db.CitizenReports.CountAsync(),
                // New tables
                TotalWitnesses         = await _db.Witnesses.CountAsync(),
                ActivePatrols          = await _db.Patrols.CountAsync(p => p.Status == "Active"),
                ActiveMissingPersons   = await _db.MissingPersonReports.CountAsync(r => r.Status == "Active"),
                SuspendedLicenses      = await _db.DrivingLicenses.CountAsync(l => l.Status == "Suspended"),
                LockedAccounts         = await _db.Users.CountAsync(u => u.LockedUntil != null && u.LockedUntil > DateTime.UtcNow),
                RecentAuditEvents      = await _db.AuditLogs.CountAsync(l => l.Timestamp >= today),
            };

            return Ok(stats);
        }
    }
}

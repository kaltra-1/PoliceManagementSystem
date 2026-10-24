using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PMS.Data;
using PMS.DTOs;
using PMS.Models;

namespace PMS.Controllers
{
    [ApiController]
    [Route("api/forensic-reports")]
    public class ForensicReportsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ForensicReportsController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? caseId, [FromQuery] string? analystBadge, [FromQuery] string? status)
        {
            var query = _db.ForensicReports.AsQueryable();
            if (caseId.HasValue) query = query.Where(r => r.CaseId == caseId);
            if (!string.IsNullOrEmpty(analystBadge)) query = query.Where(r => r.AnalystBadge == analystBadge);
            if (!string.IsNullOrEmpty(status)) query = query.Where(r => r.Status == status);
            return Ok(await query.OrderByDescending(r => r.ReceivedAt).ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var r = await _db.ForensicReports.FindAsync(id);
            if (r == null) return NotFound();
            return Ok(r);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateForensicReportRequest req)
        {
            var count = await _db.ForensicReports.CountAsync();
            var report = new ForensicReport
            {
                ReportNumber = $"FR-{(count + 1):D4}",
                CaseId = req.CaseId,
                CaseNumber = req.CaseNumber,
                EvidenceNumber = req.EvidenceNumber,
                AnalysisType = req.AnalysisType,
                Findings = req.Findings,
                AnalystBadge = req.AnalystBadge,
                DueDate = req.DueDate,
                ProgressPercent = 0,
                Status = "In Progress"
            };

            _db.ForensicReports.Add(report);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = report.Id }, report);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateForensicReportRequest req)
        {
            var r = await _db.ForensicReports.FindAsync(id);
            if (r == null) return NotFound();

            r.ProgressPercent = req.ProgressPercent;
            r.Findings = req.Findings;
            r.Status = req.Status;

            if (req.Status == "Submitted")
                r.SubmittedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var r = await _db.ForensicReports.FindAsync(id);
            if (r == null) return NotFound();
            _db.ForensicReports.Remove(r);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PMS.Data;
using PMS.DTOs;
using PMS.Models;

namespace PMS.Controllers
{
    [ApiController]
    [Route("api/citizen-reports")]
    public class CitizenReportsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public CitizenReportsController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? citizenUserId, [FromQuery] string? status)
        {
            var query = _db.CitizenReports.AsQueryable();
            if (citizenUserId.HasValue) query = query.Where(r => r.CitizenUserId == citizenUserId);
            if (!string.IsNullOrEmpty(status)) query = query.Where(r => r.Status == status);
            return Ok(await query.OrderByDescending(r => r.SubmittedAt).ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var r = await _db.CitizenReports.FindAsync(id);
            if (r == null) return NotFound();
            return Ok(r);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCitizenReportRequest req)
        {
            var count = await _db.CitizenReports.CountAsync();
            var report = new CitizenReport
            {
                ReportNumber = $"CR-{(count + 1):D4}",
                CitizenUserId = req.CitizenUserId,
                IncidentType = req.IncidentType,
                Description = req.Description,
                Location = req.Location,
                IncidentDate = req.IncidentDate,
                Status = "Submitted"
            };

            _db.CitizenReports.Add(report);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = report.Id }, report);
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var r = await _db.CitizenReports.FindAsync(id);
            if (r == null) return NotFound();
            r.Status = status;
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}

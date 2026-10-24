using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PMS.Data;
using PMS.DTOs;
using PMS.Models;

namespace PMS.Controllers
{
    [ApiController]
    [Route("api/missing-persons")]
    public class MissingPersonsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public MissingPersonsController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? citizenUserId, [FromQuery] string? status)
        {
            var query = _db.MissingPersonReports.AsQueryable();
            if (citizenUserId.HasValue) query = query.Where(r => r.CitizenUserId == citizenUserId);
            if (!string.IsNullOrEmpty(status)) query = query.Where(r => r.Status == status);
            return Ok(await query.OrderByDescending(r => r.SubmittedAt).ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var r = await _db.MissingPersonReports.FindAsync(id);
            if (r == null) return NotFound();
            return Ok(r);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMissingPersonRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.MissingPersonName))
                return BadRequest(new { message = "Missing person name is required." });
            if (string.IsNullOrWhiteSpace(req.LastKnownLocation))
                return BadRequest(new { message = "Last known location is required." });

            var count = await _db.MissingPersonReports.CountAsync();
            var report = new MissingPersonReport
            {
                ReportNumber = $"MPR-{(count + 1):D4}",
                CitizenUserId = req.CitizenUserId,
                MissingPersonName = req.MissingPersonName,
                DateOfBirth = req.DateOfBirth,
                PhysicalDescription = req.PhysicalDescription,
                LastKnownLocation = req.LastKnownLocation,
                LastSeenDate = req.LastSeenDate,
                ContactInfo = req.ContactInfo,
                AdditionalNotes = req.AdditionalNotes,
                Status = "Active"
            };
            _db.MissingPersonReports.Add(report);
            _db.AuditLogs.Add(new AuditLog
            {
                ActionType = "CREATE", EntityType = "MissingPerson", EntityId = "pending",
                UserBadge = $"citizen_{req.CitizenUserId}", UserRole = "Citizen",
                Description = $"Missing person report filed for '{req.MissingPersonName}'."
            });
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = report.Id }, report);
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var r = await _db.MissingPersonReports.FindAsync(id);
            if (r == null) return NotFound();
            r.Status = status;
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}

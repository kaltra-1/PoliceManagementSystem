using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PMS.Data;
using PMS.DTOs;
using PMS.Models;

namespace PMS.Controllers
{
    [ApiController]
    [Route("api/patrols")]
    public class PatrolsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public PatrolsController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? officerBadge, [FromQuery] string? status)
        {
            var query = _db.Patrols.AsQueryable();
            if (!string.IsNullOrEmpty(officerBadge)) query = query.Where(p => p.OfficerBadge == officerBadge);
            if (!string.IsNullOrEmpty(status)) query = query.Where(p => p.Status == status);
            return Ok(await query.OrderByDescending(p => p.StartTime).ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var p = await _db.Patrols.FindAsync(id);
            if (p == null) return NotFound();
            return Ok(p);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePatrolRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Route))
                return BadRequest(new { message = "Route is required." });
            if (req.EndTime <= req.StartTime)
                return BadRequest(new { message = "End time must be after start time." });

            // Check for scheduling conflict
            var conflict = await _db.Patrols.AnyAsync(p =>
                p.OfficerBadge == req.OfficerBadge &&
                p.Status != "Cancelled" &&
                p.StartTime < req.EndTime &&
                p.EndTime > req.StartTime);
            if (conflict)
                return Conflict(new { message = "Scheduling conflict: this officer already has a patrol during that time." });

            var count = await _db.Patrols.CountAsync();
            var patrol = new Patrol
            {
                PatrolNumber = $"PAT-{(count + 1):D4}",
                OfficerBadge = req.OfficerBadge,
                OfficerName = req.OfficerName,
                Route = req.Route,
                Area = req.Area,
                StartTime = req.StartTime,
                EndTime = req.EndTime,
                Notes = req.Notes,
                Status = "Scheduled"
            };
            _db.Patrols.Add(patrol);
            _db.AuditLogs.Add(new AuditLog
            {
                ActionType = "CREATE", EntityType = "Patrol", EntityId = "pending",
                UserBadge = req.OfficerBadge, UserRole = "Officer",
                Description = $"Patrol {patrol.PatrolNumber} scheduled for {req.Area}."
            });
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = patrol.Id }, patrol);
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var p = await _db.Patrols.FindAsync(id);
            if (p == null) return NotFound();
            p.Status = status;
            _db.AuditLogs.Add(new AuditLog
            {
                ActionType = "STATUS_CHANGE", EntityType = "Patrol", EntityId = id.ToString(),
                UserBadge = p.OfficerBadge, UserRole = "Officer",
                Description = $"Patrol {p.PatrolNumber} status changed to {status}."
            });
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var p = await _db.Patrols.FindAsync(id);
            if (p == null) return NotFound();
            _db.Patrols.Remove(p);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}

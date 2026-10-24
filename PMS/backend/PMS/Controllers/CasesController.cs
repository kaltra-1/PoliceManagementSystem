using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using PMS.Data;
using PMS.DTOs;
using PMS.Models;

namespace PMS.Controllers
{
    [ApiController]
    [Route("api/cases")]
    public class CasesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public CasesController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? status,
            [FromQuery] string? priority,
            [FromQuery] int? officerId,
            [FromQuery] int? detectiveId)
        {
            var query = _db.Cases.AsQueryable();
            if (!string.IsNullOrEmpty(status)) query = query.Where(c => c.Status == status);
            if (!string.IsNullOrEmpty(priority)) query = query.Where(c => c.Priority == priority);
            if (officerId.HasValue) query = query.Where(c => c.AssignedOfficerId == officerId);
            if (detectiveId.HasValue) query = query.Where(c => c.AssignedDetectiveId == detectiveId);

            return Ok(await query.OrderByDescending(c => c.FiledDate).ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var c = await _db.Cases.FindAsync(id);
            if (c == null) return NotFound();
            return Ok(c);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCaseRequest req)
        {
            // Generate case number
            var count = await _db.Cases.CountAsync();
            var caseNum = $"CAS-{DateTime.UtcNow.Year}-{(count + 1):D4}";

            var c = new Case
            {
                CaseNumber = caseNum,
                CrimeType = req.CrimeType,
                Description = req.Description,
                Location = req.Location,
                District = req.District,
                IncidentDate = req.IncidentDate,
                Priority = req.Priority,
                ReportingOfficerBadge = req.ReportingOfficerBadge,
                WitnessCount = req.WitnessCount,
                SuspectsIdentified = req.SuspectsIdentified,
                Status = "Open"
            };

            _db.Cases.Add(c);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = c.Id }, c);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCaseRequest req)
        {
            var c = await _db.Cases.FindAsync(id);
            if (c == null) return NotFound();

            // UC14 — Detectives cannot close a case directly; they must request closure
            var role = User.FindFirstValue(ClaimTypes.Role);
            if (!string.IsNullOrEmpty(req.Status)
                && req.Status == "Closed"
                && role == "Detective")
            {
                return BadRequest(new
                {
                    message = "Detectives cannot close cases directly. Submit a closure request to the administrator via POST /api/cases/{id}/request-close."
                });
            }

            if (!string.IsNullOrEmpty(req.Status)) c.Status = req.Status;
            if (!string.IsNullOrEmpty(req.Priority)) c.Priority = req.Priority;
            if (!string.IsNullOrEmpty(req.Description)) c.Description = req.Description;
            if (req.AssignedOfficerId.HasValue) c.AssignedOfficerId = req.AssignedOfficerId;
            if (req.AssignedDetectiveId.HasValue) c.AssignedDetectiveId = req.AssignedDetectiveId;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // FR_10 — Dedicated assign endpoint (Admin)
        [HttpPatch("{id}/assign")]
        public async Task<IActionResult> Assign(int id, [FromBody] AssignOfficerRequest req)
        {
            var c = await _db.Cases.FindAsync(id);
            if (c == null) return NotFound();

            if (req.OfficerId.HasValue)
            {
                var officer = await _db.Users.FindAsync(req.OfficerId.Value);
                if (officer == null) return BadRequest(new { message = "Officer not found." });
                if (officer.Status != "Active") return BadRequest(new { message = "Cannot assign inactive officer." });
                c.AssignedOfficerId = req.OfficerId;

                // Notify the officer
                _db.Notifications.Add(new Notification
                {
                    RecipientBadge = officer.BadgeNumber,
                    Title = "New case assigned",
                    Message = $"You have been assigned case {c.CaseNumber} ({c.CrimeType}).",
                    Type = "Info",
                    Link = c.CaseNumber
                });
            }
            if (req.DetectiveId.HasValue)
            {
                var det = await _db.Users.FindAsync(req.DetectiveId.Value);
                if (det == null) return BadRequest(new { message = "Detective not found." });
                if (det.Status != "Active") return BadRequest(new { message = "Cannot assign inactive detective." });
                c.AssignedDetectiveId = req.DetectiveId;

                _db.Notifications.Add(new Notification
                {
                    RecipientBadge = det.BadgeNumber,
                    Title = "New case assigned",
                    Message = $"You have been assigned case {c.CaseNumber} ({c.CrimeType}).",
                    Type = "Info",
                    Link = c.CaseNumber
                });
            }

            await _db.SaveChangesAsync();
            await LogAudit("ASSIGN", "Case", c.Id.ToString(),
                $"Case {c.CaseNumber} assigned (officerId={req.OfficerId}, detectiveId={req.DetectiveId}).");

            return Ok(new { message = "Assigned successfully." });
        }

        // UC14 — Detective requests closure (admin must approve)
        [HttpPost("{id}/request-close")]
        public async Task<IActionResult> RequestClose(int id, [FromBody] CaseCloseRequest req)
        {
            var c = await _db.Cases.FindAsync(id);
            if (c == null) return NotFound();

            var badge = User.FindFirstValue(ClaimTypes.Name) ?? "unknown";

            // Mark as "Pending" (pending admin approval)
            c.Status = "Pending";
            await _db.SaveChangesAsync();

            await LogAudit("CLOSE_REQUEST", "Case", c.Id.ToString(),
                $"Detective {badge} requested closure of {c.CaseNumber}. Reason: {req.Reason}");

            // Notify all Admins
            _db.Notifications.Add(new Notification
            {
                RecipientRole = "Admin",
                Title = "Case closure request",
                Message = $"Detective {badge} requests closure of {c.CaseNumber}. Reason: {req.Reason}",
                Type = "Warning",
                Link = c.CaseNumber
            });
            await _db.SaveChangesAsync();

            return Ok(new { message = "Closure request submitted. Awaiting admin approval." });
        }

        // UC14 — Admin approves/rejects closure
        [HttpPost("{id}/approve-close")]
        public async Task<IActionResult> ApproveClose(int id)
        {
            var c = await _db.Cases.FindAsync(id);
            if (c == null) return NotFound();

            c.Status = "Closed";
            await _db.SaveChangesAsync();

            await LogAudit("CLOSE_APPROVED", "Case", c.Id.ToString(),
                $"Case {c.CaseNumber} closure approved by admin.");

            // Notify the detective who requested
            if (c.AssignedDetectiveId.HasValue)
            {
                var det = await _db.Users.FindAsync(c.AssignedDetectiveId.Value);
                if (det != null)
                {
                    _db.Notifications.Add(new Notification
                    {
                        RecipientBadge = det.BadgeNumber,
                        Title = "Case closure approved",
                        Message = $"Your closure request for {c.CaseNumber} has been approved.",
                        Type = "Success",
                        Link = c.CaseNumber
                    });
                    await _db.SaveChangesAsync();
                }
            }

            return Ok(new { message = "Case closed." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var c = await _db.Cases.FindAsync(id);
            if (c == null) return NotFound();
            _db.Cases.Remove(c);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        private async Task LogAudit(string action, string entity, string entityId, string desc)
        {
            var badge = User.FindFirstValue(ClaimTypes.Name) ?? "system";
            var role  = User.FindFirstValue(ClaimTypes.Role) ?? "system";
            _db.AuditLogs.Add(new AuditLog
            {
                ActionType = action, EntityType = entity, EntityId = entityId,
                UserBadge = badge, UserRole = role, Description = desc
            });
            await _db.SaveChangesAsync();
        }
    }
}

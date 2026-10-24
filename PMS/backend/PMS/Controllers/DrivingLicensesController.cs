using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PMS.Data;
using PMS.DTOs;
using PMS.Models;

namespace PMS.Controllers
{
    [ApiController]
    [Route("api/driving-licenses")]
    public class DrivingLicensesController : ControllerBase
    {
        private readonly AppDbContext _db;
        public DrivingLicensesController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? licenseNumber,
            [FromQuery] string? holderName,
            [FromQuery] string? nationalId,
            [FromQuery] string? status,
            [FromQuery] int? citizenUserId)
        {
            var query = _db.DrivingLicenses.AsQueryable();
            if (!string.IsNullOrEmpty(licenseNumber))
                query = query.Where(l => l.LicenseNumber.Contains(licenseNumber));
            if (!string.IsNullOrEmpty(holderName))
                query = query.Where(l => l.HolderName.Contains(holderName));
            if (!string.IsNullOrEmpty(nationalId))
                query = query.Where(l => l.HolderNationalId == nationalId);
            if (!string.IsNullOrEmpty(status))
                query = query.Where(l => l.Status == status);
            if (citizenUserId.HasValue)
                query = query.Where(l => l.CitizenUserId == citizenUserId);
            return Ok(await query.OrderByDescending(l => l.CreatedAt).ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var l = await _db.DrivingLicenses.FindAsync(id);
            if (l == null) return NotFound(new { message = "No license record found." });
            return Ok(l);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDrivingLicenseRequest req)
        {
            if (await _db.DrivingLicenses.AnyAsync(l => l.LicenseNumber == req.LicenseNumber))
                return BadRequest(new { message = "A license with this number already exists." });

            var license = new DrivingLicense
            {
                LicenseNumber = req.LicenseNumber,
                CitizenUserId = req.CitizenUserId,
                HolderName = req.HolderName,
                HolderNationalId = req.HolderNationalId,
                Category = req.Category,
                IssueDate = req.IssueDate,
                ExpiryDate = req.ExpiryDate,
                Status = req.ExpiryDate < DateTime.UtcNow ? "Expired" : "Valid"
            };
            _db.DrivingLicenses.Add(license);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = license.Id }, license);
        }

        [HttpPatch("{id}/flag")]
        public async Task<IActionResult> FlagLicense(int id, [FromBody] FlagLicenseRequest req)
        {
            var l = await _db.DrivingLicenses.FindAsync(id);
            if (l == null) return NotFound(new { message = "No license record found." });

            if (l.Status == "Suspended" && req.Status == "Suspended")
                return BadRequest(new { message = "License is already suspended." });

            var old = l.Status;
            l.Status = req.Status;
            l.SuspensionReason = req.Reason;
            l.FlaggedByBadge = req.OfficerBadge;
            l.FlaggedAt = DateTime.UtcNow;

            _db.AuditLogs.Add(new AuditLog
            {
                ActionType = "STATUS_CHANGE", EntityType = "DrivingLicense", EntityId = id.ToString(),
                UserBadge = req.OfficerBadge, UserRole = "Traffic",
                Description = $"License {l.LicenseNumber} status changed from {old} to {req.Status}. Reason: {req.Reason}",
                OldValue = old, NewValue = req.Status
            });
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}

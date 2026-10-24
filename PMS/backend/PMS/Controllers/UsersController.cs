using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using PMS.Data;
using PMS.DTOs;
using PMS.Models;

namespace PMS.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _db;

        public UsersController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? role, [FromQuery] string? status)
        {
            var query = _db.Users.AsQueryable();
            if (!string.IsNullOrEmpty(role)) query = query.Where(u => u.Role == role);
            if (!string.IsNullOrEmpty(status)) query = query.Where(u => u.Status == status);

            var users = await query.Select(u => new
            {
                u.Id, u.BadgeNumber, u.FullName, u.Email, u.Role,
                u.Department, u.Status, u.CreatedAt, u.LastLogin
            }).ToListAsync();

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();
            // Don't expose password hash
            return Ok(new { user.Id, user.BadgeNumber, user.FullName, user.Email, user.Role, user.Department, user.Status, user.CreatedAt, user.LastLogin });
        }

        // UC42 — Check if user has active cases before deactivation
        [HttpGet("{id}/active-cases")]
        public async Task<IActionResult> GetActiveCases(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            var activeCases = await _db.Cases
                .Where(c => (c.AssignedOfficerId == id || c.AssignedDetectiveId == id) && c.Status != "Closed")
                .Select(c => new { c.Id, c.CaseNumber, c.CrimeType, c.Status, c.Priority })
                .ToListAsync();

            return Ok(new { count = activeCases.Count, cases = activeCases });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest req)
        {
            if (await _db.Users.AnyAsync(u => u.BadgeNumber == req.BadgeNumber))
                return BadRequest(new { message = "Badge number already exists." });

            // Only allow the valid roles
            var validRoles = new[] { "Admin", "Officer", "Detective", "Forensic", "Traffic", "Citizen" };
            if (!validRoles.Contains(req.Role))
                return BadRequest(new { message = "Invalid role." });

            var user = new User
            {
                BadgeNumber = req.BadgeNumber,
                FullName = req.FullName,
                Email = req.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
                Role = req.Role,
                Department = req.Department,
                Status = "Active"
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            await LogAudit("CREATE", "User", user.Id.ToString(), $"User {user.BadgeNumber} ({user.Role}) created.");

            return CreatedAtAction(nameof(GetById), new { id = user.Id },
                new { user.Id, user.BadgeNumber, user.FullName, user.Role, user.Department, user.Status });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequest req)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.FullName = req.FullName;
            user.Email = req.Email;
            user.Role = req.Role;
            user.Department = req.Department;
            user.Status = req.Status;

            await _db.SaveChangesAsync();
            await LogAudit("UPDATE", "User", user.Id.ToString(), $"User {user.BadgeNumber} updated.");
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();
            var badge = user.BadgeNumber;
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            await LogAudit("DELETE", "User", id.ToString(), $"User {badge} deleted.");
            return NoContent();
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            var oldStatus = user.Status;
            user.Status = status;
            await _db.SaveChangesAsync();
            await LogAudit("STATUS_CHANGE", "User", user.Id.ToString(),
                $"User {user.BadgeNumber} status changed: {oldStatus} → {status}.");
            return NoContent();
        }

        // UC39 — Admin resets a user's password
        [HttpPost("{id}/reset-password")]
        public async Task<IActionResult> ResetPassword(int id, [FromBody] AdminResetPasswordRequest req)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            if (string.IsNullOrEmpty(req.NewPassword) || req.NewPassword.Length < 8)
                return BadRequest(new { message = "New password must be at least 8 characters." });

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.NewPassword);
            // Reset lockout if any
            user.FailedLoginAttempts = 0;
            user.LockedUntil = null;
            await _db.SaveChangesAsync();

            await LogAudit("PASSWORD_RESET", "User", user.Id.ToString(),
                $"Admin reset password for user {user.BadgeNumber}.");

            // Notify the user via in-app notification
            _db.Notifications.Add(new Notification
            {
                RecipientBadge = user.BadgeNumber,
                Title = "Password Reset",
                Message = "Your password was reset by an administrator. Please log in with the new password and change it.",
                Type = "Warning"
            });
            await _db.SaveChangesAsync();

            return Ok(new { message = "Password reset successfully." });
        }

        private async Task LogAudit(string action, string entity, string entityId, string desc)
        {
            var badge = User.FindFirstValue(ClaimTypes.Name) ?? "system";
            var role  = User.FindFirstValue(ClaimTypes.Role) ?? "Admin";
            _db.AuditLogs.Add(new AuditLog
            {
                ActionType = action, EntityType = entity, EntityId = entityId,
                UserBadge = badge, UserRole = role, Description = desc
            });
            await _db.SaveChangesAsync();
        }
    }
}

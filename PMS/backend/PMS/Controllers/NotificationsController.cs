using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using PMS.Data;
using PMS.DTOs;
using PMS.Models;

namespace PMS.Controllers
{
    // FR_42 / FR_44 — In-app notifications
    [ApiController]
    [Route("api/notifications")]
    public class NotificationsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public NotificationsController(AppDbContext db) => _db = db;

        // Get notifications for the current user (by badge or role broadcast)
        [HttpGet]
        public async Task<IActionResult> GetMine([FromQuery] string? badge, [FromQuery] string? role, [FromQuery] bool unreadOnly = false)
        {
            // Prefer JWT identity; fall back to query (for non-authenticated demo)
            badge ??= User.FindFirstValue(ClaimTypes.Name);
            role  ??= User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrEmpty(badge) && string.IsNullOrEmpty(role))
                return Ok(new { unreadCount = 0, notifications = new List<object>() });

            var query = _db.Notifications.AsQueryable();
            query = query.Where(n =>
                (!string.IsNullOrEmpty(badge) && n.RecipientBadge == badge) ||
                (!string.IsNullOrEmpty(role)  && n.RecipientRole  == role  && string.IsNullOrEmpty(n.RecipientBadge))
            );

            if (unreadOnly) query = query.Where(n => !n.IsRead);

            var notifications = await query
                .OrderByDescending(n => n.CreatedAt)
                .Take(50)
                .ToListAsync();

            var unreadCount = notifications.Count(n => !n.IsRead);
            return Ok(new { unreadCount, notifications });
        }

        // Mark a single notification as read
        [HttpPatch("{id}/read")]
        public async Task<IActionResult> MarkRead(int id)
        {
            var n = await _db.Notifications.FindAsync(id);
            if (n == null) return NotFound();
            n.IsRead = true;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // Mark all as read for current user
        [HttpPost("mark-all-read")]
        public async Task<IActionResult> MarkAllRead([FromQuery] string? badge, [FromQuery] string? role)
        {
            badge ??= User.FindFirstValue(ClaimTypes.Name);
            role  ??= User.FindFirstValue(ClaimTypes.Role);

            var query = _db.Notifications.Where(n => !n.IsRead);
            query = query.Where(n =>
                (!string.IsNullOrEmpty(badge) && n.RecipientBadge == badge) ||
                (!string.IsNullOrEmpty(role)  && n.RecipientRole  == role  && string.IsNullOrEmpty(n.RecipientBadge))
            );

            var items = await query.ToListAsync();
            items.ForEach(n => n.IsRead = true);
            await _db.SaveChangesAsync();
            return Ok(new { updated = items.Count });
        }

        // Create a notification (used internally and by admin for broadcasts)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateNotificationRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Title) || string.IsNullOrWhiteSpace(req.Message))
                return BadRequest(new { message = "Title and Message are required." });

            if (string.IsNullOrEmpty(req.RecipientBadge) && string.IsNullOrEmpty(req.RecipientRole))
                return BadRequest(new { message = "Either RecipientBadge or RecipientRole is required." });

            var n = new Notification
            {
                RecipientBadge = req.RecipientBadge,
                RecipientRole = req.RecipientRole,
                Title = req.Title,
                Message = req.Message,
                Type = req.Type,
                Link = req.Link
            };
            _db.Notifications.Add(n);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetMine), null, n);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var n = await _db.Notifications.FindAsync(id);
            if (n == null) return NotFound();
            _db.Notifications.Remove(n);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}

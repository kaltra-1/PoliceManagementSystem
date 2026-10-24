using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PMS.Data;
using PMS.DTOs;
using PMS.Models;

namespace PMS.Controllers
{
    // System-wide settings (Admin → Settings page)
    [ApiController]
    [Route("api/settings")]
    public class SettingsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public SettingsController(AppDbContext db) => _db = db;

        // Default settings — used as fallback if DB is empty
        private static readonly Dictionary<string, (string Value, string Category)> Defaults = new()
        {
            { "system_name",          ("Police Management System", "General") },
            { "department_name",      ("Metro Police Department",  "General") },
            { "timezone",             ("UTC+1 (CET)",              "General") },
            { "session_timeout_min",  ("30",                       "Security") },
            { "max_login_attempts",   ("5",                        "Security") },
            { "password_min_length",  ("8",                        "Security") },
        };

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var existing = await _db.Settings.ToDictionaryAsync(s => s.Key, s => s);
            var result = new Dictionary<string, object>();
            foreach (var (key, def) in Defaults)
            {
                if (existing.TryGetValue(key, out var s))
                    result[key] = new { value = s.Value, category = s.Category, updatedAt = s.UpdatedAt };
                else
                    result[key] = new { value = def.Value, category = def.Category, updatedAt = (DateTime?)null };
            }
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAll([FromBody] UpdateSettingsRequest req)
        {
            if (req.Settings == null || req.Settings.Count == 0)
                return BadRequest(new { message = "No settings provided." });

            foreach (var (key, value) in req.Settings)
            {
                if (!Defaults.ContainsKey(key)) continue; // ignore unknown keys
                var existing = await _db.Settings.FirstOrDefaultAsync(s => s.Key == key);
                if (existing == null)
                {
                    _db.Settings.Add(new Setting
                    {
                        Key = key,
                        Value = value,
                        Category = Defaults[key].Category,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
                else
                {
                    existing.Value = value;
                    existing.UpdatedAt = DateTime.UtcNow;
                }
            }
            await _db.SaveChangesAsync();

            // Log to audit
            _db.AuditLogs.Add(new AuditLog
            {
                ActionType = "UPDATE",
                EntityType = "Settings",
                UserBadge = User.Identity?.Name ?? "system",
                UserRole = "Admin",
                Description = $"System settings updated ({req.Settings.Count} keys)."
            });
            await _db.SaveChangesAsync();

            return Ok(new { message = "Settings saved successfully." });
        }
    }
}

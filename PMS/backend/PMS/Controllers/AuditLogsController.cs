using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PMS.Data;

namespace PMS.Controllers
{
    [ApiController]
    [Route("api/audit-logs")]
    public class AuditLogsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public AuditLogsController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? userBadge,
            [FromQuery] string? actionType,
            [FromQuery] string? entityType,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            var query = _db.AuditLogs.AsQueryable();
            if (!string.IsNullOrEmpty(userBadge)) query = query.Where(l => l.UserBadge.Contains(userBadge));
            if (!string.IsNullOrEmpty(actionType)) query = query.Where(l => l.ActionType == actionType);
            if (!string.IsNullOrEmpty(entityType)) query = query.Where(l => l.EntityType == entityType);
            if (from.HasValue) query = query.Where(l => l.Timestamp >= from.Value);
            if (to.HasValue) query = query.Where(l => l.Timestamp <= to.Value);

            var total = await query.CountAsync();
            var logs = await query
                .OrderByDescending(l => l.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new { total, page, pageSize, logs });
        }
    }
}

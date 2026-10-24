using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PMS.Data;
using PMS.DTOs;
using PMS.Models;

namespace PMS.Controllers
{
    [ApiController]
    [Route("api/witnesses")]
    public class WitnessesController : ControllerBase
    {
        private readonly AppDbContext _db;
        public WitnessesController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? caseId, [FromQuery] string? caseNumber)
        {
            var query = _db.Witnesses.AsQueryable();
            if (caseId.HasValue) query = query.Where(w => w.CaseId == caseId);
            if (!string.IsNullOrEmpty(caseNumber)) query = query.Where(w => w.CaseNumber == caseNumber);
            return Ok(await query.OrderByDescending(w => w.AddedAt).ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var w = await _db.Witnesses.FindAsync(id);
            if (w == null) return NotFound();
            return Ok(w);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateWitnessRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.FullName))
                return BadRequest(new { message = "Witness name is required." });

            var caseExists = await _db.Cases.AnyAsync(c => c.Id == req.CaseId);
            if (!caseExists)
                return BadRequest(new { message = "Case not found. Please verify the Case ID." });

            var count = await _db.Witnesses.CountAsync();
            var witness = new Witness
            {
                WitnessNumber = $"WIT-{(count + 1):D4}",
                CaseId = req.CaseId,
                CaseNumber = req.CaseNumber,
                FullName = req.FullName,
                ContactInfo = req.ContactInfo,
                Testimony = req.Testimony,
                IsConfidential = req.IsConfidential,
                AddedByBadge = req.AddedByBadge
            };
            _db.Witnesses.Add(witness);
            _db.AuditLogs.Add(new AuditLog
            {
                ActionType = "CREATE", EntityType = "Witness", EntityId = "pending",
                UserBadge = req.AddedByBadge, UserRole = "Detective",
                Description = $"Witness '{req.FullName}' added to case {req.CaseNumber}."
            });
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = witness.Id }, witness);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateWitnessRequest req)
        {
            var w = await _db.Witnesses.FindAsync(id);
            if (w == null) return NotFound();
            w.FullName = req.FullName;
            w.ContactInfo = req.ContactInfo;
            w.Testimony = req.Testimony;
            w.IsConfidential = req.IsConfidential;
            _db.AuditLogs.Add(new AuditLog
            {
                ActionType = "UPDATE", EntityType = "Witness", EntityId = id.ToString(),
                UserBadge = req.AddedByBadge, UserRole = "Detective",
                Description = $"Witness {w.WitnessNumber} updated."
            });
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var w = await _db.Witnesses.FindAsync(id);
            if (w == null) return NotFound();
            _db.Witnesses.Remove(w);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}

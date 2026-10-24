using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PMS.Data;
using PMS.DTOs;
using PMS.Models;

namespace PMS.Controllers
{
    [ApiController]
    [Route("api/evidence")]
    public class EvidenceController : ControllerBase
    {
        private readonly AppDbContext _db;

        public EvidenceController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? caseId, [FromQuery] string? status)
        {
            var query = _db.Evidences.AsQueryable();
            if (caseId.HasValue) query = query.Where(e => e.CaseId == caseId);
            if (!string.IsNullOrEmpty(status)) query = query.Where(e => e.Status == status);
            return Ok(await query.OrderByDescending(e => e.CollectedAt).ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var e = await _db.Evidences.FindAsync(id);
            if (e == null) return NotFound();
            return Ok(e);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEvidenceRequest req)
        {
            var count = await _db.Evidences.CountAsync();
            var evd = new Evidence
            {
                EvidenceNumber = $"EVD-{(count + 1):D4}",
                CaseId = req.CaseId,
                CaseNumber = req.CaseNumber,
                Type = req.Type,
                Description = req.Description,
                CollectedBy = req.CollectedBy,
                StorageLocation = req.StorageLocation,
                ChainOfCustody = $"Logged by {req.CollectedBy}",
                Status = "Logged"
            };

            _db.Evidences.Add(evd);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = evd.Id }, evd);
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var e = await _db.Evidences.FindAsync(id);
            if (e == null) return NotFound();
            e.Status = status;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var e = await _db.Evidences.FindAsync(id);
            if (e == null) return NotFound();
            _db.Evidences.Remove(e);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}

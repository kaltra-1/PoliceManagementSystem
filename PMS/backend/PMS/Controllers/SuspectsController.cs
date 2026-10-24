using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PMS.Data;
using PMS.DTOs;
using PMS.Models;

namespace PMS.Controllers
{
    [ApiController]
    [Route("api/suspects")]
    public class SuspectsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public SuspectsController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? caseId, [FromQuery] string? status)
        {
            var query = _db.Suspects.AsQueryable();
            if (caseId.HasValue) query = query.Where(s => s.CaseId == caseId);
            if (!string.IsNullOrEmpty(status)) query = query.Where(s => s.Status == status);
            return Ok(await query.OrderByDescending(s => s.AddedAt).ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var s = await _db.Suspects.FindAsync(id);
            if (s == null) return NotFound();
            return Ok(s);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSuspectRequest req)
        {
            var suspect = new Suspect
            {
                FullName = req.FullName,
                Age = req.Age,
                Gender = req.Gender,
                PhysicalDescription = req.PhysicalDescription,
                CaseId = req.CaseId,
                CaseNumber = req.CaseNumber,
                Charge = req.Charge,
                AddedByBadge = req.AddedByBadge,
                Status = "Identified"
            };

            _db.Suspects.Add(suspect);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = suspect.Id }, suspect);
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var s = await _db.Suspects.FindAsync(id);
            if (s == null) return NotFound();
            s.Status = status;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var s = await _db.Suspects.FindAsync(id);
            if (s == null) return NotFound();
            _db.Suspects.Remove(s);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}

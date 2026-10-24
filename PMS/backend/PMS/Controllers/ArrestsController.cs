using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PMS.Data;
using PMS.DTOs;
using PMS.Models;

namespace PMS.Controllers
{
    [ApiController]
    [Route("api/arrests")]
    public class ArrestsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ArrestsController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? caseId, [FromQuery] string? officerBadge)
        {
            var query = _db.Arrests.AsQueryable();
            if (caseId.HasValue) query = query.Where(a => a.CaseId == caseId);
            if (!string.IsNullOrEmpty(officerBadge)) query = query.Where(a => a.ArrestingOfficerBadge == officerBadge);
            return Ok(await query.OrderByDescending(a => a.ArrestedAt).ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var a = await _db.Arrests.FindAsync(id);
            if (a == null) return NotFound();
            return Ok(a);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateArrestRequest req)
        {
            var count = await _db.Arrests.CountAsync();
            var arrest = new Arrest
            {
                ArrestNumber = $"ARR-{(count + 1):D4}",
                SuspectName = req.SuspectName,
                CaseId = req.CaseId,
                CaseNumber = req.CaseNumber,
                Charge = req.Charge,
                ArrestLocation = req.ArrestLocation,
                ArrestingOfficerBadge = req.ArrestingOfficerBadge,
                Status = "Processing"
            };

            _db.Arrests.Add(arrest);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = arrest.Id }, arrest);
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var a = await _db.Arrests.FindAsync(id);
            if (a == null) return NotFound();
            a.Status = status;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var a = await _db.Arrests.FindAsync(id);
            if (a == null) return NotFound();
            _db.Arrests.Remove(a);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}

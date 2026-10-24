using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PMS.Data;
using PMS.DTOs;
using PMS.Models;

namespace PMS.Controllers
{
    [ApiController]
    [Route("api/fines")]
    public class FinesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public FinesController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? officerBadge,
            [FromQuery] string? licensePlate,
            [FromQuery] int? citizenUserId,
            [FromQuery] string? status)
        {
            var query = _db.Fines.AsQueryable();
            if (!string.IsNullOrEmpty(officerBadge)) query = query.Where(f => f.IssuingOfficerBadge == officerBadge);
            if (!string.IsNullOrEmpty(licensePlate)) query = query.Where(f => f.LicensePlate == licensePlate);
            if (citizenUserId.HasValue) query = query.Where(f => f.CitizenUserId == citizenUserId);
            if (!string.IsNullOrEmpty(status)) query = query.Where(f => f.Status == status);
            return Ok(await query.OrderByDescending(f => f.IssuedAt).ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var f = await _db.Fines.FindAsync(id);
            if (f == null) return NotFound();
            return Ok(f);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFineRequest req)
        {
            var count = await _db.Fines.CountAsync();
            var fine = new Fine
            {
                FineNumber = $"FIN-{(count + 1):D4}",
                LicensePlate = req.LicensePlate.ToUpper(),
                VehicleType = req.VehicleType,
                ViolationType = req.ViolationType,
                Amount = req.Amount,
                IssuingOfficerBadge = req.IssuingOfficerBadge,
                Location = req.Location,
                Notes = req.Notes,
                CitizenUserId = req.CitizenUserId,
                Status = "Unpaid"
            };

            _db.Fines.Add(fine);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = fine.Id }, fine);
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var f = await _db.Fines.FindAsync(id);
            if (f == null) return NotFound();
            f.Status = status;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("stats/today")]
        public async Task<IActionResult> TodayStats()
        {
            var today = DateTime.UtcNow.Date;
            var count = await _db.Fines.CountAsync(f => f.IssuedAt >= today);
            var total = await _db.Fines.Where(f => f.IssuedAt >= today).SumAsync(f => f.Amount);
            return Ok(new { count, total });
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PMS.Data;
using PMS.DTOs;
using PMS.Models;

namespace PMS.Controllers
{
    // FR_09 — Department management (Admin only)
    [ApiController]
    [Route("api/departments")]
    public class DepartmentsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public DepartmentsController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? status)
        {
            var query = _db.Departments.AsQueryable();
            if (!string.IsNullOrEmpty(status)) query = query.Where(d => d.Status == status);
            var depts = await query.OrderBy(d => d.Name).ToListAsync();

            // Include active staff count per department (matched by name)
            var users = await _db.Users.Where(u => u.Status == "Active").ToListAsync();
            var result = depts.Select(d => new
            {
                d.Id, d.Name, d.Code, d.Description, d.HeadBadge, d.Status, d.CreatedAt,
                StaffCount = users.Count(u => u.Department == d.Name)
            });
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var d = await _db.Departments.FindAsync(id);
            if (d == null) return NotFound();
            return Ok(d);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDepartmentRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Name) || string.IsNullOrWhiteSpace(req.Code))
                return BadRequest(new { message = "Name and Code are required." });

            if (await _db.Departments.AnyAsync(d => d.Code == req.Code))
                return BadRequest(new { message = "Department code already exists." });

            var d = new Department
            {
                Name = req.Name,
                Code = req.Code.ToUpper(),
                Description = req.Description,
                HeadBadge = req.HeadBadge,
                Status = "Active"
            };
            _db.Departments.Add(d);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = d.Id }, d);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDepartmentRequest req)
        {
            var d = await _db.Departments.FindAsync(id);
            if (d == null) return NotFound();

            d.Name = req.Name;
            d.Code = req.Code.ToUpper();
            d.Description = req.Description;
            d.HeadBadge = req.HeadBadge;
            d.Status = req.Status;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var d = await _db.Departments.FindAsync(id);
            if (d == null) return NotFound();

            // Check if any active users belong to this department
            var inUse = await _db.Users.AnyAsync(u => u.Department == d.Name && u.Status == "Active");
            if (inUse)
                return BadRequest(new { message = "Cannot delete: department has active staff. Reassign them first." });

            _db.Departments.Remove(d);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using PMS.Data;
using PMS.DTOs;
using PMS.Models;

namespace PMS.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;
        private const int MaxFailedAttempts = 5;
        private const int LockoutMinutes = 15;

        public AuthController(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.BadgeNumber) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest(new { message = "Badge number and password are required." });

            var user = await _db.Users.FirstOrDefaultAsync(u => u.BadgeNumber == req.BadgeNumber);

            if (user == null)
                return Unauthorized(new { message = "Invalid credentials." });

            // FR_04: Check lockout
            if (user.LockedUntil.HasValue && user.LockedUntil > DateTime.UtcNow)
            {
                var remaining = (int)(user.LockedUntil.Value - DateTime.UtcNow).TotalMinutes + 1;
                await LogAudit("LOCKOUT_ATTEMPT", "User", user.Id.ToString(), user.BadgeNumber, user.Role,
                    $"Login attempted on locked account. Locked for {remaining} more minute(s).");
                return Unauthorized(new { message = $"Account is locked. Please try again in {remaining} minute(s) or contact the Administrator." });
            }

            if (user.Status == "Inactive" || user.Status == "Locked")
                return Unauthorized(new { message = "Account is inactive. Please contact the Administrator." });

            if (!BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            {
                user.FailedLoginAttempts++;
                if (user.FailedLoginAttempts >= MaxFailedAttempts)
                {
                    user.LockedUntil = DateTime.UtcNow.AddMinutes(LockoutMinutes);
                    await LogAudit("ACCOUNT_LOCKED", "User", user.Id.ToString(), user.BadgeNumber, user.Role,
                        $"Account locked after {MaxFailedAttempts} failed login attempts.");
                    await _db.SaveChangesAsync();
                    return Unauthorized(new { message = $"Too many failed attempts. Account locked for {LockoutMinutes} minutes." });
                }
                await _db.SaveChangesAsync();
                int remaining = MaxFailedAttempts - user.FailedLoginAttempts;
                return Unauthorized(new { message = $"Invalid credentials. {remaining} attempt(s) remaining before lockout." });
            }

            // Optional role check
            if (!string.IsNullOrEmpty(req.Role) && user.Role != req.Role)
                return Unauthorized(new { message = "Role mismatch. Please select the correct role." });

            // Successful login — reset counters
            user.FailedLoginAttempts = 0;
            user.LockedUntil = null;
            user.LastLogin = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            await LogAudit("LOGIN", "User", user.Id.ToString(), user.BadgeNumber, user.Role, "User logged in successfully.");

            var token = GenerateToken(user);
            return Ok(new LoginResponse
            {
                Id = user.Id,
                BadgeNumber = user.BadgeNumber,
                FullName = user.FullName,
                Role = user.Role,
                Department = user.Department,
                Token = token
            });
        }

        // UC3: Change own password
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest req)
        {
            // Get badge from JWT
            var badge = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(badge))
                return Unauthorized(new { message = "Not authenticated." });

            var user = await _db.Users.FirstOrDefaultAsync(u => u.BadgeNumber == badge);
            if (user == null) return NotFound(new { message = "User not found." });

            if (!BCrypt.Net.BCrypt.Verify(req.CurrentPassword, user.PasswordHash))
                return BadRequest(new { message = "Current password is incorrect." });

            if (req.NewPassword.Length < 8)
                return BadRequest(new { message = "New password must be at least 8 characters." });

            if (BCrypt.Net.BCrypt.Verify(req.NewPassword, user.PasswordHash))
                return BadRequest(new { message = "New password must be different from the current password." });

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.NewPassword);
            await _db.SaveChangesAsync();

            await LogAudit("PASSWORD_CHANGE", "User", user.Id.ToString(), user.BadgeNumber, user.Role, "User changed their own password.");

            return Ok(new { message = "Password changed successfully." });
        }

        private string GenerateToken(User user)
        {
            var jwtKey = _config["Jwt:Key"] ?? "PMS_DefaultSecretKey_ChangeInProduction_2024!";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.BadgeNumber),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("fullName", user.FullName),
                new Claim("department", user.Department)
            };
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"] ?? "PMS",
                audience: _config["Jwt:Audience"] ?? "PMS",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task LogAudit(string action, string entity, string? entityId, string badge, string role, string desc)
        {
            _db.AuditLogs.Add(new AuditLog
            {
                ActionType = action, EntityType = entity, EntityId = entityId,
                UserBadge = badge, UserRole = role, Description = desc
            });
            await _db.SaveChangesAsync();
        }
    }
}

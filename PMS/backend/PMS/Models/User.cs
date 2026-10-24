namespace PMS.Models
{
    public class User
    {
        public int Id { get; set; }
        public string BadgeNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // Admin, Officer, Detective, Forensic, Traffic, Citizen
        public string Department { get; set; } = string.Empty;
        public string Status { get; set; } = "Active"; // Active, Inactive, Locked, Pending
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLogin { get; set; }
        // FR_04: Account lockout
        public int FailedLoginAttempts { get; set; } = 0;
        public DateTime? LockedUntil { get; set; }
    }
}

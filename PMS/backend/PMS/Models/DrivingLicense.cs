namespace PMS.Models
{
    public class DrivingLicense
    {
        public int Id { get; set; }
        public string LicenseNumber { get; set; } = string.Empty;
        public int? CitizenUserId { get; set; }
        public string HolderName { get; set; } = string.Empty;
        public string HolderNationalId { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // A, B, C, D, BE...
        public DateTime IssueDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Status { get; set; } = "Valid"; // Valid, Expired, Suspended, Revoked
        public string? SuspensionReason { get; set; }
        public string? FlaggedByBadge { get; set; }
        public DateTime? FlaggedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

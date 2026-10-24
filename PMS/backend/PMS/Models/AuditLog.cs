namespace PMS.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string ActionType { get; set; } = string.Empty; // LOGIN, LOGOUT, CREATE, UPDATE, DELETE, STATUS_CHANGE, PASSWORD_CHANGE, LOCKOUT
        public string EntityType { get; set; } = string.Empty; // User, Case, Evidence, Suspect, Arrest, Fine, ForensicReport, CitizenReport, Witness, Patrol, MissingPerson, License
        public string? EntityId { get; set; }
        public string UserBadge { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}

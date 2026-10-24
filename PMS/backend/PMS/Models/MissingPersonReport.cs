namespace PMS.Models
{
    public class MissingPersonReport
    {
        public int Id { get; set; }
        public string ReportNumber { get; set; } = string.Empty; // e.g. MPR-0001
        public int CitizenUserId { get; set; }
        public string MissingPersonName { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string PhysicalDescription { get; set; } = string.Empty;
        public string LastKnownLocation { get; set; } = string.Empty;
        public DateTime LastSeenDate { get; set; }
        public string ContactInfo { get; set; } = string.Empty;
        public string AdditionalNotes { get; set; } = string.Empty;
        public string Status { get; set; } = "Active"; // Active, Located, Closed
        public string? AssignedToBadge { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}

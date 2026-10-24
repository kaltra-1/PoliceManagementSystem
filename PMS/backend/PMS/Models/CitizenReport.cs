namespace PMS.Models
{
    public class CitizenReport
    {
        public int Id { get; set; }
        public string ReportNumber { get; set; } = string.Empty; // e.g. CR-0001
        public int CitizenUserId { get; set; }
        public string IncidentType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime IncidentDate { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Submitted"; // Submitted, Under Review, Assigned, Resolved, Closed
        public string? AssignedToBadge { get; set; }
        public string? OfficialNotes { get; set; }
    }
}

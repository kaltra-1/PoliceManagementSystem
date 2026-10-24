namespace PMS.Models
{
    public class Case
    {
        public int Id { get; set; }
        public string CaseNumber { get; set; } = string.Empty; // e.g. CAS-2024-0001
        public string CrimeType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public DateTime IncidentDate { get; set; }
        public DateTime FiledDate { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Open"; // Open, Active, Urgent, Pending, Closed
        public string Priority { get; set; } = "Low"; // Low, Medium, High
        public int? AssignedOfficerId { get; set; }
        public int? AssignedDetectiveId { get; set; }
        public string ReportingOfficerBadge { get; set; } = string.Empty;
        public int WitnessCount { get; set; } = 0;
        public bool SuspectsIdentified { get; set; } = false;
    }
}

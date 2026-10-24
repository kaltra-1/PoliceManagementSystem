namespace PMS.Models
{
    public class Patrol
    {
        public int Id { get; set; }
        public string PatrolNumber { get; set; } = string.Empty; // e.g. PAT-0001
        public string OfficerBadge { get; set; } = string.Empty;
        public string OfficerName { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; } = "Scheduled"; // Scheduled, Active, Completed, Cancelled
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

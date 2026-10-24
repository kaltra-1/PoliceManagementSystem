namespace PMS.Models
{
    public class ForensicReport
    {
        public int Id { get; set; }
        public string ReportNumber { get; set; } = string.Empty; // e.g. FR-0001
        public int CaseId { get; set; }
        public string CaseNumber { get; set; } = string.Empty;
        public string EvidenceNumber { get; set; } = string.Empty;
        public string AnalysisType { get; set; } = string.Empty; // DNA, Fingerprints, Ballistics, Chemical, Fiber, Digital
        public string Findings { get; set; } = string.Empty;
        public int ProgressPercent { get; set; } = 0;
        public string Status { get; set; } = "In Progress"; // In Progress, Confirming, Submitted, Archived
        public string AnalystBadge { get; set; } = string.Empty;
        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
        public DateTime? SubmittedAt { get; set; }
        public DateTime? DueDate { get; set; }
    }
}

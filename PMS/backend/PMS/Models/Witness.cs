namespace PMS.Models
{
    public class Witness
    {
        public int Id { get; set; }
        public string WitnessNumber { get; set; } = string.Empty; // e.g. WIT-0001
        public int CaseId { get; set; }
        public string CaseNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string ContactInfo { get; set; } = string.Empty;
        public string Testimony { get; set; } = string.Empty;
        public bool IsConfidential { get; set; } = false;
        public string AddedByBadge { get; set; } = string.Empty;
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}

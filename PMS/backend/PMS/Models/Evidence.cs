namespace PMS.Models
{
    public class Evidence
    {
        public int Id { get; set; }
        public string EvidenceNumber { get; set; } = string.Empty; // e.g. EVD-0001
        public int CaseId { get; set; }
        public string CaseNumber { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // Physical, Digital, Documentary, Biological
        public string Description { get; set; } = string.Empty;
        public string ChainOfCustody { get; set; } = string.Empty;
        public string Status { get; set; } = "Logged"; // Logged, Processing, Lab Pending, In Storage, Released
        public string CollectedBy { get; set; } = string.Empty;
        public DateTime CollectedAt { get; set; } = DateTime.UtcNow;
        public string StorageLocation { get; set; } = string.Empty;
    }
}

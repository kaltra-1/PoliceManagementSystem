namespace PMS.Models
{
    public class Arrest
    {
        public int Id { get; set; }
        public string ArrestNumber { get; set; } = string.Empty; // e.g. ARR-0001
        public string SuspectName { get; set; } = string.Empty;
        public int CaseId { get; set; }
        public string CaseNumber { get; set; } = string.Empty;
        public string Charge { get; set; } = string.Empty;
        public DateTime ArrestedAt { get; set; } = DateTime.UtcNow;
        public string ArrestLocation { get; set; } = string.Empty;
        public string ArrestingOfficerBadge { get; set; } = string.Empty;
        public string Status { get; set; } = "Processing"; // Processing, Detained, Released, Released (Bail), Charged
    }
}

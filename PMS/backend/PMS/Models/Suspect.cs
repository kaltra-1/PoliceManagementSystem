namespace PMS.Models
{
    public class Suspect
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string PhysicalDescription { get; set; } = string.Empty;
        public int CaseId { get; set; }
        public string CaseNumber { get; set; } = string.Empty;
        public string Charge { get; set; } = string.Empty;
        public string Status { get; set; } = "Identified"; // Identified, Detained, Arrested, Released, At Large
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
        public string AddedByBadge { get; set; } = string.Empty;
    }
}

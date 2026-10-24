namespace PMS.Models
{
    // FR_09 — Department management (admin-only CRUD)
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty; // e.g. CID, PAT, TRF, FOR, ADM
        public string Description { get; set; } = string.Empty;
        public string HeadBadge { get; set; } = string.Empty; // Badge of department head (optional)
        public string Status { get; set; } = "Active"; // Active, Inactive
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

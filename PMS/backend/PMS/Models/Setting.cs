namespace PMS.Models
{
    // System-wide settings (key/value). Used by Admin → Settings page.
    public class Setting
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Category { get; set; } = "General"; // General, Security
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}

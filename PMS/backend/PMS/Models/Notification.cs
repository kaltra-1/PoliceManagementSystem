namespace PMS.Models
{
    // FR_42 / FR_44 — In-app notifications, per recipient
    public class Notification
    {
        public int Id { get; set; }
        // Either RecipientBadge or RecipientRole (broadcast) is set.
        public string RecipientBadge { get; set; } = string.Empty;
        public string RecipientRole { get; set; } = string.Empty; // empty = direct only
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = "Info"; // Info, Warning, Alert, Success
        public string? Link { get; set; } // optional reference, e.g. CAS-2024-0001
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

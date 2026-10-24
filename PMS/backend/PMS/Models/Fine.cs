namespace PMS.Models
{
    public class Fine
    {
        public int Id { get; set; }
        public string FineNumber { get; set; } = string.Empty; // e.g. FIN-0001
        public string LicensePlate { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;
        public string ViolationType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string IssuingOfficerBadge { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Unpaid"; // Unpaid, Paid, Processing, Contested
        public string? Notes { get; set; }
        // Link to citizen user if registered
        public int? CitizenUserId { get; set; }
    }
}

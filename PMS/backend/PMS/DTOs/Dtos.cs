namespace PMS.DTOs
{
    // Auth
    public class LoginRequest
    {
        public string BadgeNumber { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public int Id { get; set; }
        public string BadgeNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }

    // User
    public class CreateUserRequest
    {
        public string BadgeNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
    }

    public class UpdateUserRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    // Case
    public class CreateCaseRequest
    {
        public string CrimeType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public DateTime IncidentDate { get; set; }
        public string Priority { get; set; } = "Low";
        public string ReportingOfficerBadge { get; set; } = string.Empty;
        public int WitnessCount { get; set; } = 0;
        public bool SuspectsIdentified { get; set; } = false;
    }

    public class UpdateCaseRequest
    {
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public int? AssignedOfficerId { get; set; }
        public int? AssignedDetectiveId { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    // Evidence
    public class CreateEvidenceRequest
    {
        public int CaseId { get; set; }
        public string CaseNumber { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CollectedBy { get; set; } = string.Empty;
        public string StorageLocation { get; set; } = string.Empty;
    }

    // Suspect
    public class CreateSuspectRequest
    {
        public string FullName { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string PhysicalDescription { get; set; } = string.Empty;
        public int CaseId { get; set; }
        public string CaseNumber { get; set; } = string.Empty;
        public string Charge { get; set; } = string.Empty;
        public string AddedByBadge { get; set; } = string.Empty;
    }

    // Arrest
    public class CreateArrestRequest
    {
        public string SuspectName { get; set; } = string.Empty;
        public int CaseId { get; set; }
        public string CaseNumber { get; set; } = string.Empty;
        public string Charge { get; set; } = string.Empty;
        public string ArrestLocation { get; set; } = string.Empty;
        public string ArrestingOfficerBadge { get; set; } = string.Empty;
    }

    // Fine
    public class CreateFineRequest
    {
        public string LicensePlate { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;
        public string ViolationType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string IssuingOfficerBadge { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public int? CitizenUserId { get; set; }
    }

    // Forensic Report
    public class CreateForensicReportRequest
    {
        public int CaseId { get; set; }
        public string CaseNumber { get; set; } = string.Empty;
        public string EvidenceNumber { get; set; } = string.Empty;
        public string AnalysisType { get; set; } = string.Empty;
        public string Findings { get; set; } = string.Empty;
        public string AnalystBadge { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
    }

    public class UpdateForensicReportRequest
    {
        public int ProgressPercent { get; set; }
        public string Findings { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    // Citizen Report
    public class CreateCitizenReportRequest
    {
        public int CitizenUserId { get; set; }
        public string IncidentType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime IncidentDate { get; set; }
    }

    // Stats
    public class DashboardStats
    {
        public int TotalUsers { get; set; }
        public int ActiveCases { get; set; }
        public int ClosedCases { get; set; }
        public int UrgentCases { get; set; }
        public int TotalEvidenceItems { get; set; }
        public int PendingForensicReports { get; set; }
        public int FinesIssuedToday { get; set; }
        public decimal FinesCollectedThisMonth { get; set; }
    }

    // Witness
    public class CreateWitnessRequest
    {
        public int CaseId { get; set; }
        public string CaseNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string ContactInfo { get; set; } = string.Empty;
        public string Testimony { get; set; } = string.Empty;
        public bool IsConfidential { get; set; } = false;
        public string AddedByBadge { get; set; } = string.Empty;
    }

    // Patrol
    public class CreatePatrolRequest
    {
        public string OfficerBadge { get; set; } = string.Empty;
        public string OfficerName { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Notes { get; set; } = string.Empty;
    }

    // Missing Person
    public class CreateMissingPersonRequest
    {
        public int CitizenUserId { get; set; }
        public string MissingPersonName { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string PhysicalDescription { get; set; } = string.Empty;
        public string LastKnownLocation { get; set; } = string.Empty;
        public DateTime LastSeenDate { get; set; }
        public string ContactInfo { get; set; } = string.Empty;
        public string AdditionalNotes { get; set; } = string.Empty;
    }

    // Driving License
    public class CreateDrivingLicenseRequest
    {
        public string LicenseNumber { get; set; } = string.Empty;
        public int? CitizenUserId { get; set; }
        public string HolderName { get; set; } = string.Empty;
        public string HolderNationalId { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public DateTime ExpiryDate { get; set; }
    }

    public class FlagLicenseRequest
    {
        public string Status { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string OfficerBadge { get; set; } = string.Empty;
    }

    // Change Password (UC3)
    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    // Audit Log filter
    public class AuditLogQuery
    {
        public string? UserBadge { get; set; }
        public string? ActionType { get; set; }
        public string? EntityType { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }

    // FR_09 — Departments
    public class CreateDepartmentRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string HeadBadge { get; set; } = string.Empty;
    }

    public class UpdateDepartmentRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string HeadBadge { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";
    }

    // Settings
    public class UpdateSettingsRequest
    {
        public Dictionary<string, string> Settings { get; set; } = new();
    }

    // Notifications (FR_42 / FR_44)
    public class CreateNotificationRequest
    {
        public string RecipientBadge { get; set; } = string.Empty;
        public string RecipientRole { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = "Info";
        public string? Link { get; set; }
    }

    // UC39 — Admin reset password
    public class AdminResetPasswordRequest
    {
        public string NewPassword { get; set; } = string.Empty;
    }

    // UC14 — Detective requests case closure (admin must approve)
    public class CaseCloseRequest
    {
        public string Reason { get; set; } = string.Empty;
    }

    // FR_10 — Assign officer/detective to case
    public class AssignOfficerRequest
    {
        public int? OfficerId { get; set; }
        public int? DetectiveId { get; set; }
    }
}

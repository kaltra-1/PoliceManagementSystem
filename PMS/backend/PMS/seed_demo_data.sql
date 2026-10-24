/*
  PMS Demo Data Seed
  Run after migrations have created the database.
  Database expected: PMS_Database on localhost\SQLEXPRESS

  Demo login password for ALL seeded users: password

  Example command without SSMS:
  sqlcmd -S localhost\SQLEXPRESS -d PMS_Database -E -i seed_demo_data.sql
*/

USE [PMS_Database];
GO

SET NOCOUNT ON;
GO

-- PasswordHash below is a BCrypt hash for: password
-- It is used only for demo/testing users.
DECLARE @PasswordHash NVARCHAR(MAX) = N'$2a$12$FjEa37t8x3GSl.sxLn/VFORH3WhoPGaxA6eTgmIAooX./5c42TNse';

-- Clean old demo data. Order matters.
DELETE FROM [Notifications];
DELETE FROM [Settings];
DELETE FROM [Departments];
DELETE FROM [AuditLogs];
DELETE FROM [DrivingLicenses];
DELETE FROM [MissingPersonReports];
DELETE FROM [Patrols];
DELETE FROM [Witnesses];
DELETE FROM [CitizenReports];
DELETE FROM [ForensicReports];
DELETE FROM [Fines];
DELETE FROM [Arrests];
DELETE FROM [Suspects];
DELETE FROM [Evidences];
DELETE FROM [Cases];
DELETE FROM [Users];
GO

DBCC CHECKIDENT ('Users', RESEED, 0);
DBCC CHECKIDENT ('Cases', RESEED, 0);
DBCC CHECKIDENT ('Evidences', RESEED, 0);
DBCC CHECKIDENT ('Suspects', RESEED, 0);
DBCC CHECKIDENT ('Arrests', RESEED, 0);
DBCC CHECKIDENT ('Fines', RESEED, 0);
DBCC CHECKIDENT ('ForensicReports', RESEED, 0);
DBCC CHECKIDENT ('CitizenReports', RESEED, 0);
DBCC CHECKIDENT ('Witnesses', RESEED, 0);
DBCC CHECKIDENT ('Patrols', RESEED, 0);
DBCC CHECKIDENT ('MissingPersonReports', RESEED, 0);
DBCC CHECKIDENT ('DrivingLicenses', RESEED, 0);
DBCC CHECKIDENT ('AuditLogs', RESEED, 0);
DBCC CHECKIDENT ('Departments', RESEED, 0);
DBCC CHECKIDENT ('Settings', RESEED, 0);
DBCC CHECKIDENT ('Notifications', RESEED, 0);
GO

DECLARE @PasswordHash NVARCHAR(MAX) = N'$2a$12$FjEa37t8x3GSl.sxLn/VFORH3WhoPGaxA6eTgmIAooX./5c42TNse';

SET IDENTITY_INSERT [Users] ON;
INSERT INTO [Users] ([Id], [BadgeNumber], [FullName], [Email], [PasswordHash], [Role], [Department], [Status], [CreatedAt], [LastLogin], [FailedLoginAttempts], [LockedUntil]) VALUES
(1, N'ADMIN001', N'System Administrator', N'admin@pms.com', @PasswordHash, N'Admin', N'Administration', N'Active', SYSUTCDATETIME(), NULL, 0, NULL),
(2, N'OFF001', N'Arben Hoxha', N'arben.hoxha@pms.com', @PasswordHash, N'Officer', N'Patrol Unit', N'Active', SYSUTCDATETIME(), NULL, 0, NULL),
(3, N'DET001', N'Elira Dervishi', N'elira.dervishi@pms.com', @PasswordHash, N'Detective', N'Criminal Investigation', N'Active', SYSUTCDATETIME(), NULL, 0, NULL),
(4, N'FOR001', N'Gentian Kola', N'gentian.kola@pms.com', @PasswordHash, N'Forensic', N'Forensic Laboratory', N'Active', SYSUTCDATETIME(), NULL, 0, NULL),
(5, N'TRF001', N'Klodian Meta', N'klodian.meta@pms.com', @PasswordHash, N'Traffic', N'Traffic Police', N'Active', SYSUTCDATETIME(), NULL, 0, NULL),
(6, N'CIT001', N'Mira Leka', N'mira.leka@example.com', @PasswordHash, N'Citizen', N'Public Portal', N'Active', SYSUTCDATETIME(), NULL, 0, NULL),
(7, N'CIT002', N'Bledi Marku', N'bledi.marku@example.com', @PasswordHash, N'Citizen', N'Public Portal', N'Active', SYSUTCDATETIME(), NULL, 0, NULL);
SET IDENTITY_INSERT [Users] OFF;
GO

SET IDENTITY_INSERT [Departments] ON;
INSERT INTO [Departments] ([Id], [Name], [Code], [Description], [HeadBadge], [Status], [CreatedAt]) VALUES
(1, N'Administration', N'ADM', N'System administration and user management.', N'ADMIN001', N'Active', SYSUTCDATETIME()),
(2, N'Patrol Unit', N'PAT', N'Daily patrols and first response.', N'OFF001', N'Active', SYSUTCDATETIME()),
(3, N'Criminal Investigation', N'CID', N'Case investigation and suspect tracking.', N'DET001', N'Active', SYSUTCDATETIME()),
(4, N'Forensic Laboratory', N'FOR', N'Evidence analysis and forensic reporting.', N'FOR001', N'Active', SYSUTCDATETIME()),
(5, N'Traffic Police', N'TRF', N'Traffic violations, fines and driving license checks.', N'TRF001', N'Active', SYSUTCDATETIME());
SET IDENTITY_INSERT [Departments] OFF;
GO

SET IDENTITY_INSERT [Settings] ON;
INSERT INTO [Settings] ([Id], [Key], [Value], [Category], [UpdatedAt]) VALUES
(1, N'SystemName', N'Police Management System', N'General', SYSUTCDATETIME()),
(2, N'DefaultLanguage', N'English', N'General', SYSUTCDATETIME()),
(3, N'LoginMaxAttempts', N'5', N'Security', SYSUTCDATETIME()),
(4, N'LockoutMinutes', N'15', N'Security', SYSUTCDATETIME());
SET IDENTITY_INSERT [Settings] OFF;
GO

SET IDENTITY_INSERT [Cases] ON;
INSERT INTO [Cases] ([Id], [CaseNumber], [CrimeType], [Description], [Location], [District], [IncidentDate], [FiledDate], [Status], [Priority], [AssignedOfficerId], [AssignedDetectiveId], [ReportingOfficerBadge], [WitnessCount], [SuspectsIdentified]) VALUES
(1, N'CAS-2026-0001', N'Theft', N'Reported theft of electronic equipment from a small office.', N'Rruga e Durresit', N'Tirana', '2026-05-20T10:30:00', SYSUTCDATETIME(), N'Active', N'Medium', 2, 3, N'OFF001', 2, 1),
(2, N'CAS-2026-0002', N'Burglary', N'Forced entry reported at a residential apartment.', N'Lagjja 13', N'Durres', '2026-05-21T23:15:00', SYSUTCDATETIME(), N'Urgent', N'High', 2, 3, N'OFF001', 1, 1),
(3, N'CAS-2026-0003', N'Fraud', N'Citizen reported online payment fraud.', N'Blloku', N'Tirana', '2026-05-18T14:00:00', SYSUTCDATETIME(), N'Open', N'Low', 2, 3, N'OFF001', 0, 0);
SET IDENTITY_INSERT [Cases] OFF;
GO

SET IDENTITY_INSERT [Evidences] ON;
INSERT INTO [Evidences] ([Id], [EvidenceNumber], [CaseId], [CaseNumber], [Type], [Description], [ChainOfCustody], [Status], [CollectedBy], [CollectedAt], [StorageLocation]) VALUES
(1, N'EVD-0001', 1, N'CAS-2026-0001', N'Digital', N'Security camera footage copied from office DVR.', N'Collected by OFF001, transferred to forensic lab FOR001.', N'Processing', N'OFF001', '2026-05-20T12:00:00', N'Lab Storage A1'),
(2, N'EVD-0002', 2, N'CAS-2026-0002', N'Physical', N'Fingerprint sample from door handle.', N'Collected by OFF001 and sealed for analysis.', N'Lab Pending', N'OFF001', '2026-05-22T01:10:00', N'Lab Storage B2'),
(3, N'EVD-0003', 3, N'CAS-2026-0003', N'Documentary', N'Bank transaction screenshot submitted by citizen.', N'Submitted through public portal.', N'Logged', N'CIT001', '2026-05-18T15:30:00', N'Digital Archive');
SET IDENTITY_INSERT [Evidences] OFF;
GO

SET IDENTITY_INSERT [Suspects] ON;
INSERT INTO [Suspects] ([Id], [FullName], [Age], [Gender], [PhysicalDescription], [CaseId], [CaseNumber], [Charge], [Status], [AddedAt], [AddedByBadge]) VALUES
(1, N'Dritan Basha', 32, N'Male', N'Medium height, black jacket seen on camera.', 1, N'CAS-2026-0001', N'Theft', N'Identified', SYSUTCDATETIME(), N'DET001'),
(2, N'Unknown Male', 28, N'Male', N'Approximate age 25-30, face partially covered.', 2, N'CAS-2026-0002', N'Burglary', N'At Large', SYSUTCDATETIME(), N'DET001');
SET IDENTITY_INSERT [Suspects] OFF;
GO

SET IDENTITY_INSERT [Arrests] ON;
INSERT INTO [Arrests] ([Id], [ArrestNumber], [SuspectName], [CaseId], [CaseNumber], [Charge], [ArrestedAt], [ArrestLocation], [ArrestingOfficerBadge], [Status]) VALUES
(1, N'ARR-0001', N'Dritan Basha', 1, N'CAS-2026-0001', N'Theft', '2026-05-22T09:20:00', N'Tirana Police Station', N'OFF001', N'Detained');
SET IDENTITY_INSERT [Arrests] OFF;
GO

SET IDENTITY_INSERT [Fines] ON;
INSERT INTO [Fines] ([Id], [FineNumber], [LicensePlate], [VehicleType], [ViolationType], [Amount], [IssuingOfficerBadge], [Location], [IssuedAt], [Status], [Notes], [CitizenUserId]) VALUES
(1, N'FIN-0001', N'AA123BB', N'Car', N'Speeding', 5000.00, N'TRF001', N'Unaza e Madhe, Tirana', '2026-05-23T11:15:00', N'Unpaid', N'Exceeded speed limit by 20 km/h.', 6),
(2, N'FIN-0002', N'DR456CC', N'Car', N'Illegal Parking', 2000.00, N'TRF001', N'Durres City Center', '2026-05-24T16:40:00', N'Paid', N'Paid online by citizen.', 7),
(3, N'FIN-0003', N'AB789DE', N'Motorcycle', N'No Helmet', 3000.00, N'TRF001', N'Rruga Kavajes, Tirana', '2026-05-25T08:20:00', N'Contested', N'Citizen requested review.', NULL);
SET IDENTITY_INSERT [Fines] OFF;
GO

SET IDENTITY_INSERT [ForensicReports] ON;
INSERT INTO [ForensicReports] ([Id], [ReportNumber], [CaseId], [CaseNumber], [EvidenceNumber], [AnalysisType], [Findings], [ProgressPercent], [Status], [AnalystBadge], [ReceivedAt], [SubmittedAt], [DueDate]) VALUES
(1, N'FR-0001', 1, N'CAS-2026-0001', N'EVD-0001', N'Digital', N'Video confirms one suspect entering the office after closing time.', 85, N'Confirming', N'FOR001', '2026-05-20T13:00:00', NULL, '2026-05-28T17:00:00'),
(2, N'FR-0002', 2, N'CAS-2026-0002', N'EVD-0002', N'Fingerprints', N'Partial fingerprint recovered; comparison still in progress.', 55, N'In Progress', N'FOR001', '2026-05-22T02:30:00', NULL, '2026-05-30T17:00:00');
SET IDENTITY_INSERT [ForensicReports] OFF;
GO

SET IDENTITY_INSERT [CitizenReports] ON;
INSERT INTO [CitizenReports] ([Id], [ReportNumber], [CitizenUserId], [IncidentType], [Description], [Location], [IncidentDate], [SubmittedAt], [Status], [AssignedToBadge], [OfficialNotes]) VALUES
(1, N'CR-0001', 6, N'Fraud', N'Citizen reported unauthorized online transaction.', N'Tirana', '2026-05-18T14:00:00', SYSUTCDATETIME(), N'Assigned', N'DET001', N'Converted into case CAS-2026-0003.'),
(2, N'CR-0002', 7, N'Noise Complaint', N'Loud music reported after midnight.', N'Durres', '2026-05-24T00:30:00', SYSUTCDATETIME(), N'Under Review', N'OFF001', N'Patrol assigned for verification.');
SET IDENTITY_INSERT [CitizenReports] OFF;
GO

SET IDENTITY_INSERT [Witnesses] ON;
INSERT INTO [Witnesses] ([Id], [WitnessNumber], [CaseId], [CaseNumber], [FullName], [ContactInfo], [Testimony], [IsConfidential], [AddedByBadge], [AddedAt]) VALUES
(1, N'WIT-0001', 1, N'CAS-2026-0001', N'Ina Prifti', N'ina.prifti@example.com', N'Saw a person leaving the office building around 22:00.', 0, N'OFF001', SYSUTCDATETIME()),
(2, N'WIT-0002', 2, N'CAS-2026-0002', N'Redacted Witness', N'Confidential', N'Provided information about suspicious movement near the building.', 1, N'DET001', SYSUTCDATETIME());
SET IDENTITY_INSERT [Witnesses] OFF;
GO

SET IDENTITY_INSERT [Patrols] ON;
INSERT INTO [Patrols] ([Id], [PatrolNumber], [OfficerBadge], [OfficerName], [Route], [Area], [StartTime], [EndTime], [Status], [Notes], [CreatedAt]) VALUES
(1, N'PAT-0001', N'OFF001', N'Arben Hoxha', N'Central Tirana route', N'Tirana Center', '2026-05-25T08:00:00', '2026-05-25T16:00:00', N'Active', N'Focus on public squares and main roads.', SYSUTCDATETIME()),
(2, N'PAT-0002', N'OFF001', N'Arben Hoxha', N'Evening residential route', N'Durres Residential Area', '2026-05-25T18:00:00', '2026-05-26T02:00:00', N'Scheduled', N'Follow-up after burglary reports.', SYSUTCDATETIME());
SET IDENTITY_INSERT [Patrols] OFF;
GO

SET IDENTITY_INSERT [MissingPersonReports] ON;
INSERT INTO [MissingPersonReports] ([Id], [ReportNumber], [CitizenUserId], [MissingPersonName], [DateOfBirth], [PhysicalDescription], [LastKnownLocation], [LastSeenDate], [ContactInfo], [AdditionalNotes], [Status], [AssignedToBadge], [SubmittedAt]) VALUES
(1, N'MPR-0001', 6, N'Andi Leka', '2008-04-12T00:00:00', N'Brown hair, approximately 175 cm tall.', N'Near Tirana East Gate', '2026-05-24T19:20:00', N'mira.leka@example.com', N'Family reported that the person did not return home.', N'Active', N'OFF001', SYSUTCDATETIME());
SET IDENTITY_INSERT [MissingPersonReports] OFF;
GO

SET IDENTITY_INSERT [DrivingLicenses] ON;
INSERT INTO [DrivingLicenses] ([Id], [LicenseNumber], [CitizenUserId], [HolderName], [HolderNationalId], [Category], [IssueDate], [ExpiryDate], [Status], [SuspensionReason], [FlaggedByBadge], [FlaggedAt], [CreatedAt]) VALUES
(1, N'DL-AL-0001', 6, N'Mira Leka', N'J60412001A', N'B', '2022-06-10T00:00:00', '2032-06-10T00:00:00', N'Valid', NULL, NULL, NULL, SYSUTCDATETIME()),
(2, N'DL-AL-0002', 7, N'Bledi Marku', N'K70523002B', N'B', '2020-03-15T00:00:00', '2030-03-15T00:00:00', N'Suspended', N'Multiple unpaid traffic fines.', N'TRF001', SYSUTCDATETIME(), SYSUTCDATETIME());
SET IDENTITY_INSERT [DrivingLicenses] OFF;
GO

SET IDENTITY_INSERT [Notifications] ON;
INSERT INTO [Notifications] ([Id], [RecipientBadge], [RecipientRole], [Title], [Message], [Type], [Link], [IsRead], [CreatedAt]) VALUES
(1, N'ADMIN001', N'', N'Demo data loaded', N'The PMS database has been filled with sample users, cases, patrols, fines and reports.', N'Success', NULL, 0, SYSUTCDATETIME()),
(2, N'DET001', N'', N'New case assigned', N'Case CAS-2026-0003 has been assigned for investigation.', N'Info', N'CAS-2026-0003', 0, SYSUTCDATETIME()),
(3, N'', N'Traffic', N'Contested fine', N'Fine FIN-0003 was contested and requires review.', N'Warning', N'FIN-0003', 0, SYSUTCDATETIME()),
(4, N'OFF001', N'', N'Missing person report', N'Missing person report MPR-0001 has been assigned to your unit.', N'Alert', N'MPR-0001', 0, SYSUTCDATETIME());
SET IDENTITY_INSERT [Notifications] OFF;
GO

SET IDENTITY_INSERT [AuditLogs] ON;
INSERT INTO [AuditLogs] ([Id], [ActionType], [EntityType], [EntityId], [UserBadge], [UserRole], [Description], [OldValue], [NewValue], [Timestamp]) VALUES
(1, N'SEED', N'Database', NULL, N'system', N'Admin', N'Demo data inserted for project presentation.', NULL, NULL, SYSUTCDATETIME()),
(2, N'CREATE', N'User', N'1', N'system', N'Admin', N'Admin user ADMIN001 created by seed file.', NULL, NULL, SYSUTCDATETIME()),
(3, N'CREATE', N'Case', N'CAS-2026-0001', N'system', N'Admin', N'Demo case CAS-2026-0001 created.', NULL, NULL, SYSUTCDATETIME());
SET IDENTITY_INSERT [AuditLogs] OFF;
GO

PRINT 'PMS demo data inserted successfully.';
PRINT 'Demo users: ADMIN001, OFF001, DET001, FOR001, TRF001, CIT001, CIT002';
PRINT 'Password for all demo users: password';
GO

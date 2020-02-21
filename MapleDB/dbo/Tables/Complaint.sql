CREATE TABLE [dbo].[Complaint]
(
	[Id] INT IDENTITY(1,1) PRIMARY KEY, 
    [ComplaintTypeId] INT NOT NULL, 
    [ComplaintStatusId] INT NOT NULL, 
    [IncidentDate] DATETIME NOT NULL, 
    [ReportedDate] DATETIME NOT NULL DEFAULT SYSDATETIME(), 
    [Details] VARCHAR(MAX) NULL, 
    [TenantId] INT NOT NULL, 
    [PropertyManagerId] INT NOT NULL
)

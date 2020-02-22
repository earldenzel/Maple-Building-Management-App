CREATE TABLE [dbo].[Account]
(	
	[Id] INT IDENTITY(1,1) PRIMARY KEY, 
    [FirstName] NVARCHAR(50) NOT NULL, 
    [LastName] NVARCHAR(50) NOT NULL, 
    [EmailAddress] NVARCHAR(50) NOT NULL, 
    [Tenant] BIT NOT NULL, 
    [PropertyCode] NVARCHAR(50) NULL, 
    [Password] NVARCHAR(MAX) NOT NULL 
)

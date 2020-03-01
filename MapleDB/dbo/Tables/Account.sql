CREATE TABLE [dbo].[Account]
(	
	[Id] INT IDENTITY(1,1) PRIMARY KEY, 
    [FirstName] NVARCHAR(50) NOT NULL, 
    [LastName] NVARCHAR(50) NOT NULL, 
    [EmailAddress] NVARCHAR(50) NOT NULL, 
    [Tenant] BIT NOT NULL DEFAULT 0, 
    [PropertyCode] NVARCHAR(50) NULL, 
    [Password] NVARCHAR(MAX) NOT NULL, 
    [PhoneNumber] NVARCHAR(10) NULL, 
    [TwoFactor] BIT NOT NULL DEFAULT 0, 
    [Admin] BIT NOT NULL DEFAULT 0 
)

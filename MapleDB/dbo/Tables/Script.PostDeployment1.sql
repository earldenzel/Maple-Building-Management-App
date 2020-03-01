/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

BEGIN
   IF NOT EXISTS (SELECT * FROM [dbo].[ComplaintType])
   BEGIN
	INSERT INTO [dbo].[ComplaintType] ([Id], [Value]) VALUES (1, 'Emergency');
	INSERT INTO [dbo].[ComplaintType] ([Id], [Value]) VALUES (2, 'Pests');
	INSERT INTO [dbo].[ComplaintType] ([Id], [Value]) VALUES (3, 'Maintenance');
	INSERT INTO [dbo].[ComplaintType] ([Id], [Value]) VALUES (4, 'Noise');
   END
END
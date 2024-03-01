CREATE TABLE [dbo].[CustomerConnectors]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [CustomerId] INT NOT NULL, 
    [ConnectorId] INT NOT NULL, 
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(), 
    [CreatedBy] INT NOT NULL
)

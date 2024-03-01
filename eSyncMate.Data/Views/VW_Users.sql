CREATE VIEW [dbo].[VW_Users]
	AS SELECT [Id], [FirstName], [LastName], [Email], [Mobile], [Password], [Status], [CreatedDate], [CreatedBy], [UserType]
	FROM Users WITH (NOLOCK)
GO
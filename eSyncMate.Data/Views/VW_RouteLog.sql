CREATE VIEW [dbo].[VW_RouteLog]
AS
SELECT [Id], [RouteId],[Type], [Message], [Details], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy]
FROM [dbo].[RouteLog] WITH (NOLOCK)

GO
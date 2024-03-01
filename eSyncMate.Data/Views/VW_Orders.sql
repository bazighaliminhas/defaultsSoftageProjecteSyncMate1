CREATE VIEW [dbo].[VW_Orders] AS 
SELECT O.*, C.Name CustomerName
FROM Orders O
	INNER JOIN Customers C ON O.CustomerId = C.Id
GO

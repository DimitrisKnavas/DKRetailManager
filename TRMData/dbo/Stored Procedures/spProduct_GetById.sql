CREATE PROCEDURE [dbo].[spProduct_GetById]
	@id int 	
AS
BEGIN
     SET NOCOUNT ON;
	 SELECT Id, ProductName, [Description], RetailPrice, QuantityInStock, IsTaxable
	 from dbo.Product
	 where Id = @id;
END

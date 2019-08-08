CREATE PROCEDURE [dbo].[spProduct_QuantityUpdate]
	@ProductId int ,
	@QuantityInStock int
AS
BEGIN
       SET NOCOUNT ON;

	   UPDATE dbo.Product
	   SET QuantityInStock = @QuantityInStock
	   WHERE Id = @ProductId;
END

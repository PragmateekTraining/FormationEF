CREATE PROCEDURE EF_Sum @a int, @b int
AS
BEGIN
	SELECT @a + @b as [sum]
END
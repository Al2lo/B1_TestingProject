create database TestingProjectWPF;

create table FirstTaskTable(Date datetime, LatinString varchar(10), RussianString varchar(10), IntegerNumber int, DecimalNumber decimal(10,8));

CREATE PROCEDURE CalculateSumAndMedian
    @FullSum BIGINT OUTPUT,
    @Median FLOAT OUTPUT
AS
BEGIN
    SELECT @FullSum = SUM(CAST(IntegerNumber AS BIGINT))
    FROM FirstTaskTable;

    WITH SortedDecimals AS (
        SELECT DecimalNumber,
               ROW_NUMBER() OVER (ORDER BY DecimalNumber) AS RowNum,
               COUNT(*) OVER () AS TotalCount
        FROM FirstTaskTable
        WHERE DecimalNumber IS NOT NULL
    )
    SELECT @Median = AVG(DecimalNumber)
    FROM SortedDecimals
    WHERE RowNum IN ((TotalCount + 1) / 2, (TotalCount + 2) / 2);
END;


begin
DECLARE @Sum BIGINT; 
DECLARE @Median FLOAT;

EXEC CalculateSumAndMedian @FullSum = @Sum OUTPUT, @Median = @Median OUTPUT;

SELECT @Sum AS FullSum, @Median AS Median;

end;
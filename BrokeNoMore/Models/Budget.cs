namespace BrokeNoMore.Models;

public class Budget
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Limit { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateTime PeriodStartDate { get; set; }
    public DateTime PeriodEndDate { get; set; }
}

namespace BrokeNoMore.Models;

public class Transaction
{
    public long Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public int? BudgetId { get; set; }
}

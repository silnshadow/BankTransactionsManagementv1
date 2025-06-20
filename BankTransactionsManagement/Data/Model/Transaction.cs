public class Transaction
{
    public int Id { get; set; }
    public required string Description { get; set; }
    public decimal Amount { get; set; }
    public required string TransactionType { get; set; } // e.g., "Credit", "Debit"
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    }

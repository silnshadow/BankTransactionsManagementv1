public class Transaction
{
    public int Id { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public string TransactionType { get; set; } // e.g., "Credit", "Debit"
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    }
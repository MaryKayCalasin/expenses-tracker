namespace ExpensesTracker.Models;

public class Expense
{
    public int Id { get; set; }
    public int UserId { get; set; }        // ← ADD THIS
    public decimal Amount { get; set; }
    public string Category { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime Date { get; set; }
    public string? ReceiptText { get; set; }
    public bool AiParsed { get; set; } = false;

    public User? User { get; set; }        // ← ADD THIS
}
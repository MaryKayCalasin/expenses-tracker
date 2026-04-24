namespace ExpensesTracker.Models;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property — one user has many expenses
    public List<Expense> Expenses { get; set; } = new();
}
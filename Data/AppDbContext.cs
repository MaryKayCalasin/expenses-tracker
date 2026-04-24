using Microsoft.EntityFrameworkCore;
using ExpensesTracker.Models;

namespace ExpensesTracker.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<User> Users => Set<User>();    // ← ADD THIS
}
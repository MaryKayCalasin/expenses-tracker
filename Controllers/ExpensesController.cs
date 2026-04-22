using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpensesTracker.Data;
using ExpensesTracker.Models;

namespace ExpensesTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExpensesController : ControllerBase
{
    private readonly AppDbContext _db;

    public ExpensesController(AppDbContext db)
    {
        _db = db;
    }

    // GET /api/expenses
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var expenses = await _db.Expenses
            .OrderByDescending(e => e.Date)
            .ToListAsync();

        return Ok(expenses);
    }

    // POST /api/expenses
    [HttpPost]
    public async Task<IActionResult> Create(Expense expense)
    {
        expense.Date = DateTime.Now;
        _db.Expenses.Add(expense);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), expense);
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ExpensesTracker.Data;
using ExpensesTracker.Models;

namespace ExpensesTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]  // ← ALL endpoints now require login
public class ExpensesController : ControllerBase
{
    private readonly AppDbContext _db;

    public ExpensesController(AppDbContext db)
    {
        _db = db;
    }

    // Helper — gets the logged-in user's ID from their token
    private int GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(claim!);
    }

    // GET /api/expenses — only YOUR expenses
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = GetUserId();
        var expenses = await _db.Expenses
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.Date)
            .ToListAsync();

        return Ok(expenses);
    }

    // POST /api/expenses
    [HttpPost]
    public async Task<IActionResult> Create(Expense expense)
    {
        expense.UserId = GetUserId();
        expense.Date = DateTime.Now;
        _db.Expenses.Add(expense);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), expense);
    }

    // DELETE /api/expenses/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();
        var expense = await _db.Expenses
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

        if (expense == null)
            return NotFound();

        _db.Expenses.Remove(expense);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // GET /api/expenses/summary
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var userId = GetUserId();
        var expenses = await _db.Expenses
            .Where(e => e.UserId == userId)
            .ToListAsync();

        var summary = expenses
            .GroupBy(e => e.Category)
            .Select(group => new
            {
                Category = group.Key,
                Total = group.Sum(e => e.Amount),
                Count = group.Count()
            })
            .OrderByDescending(s => s.Total)
            .ToList();

        return Ok(summary);
    }
}
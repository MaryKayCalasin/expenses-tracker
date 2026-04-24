using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ExpensesTracker.Data;
using ExpensesTracker.Models;
using System.Text;
using System.Text.Json;

namespace ExpensesTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AiController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public AiController(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    private int GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(claim!);
    }

    [HttpPost("parse-receipt")]
    public async Task<IActionResult> ParseReceipt([FromBody] ReceiptRequest request)
    {
        var apiKey = _config["OpenRouter:ApiKey"];

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        client.DefaultRequestHeaders.Add("HTTP-Referer", "http://localhost:5210");

        var prompt = "Parse this receipt and extract expense info. Receipt: "
            + request.ReceiptText
            + " Reply with ONLY this JSON, nothing else: {\"amount\": 149.90, \"category\": \"Uniform\", \"description\": \"Short description\"}. Categories: Food, Transport, Uniform, Medical, Education, Other";

        var body = new
        {
            model = "google/gemma-3-4b-it:free",
            max_tokens = 200,
            messages = new[]
            {
                new { role = "user", content = prompt }
            }
        };

        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(
            "https://openrouter.ai/api/v1/chat/completions", content);

        var responseText = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"AI response: {responseText}");

        using var doc = JsonDocument.Parse(responseText);

        if (doc.RootElement.TryGetProperty("error", out var error))
        {
            return BadRequest($"AI error: {error}");
        }

        var aiReply = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        var startIndex = aiReply!.IndexOf('{');
        var endIndex = aiReply.LastIndexOf('}');
        var jsonOnly = aiReply.Substring(startIndex, endIndex - startIndex + 1);

        var parsed = JsonSerializer.Deserialize<ParsedExpense>(jsonOnly,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var expense = new Expense
        {
            Amount = parsed!.Amount,
            Category = parsed.Category,
            Description = parsed.Description,
            Date = DateTime.Now,
            ReceiptText = request.ReceiptText,
            AiParsed = true,
            UserId = GetUserId()
        };

        _db.Expenses.Add(expense);
        await _db.SaveChangesAsync();

        return Ok(expense);
    }
}

public class ReceiptRequest
{
    public string ReceiptText { get; set; } = "";
}

public class ParsedExpense
{
    public decimal Amount { get; set; }
    public string Category { get; set; } = "";
    public string Description { get; set; } = "";
}
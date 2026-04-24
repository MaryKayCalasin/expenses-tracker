using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ExpensesTracker.Data;
using ExpensesTracker.Models;

namespace ExpensesTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public AuthController(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    // POST /api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register(AuthRequest request)
    {
        // Check if email already exists
        var exists = await _db.Users.AnyAsync(u => u.Email == request.Email);
        if (exists)
            return BadRequest("Email already registered");

        // Hash the password — never store plain text!
        var user = new User
        {
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok(new { message = "Account created!", email = user.Email });
    }

    // POST /api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login(AuthRequest request)
    {
        // Find user by email
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null)
            return Unauthorized("Invalid email or password");

        // Check password against hash
        var valid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!valid)
            return Unauthorized("Invalid email or password");

        // Create JWT token
        var token = CreateToken(user);
        return Ok(new { token, email = user.Email });
    }

    private string CreateToken(User user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class AuthRequest
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}
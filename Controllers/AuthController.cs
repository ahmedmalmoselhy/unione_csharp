using Microsoft.AspNetCore.Mvc;

namespace UniOne.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // Placeholder for login logic
        return Ok(new { token = "placeholder_token", user = new { email = request.Email } });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return Ok(new { message = "Logged out successfully" });
    }

    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new { user_id = "1", email = "user@example.com", roles = new[] { "student" } });
    }
}

public record LoginRequest(string Email, string Password);

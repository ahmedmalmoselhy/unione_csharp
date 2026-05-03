using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Security.Claims;
using UniOne.Application.Contracts;

namespace UniOne.Api.Controllers;

[ApiController]
[Route("api/v1/auth/tokens")]
[Authorize]
public class TokenController : ControllerBase
{
    private readonly IIdentityService _identityService;

    public TokenController(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTokens()
    {
        var userId = GetUserId();
        var tokens = await _identityService.GetActiveTokensAsync(userId, GetBearerToken());
        return Ok(new { tokens });
    }

    [HttpDelete("{tokenId}")]
    public async Task<IActionResult> RevokeToken(long tokenId)
    {
        var userId = GetUserId();
        var result = await _identityService.RevokeTokenAsync(userId, tokenId);
        if (!result) return NotFound();

        return Ok(new { message = "Token revoked" });
    }

    [HttpDelete]
    public async Task<IActionResult> RevokeAllTokens()
    {
        var userId = GetUserId();
        await _identityService.RevokeAllTokensAsync(userId);
        return Ok(new { message = "All tokens revoked" });
    }

    private long GetUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return long.Parse(userIdClaim!);
    }

    private string? GetBearerToken()
    {
        var authorization = Request.Headers[HeaderNames.Authorization].ToString();
        return authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
            ? authorization["Bearer ".Length..].Trim()
            : null;
    }
}

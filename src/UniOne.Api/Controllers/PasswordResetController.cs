using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniOne.Application.Contracts;
using UniOne.Application.DTOs;

namespace UniOne.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class PasswordResetController : ControllerBase
{
    private readonly IIdentityService _identityService;

    public PasswordResetController(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var result = await _identityService.ForgotPasswordAsync(request.Email);

        // Always return OK to avoid email enumeration
        return Ok(new { message = "If your email is in our system, you will receive a password reset link shortly." });
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var result = await _identityService.ResetPasswordAsync(request);
        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
        }

        return Ok(new { message = "Your password has been reset successfully." });
    }
}

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
            return BadRequest(new ValidationProblemDetails(ToErrorDictionary(result.Errors))
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "One or more validation errors occurred."
            });
        }

        return Ok(new { message = "Your password has been reset successfully." });
    }

    private static Dictionary<string, string[]> ToErrorDictionary(IEnumerable<Microsoft.AspNetCore.Identity.IdentityError> errors)
    {
        return errors
            .GroupBy(error => string.IsNullOrWhiteSpace(error.Code) ? "identity" : error.Code)
            .ToDictionary(group => group.Key, group => group.Select(error => error.Description).ToArray());
    }
}

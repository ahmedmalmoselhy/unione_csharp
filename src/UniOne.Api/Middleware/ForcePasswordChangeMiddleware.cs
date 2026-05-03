using Microsoft.AspNetCore.Http;
using UniOne.Application.Contracts;

namespace UniOne.Api.Middleware;

public class ForcePasswordChangeMiddleware
{
    private readonly RequestDelegate _next;

    public ForcePasswordChangeMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ICurrentUserService currentUserService)
    {
        if (currentUserService.IsAuthenticated && currentUserService.MustChangePassword)
        {
            var path = context.Request.Path.Value?.ToLower();

            // Allow password change and logout endpoints
            var isAllowed = path != null && (
                path.Contains("/auth/change-password") ||
                path.Contains("/auth/logout") ||
                path.Contains("/auth/me")
            );

            if (!isAllowed)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new
                {
                    message = "You must change your password before continuing.",
                    code = "MUST_CHANGE_PASSWORD"
                });
                return;
            }
        }

        await _next(context);
    }
}

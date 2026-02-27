using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Application.Shared;

internal static class Utils
{
    public static Guid? GetCurrentUserId(IHttpContextAccessor httpContextAccessor)
    {
        return Guid.TryParse(httpContextAccessor.HttpContext?.User.FindFirst("id")?.Value, out var userId) ? userId : null;
    }
}
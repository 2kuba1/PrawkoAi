using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Application.UnitTests.TestHelpers;

public static class HttpContextHelper
{
    public static IHttpContextAccessor WithUserId(Guid userId)
    {
        return new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(
                [
                    new Claim("id", userId.ToString())
                ]))
            }
        };
    }
}
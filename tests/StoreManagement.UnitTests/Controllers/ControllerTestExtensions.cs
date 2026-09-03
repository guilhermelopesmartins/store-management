using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace StoreManagement.UnitTests.Controllers;

public static class ControllerTestExtensions
{
    public static void SetCompanyIdClaim(this ControllerBase controller, Guid companyId)
    {
        var claims = new List<Claim> { new("companyId", companyId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }
}
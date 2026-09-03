using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace StoreManagement.UnitTests.Controllers;

public static class ControllerTestExtensions
{
    public static void SetCompanyIdClaim(this ControllerBase controller, Guid companyId)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Items["CompanyId"] = companyId;

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }
}
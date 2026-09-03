using Microsoft.AspNetCore.Authorization;
using StoreManagement.Api.Attributes;

namespace StoreManagement.Api.Middlewares;

public class CompanyValidationMiddleware
{
    private readonly RequestDelegate _next;

    public CompanyValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();

        var requiresAuthorization =
            endpoint?.Metadata.GetMetadata<IAuthorizeData>() != null;

        var skipValidation =
            endpoint?.Metadata.GetMetadata<SkipCompanyValidationAttribute>() != null;

        if (!requiresAuthorization || skipValidation)
        {
            await _next(context);
            return;
        }

        var claim = context.User.FindFirst("companyId")?.Value;

        if (!Guid.TryParse(claim, out var companyId))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Missing or invalid companyId claim.");
            return;
        }

        context.Items["CompanyId"] = companyId;

        await _next(context);
    }
}
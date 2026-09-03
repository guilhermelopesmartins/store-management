using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreManagement.Api.Attributes;
using StoreManagement.Api.DTOs;
using StoreManagement.Api.Services;

namespace StoreManagement.Api.Controllers;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
[SkipCompanyValidation]
public class AuthController : ControllerBase
{
    private readonly TokenService _tokenService;

    public AuthController(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    /// <summary>
    /// Test-only endpoint that issues a JWT containing a companyId claim,
    /// simulating a login without a real identity provider.
    /// </summary>
    [HttpPost("token")]
    public IActionResult GenerateToken(TokenRequestDto dto)
    {
        var (token, expiresAt) = _tokenService.GenerateToken(dto.CompanyId);

        return Ok(new TokenResponseDto { Token = token, ExpiresAt = expiresAt });
    }
}
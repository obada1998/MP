using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;

namespace Platform.Api.Controllers;

public abstract class ApiControllerBase : ControllerBase
{
    protected string CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)
        ?? User.FindFirstValue("nameid")
        ?? throw new UnauthorizedAccessException("Authenticated user id is missing.");
}

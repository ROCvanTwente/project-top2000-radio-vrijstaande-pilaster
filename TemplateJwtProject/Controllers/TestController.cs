using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TemplateJwtProject.Constants;

namespace TemplateJwtProject.Controllers;

/// <summary>
/// Controller for testing role-based authorization.
/// Contains endpoints with different authorization requirements.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TestController : ControllerBase
{
    /// <summary>
    /// Test endpoint accessible by users with the User role.
    /// </summary>
    /// <returns>A test message confirming access.</returns>
    /// <response code="200">User has access.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have the required role.</response>
    [HttpGet("user")]
    [Authorize(Roles = Roles.User)]
    public IActionResult UserEndpoint()
    {
        return Ok(new { message = "This endpoint is accessible by Users", user = User.Identity?.Name });
    }

    /// <summary>
    /// Test endpoint accessible only by users with the Admin role.
    /// </summary>
    /// <returns>A test message confirming admin access.</returns>
    /// <response code="200">Admin has access.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have the required role.</response>
    [HttpGet("admin")]
    [Authorize(Roles = Roles.Admin)]
    public IActionResult AdminEndpoint()
    {
        return Ok(new { message = "This endpoint is only accessible by Admins", user = User.Identity?.Name });
    }

    /// <summary>
    /// Test endpoint accessible by users with either User or Admin role.
    /// </summary>
    /// <returns>A test message with user information and roles.</returns>
    /// <response code="200">User has access.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have the required role.</response>
    [HttpGet("user-or-admin")]
    [Authorize(Roles = $"{Roles.User},{Roles.Admin}")]
    public IActionResult UserOrAdminEndpoint()
    {
        var roles = User.Claims
            .Where(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
            .Select(c => c.Value)
            .ToList();

        return Ok(new 
        { 
            message = "This endpoint is accessible by Users or Admins", 
            user = User.Identity?.Name,
            roles = roles
        });
    }
}

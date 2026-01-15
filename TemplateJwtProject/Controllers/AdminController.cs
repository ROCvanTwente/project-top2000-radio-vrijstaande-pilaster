using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TemplateJwtProject.Constants;
using TemplateJwtProject.Models;
using TemplateJwtProject.Models.DTOs;

namespace TemplateJwtProject.Controllers;

/// <summary>
/// Controller for administrative operations including user management and role assignment.
/// Requires Admin role for all endpoints.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Admin)]
public class AdminController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        UserManager<ApplicationUser> userManager,
        ILogger<AdminController> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    /// <summary>
    /// Assigns a role to a user.
    /// </summary>
    /// <param name="model">The user email and role to assign.</param>
    /// <returns>Returns the updated user information with roles.</returns>
    /// <response code="200">Role assigned successfully.</response>
    /// <response code="400">If the role is invalid or user already has the role.</response>
    /// <response code="404">If the user is not found.</response>
    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        // Valideer of de rol bestaat
        if (model.Role != Roles.Admin && model.Role != Roles.User)
        {
            return BadRequest(new { message = $"Invalid role. Valid roles are: {Roles.Admin}, {Roles.User}" });
        }

        // Check of gebruiker al deze rol heeft
        if (await _userManager.IsInRoleAsync(user, model.Role))
        {
            return BadRequest(new { message = $"User already has the {model.Role} role" });
        }

        var result = await _userManager.AddToRoleAsync(user, model.Role);
        
        if (!result.Succeeded)
        {
            return BadRequest(new { message = "Failed to assign role", errors = result.Errors });
        }

        _logger.LogInformation("Admin assigned role {Role} to user {Email}", model.Role, model.Email);

        var roles = await _userManager.GetRolesAsync(user);
        
        return Ok(new 
        { 
            message = $"Role {model.Role} assigned successfully",
            email = user.Email,
            roles = roles
        });
    }

    /// <summary>
    /// Removes a role from a user.
    /// </summary>
    /// <param name="model">The user email and role to remove.</param>
    /// <returns>Returns the updated user information with roles.</returns>
    /// <response code="200">Role removed successfully.</response>
    /// <response code="400">If the user does not have the specified role.</response>
    /// <response code="404">If the user is not found.</response>
    [HttpPost("remove-role")]
    public async Task<IActionResult> RemoveRole([FromBody] AssignRoleDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        if (!await _userManager.IsInRoleAsync(user, model.Role))
        {
            return BadRequest(new { message = $"User does not have the {model.Role} role" });
        }

        var result = await _userManager.RemoveFromRoleAsync(user, model.Role);
        
        if (!result.Succeeded)
        {
            return BadRequest(new { message = "Failed to remove role", errors = result.Errors });
        }

        _logger.LogInformation("Admin removed role {Role} from user {Email}", model.Role, model.Email);

        var roles = await _userManager.GetRolesAsync(user);
        
        return Ok(new 
        { 
            message = $"Role {model.Role} removed successfully",
            email = user.Email,
            roles = roles
        });
    }

    /// <summary>
    /// Retrieves a list of all users with their assigned roles.
    /// </summary>
    /// <returns>Returns a list of users with their email, username, and roles.</returns>
    /// <response code="200">Returns the list of all users.</response>
    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userManager.Users.ToListAsync();
        
        var userList = new List<object>();
        
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userList.Add(new
            {
                id = user.Id,
                email = user.Email,
                userName = user.UserName,
                roles = roles
            });
        }

        return Ok(userList);
    }
}

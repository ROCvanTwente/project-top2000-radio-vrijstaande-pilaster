using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TemplateJwtProject.Constants;
using TemplateJwtProject.Models;
using TemplateJwtProject.Models.DTOs;
using System.Security.Claims;

namespace TemplateJwtProject.Controllers;

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

    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(currentUserId))
        {
            return Unauthorized();
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        if (user.Id == currentUserId)
        {
            return BadRequest(new
            {
                message = "You cannot change roles on your own account."
            });
        }

        if (model.Role != Roles.Admin && model.Role != Roles.User)
        {
            return BadRequest(new
            {
                message = $"Invalid role. Valid roles are: {Roles.Admin}, {Roles.User}"
            });
        }

        if (await _userManager.IsInRoleAsync(user, model.Role))
        {
            return BadRequest(new
            {
                message = $"User already has the {model.Role} role"
            });
        }

        var result = await _userManager.AddToRoleAsync(user, model.Role);

        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                message = "Failed to assign role",
                errors = result.Errors
            });
        }

        var roles = await _userManager.GetRolesAsync(user);

        return Ok(new
        {
            message = $"Role {model.Role} assigned successfully",
            email = user.Email,
            roles
        });
    }


    [HttpPost("remove-role")]
    public async Task<IActionResult> RemoveRole([FromBody] AssignRoleDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var claims = User.Claims.Select(c => new { c.Type, c.Value });

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(currentUserId))
        {
            return Unauthorized();
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        if (user.Id == currentUserId)
        {
            return BadRequest(new
            {
                message = "You cannot change roles on your own account."
            });
        }

        if (!await _userManager.IsInRoleAsync(user, model.Role))
        {
            return BadRequest(new
            {
                message = $"User does not have the {model.Role} role"
            });
        }

        var result = await _userManager.RemoveFromRoleAsync(user, model.Role);

        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                message = "Failed to remove role",
                errors = result.Errors
            });
        }

        var roles = await _userManager.GetRolesAsync(user);

        return Ok(new
        {
            message = $"Role {model.Role} removed successfully",
            email = user.Email,
            roles
        });
    }


    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = _userManager.Users.ToList();
        
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

    [HttpDelete("delete-user/{userId}")]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(currentUserId))
            return Unauthorized();

        if (userId == currentUserId)
        {
            return BadRequest(new
            {
                message = "You cannot delete your own account."
            });
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                message = "Failed to delete user",
                errors = result.Errors
            });
        }


        return Ok(new
        {
            message = "User deleted successfully",
            userId
        });
    }

}

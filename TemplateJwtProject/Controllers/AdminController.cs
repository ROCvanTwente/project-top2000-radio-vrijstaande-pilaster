using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TemplateJwtProject.Constants;
using TemplateJwtProject.Models;
using TemplateJwtProject.Models.DTOs;
using System.Security.Claims;
using TemplateJwtProject.Data;

namespace TemplateJwtProject.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Admin)]
public class AdminController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<AdminController> _logger;
    private readonly AppDbContext _context;

    public AdminController(
        UserManager<ApplicationUser> userManager,
        AppDbContext context,
        ILogger<AdminController> logger)
    {
        _userManager = userManager;
        _logger = logger;
        _context = context;
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
            return NotFound(new { message = "Gebruiker niet gevonden" });
        }

        if (user.Id == currentUserId)
        {
            return BadRequest(new
            {
                message = "Rollen van het huidig ingelogde account mogen niet worden aangepast."
            });
        }

        if (model.Role != Roles.Admin && model.Role != Roles.User)
        {
            return BadRequest(new
            {
                message = $"Ongeldige rol, Geldige rollen zijn: {Roles.Admin}, {Roles.User}"
            });
        }

        if (await _userManager.IsInRoleAsync(user, model.Role))
        {
            return BadRequest(new
            {
                message = $"Gebruiker heeft de {model.Role} rol al"
            });
        }

        var result = await _userManager.AddToRoleAsync(user, model.Role);

        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                message = "Toevoegen van rol mislukt",
                errors = result.Errors
            });
        }

        var roles = await _userManager.GetRolesAsync(user);

        return Ok(new
        {
            message = $"{model.Role} rol succesvol toegevoegd",
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
            return NotFound(new { message = "Gebruiker niet gevonden" });
        }

        if (user.Id == currentUserId)
        {
            return BadRequest(new
            {
                message = "Rollen van het huidig ingelogde account mogen niet worden aangepast."
            });
        }

        if (!await _userManager.IsInRoleAsync(user, model.Role))
        {
            return BadRequest(new
            {
                message = $"Gebruiker heeft de {model.Role} rol niet."
            });
        }

        var result = await _userManager.RemoveFromRoleAsync(user, model.Role);

        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                message = "Mislukt om rol te verwijderen",
                errors = result.Errors
            });
        }

        var roles = await _userManager.GetRolesAsync(user);

        return Ok(new
        {
            message = $"{model.Role} rol succesvol verwijderd",
            email = user.Email,
            roles
        });
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
                message = "Je kunt je eigen account niet verwijderen."
            });
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound(new { message = "Gebruiker niet gevonden" });
        }

        var playlists = _context.UserPlaylists.Where(up => up.UserId == userId);
        _context.UserPlaylists.RemoveRange(playlists);

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                message = "Verwijderen van gebruiker mislukt",
                errors = result.Errors
            });
        }

        return Ok(new
        {
            message = "Gebruiker succesvol verwijderd.",
            userId
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
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TemplateJwtProject.Constants;
using TemplateJwtProject.Models;
using TemplateJwtProject.Models.DTOs;
using TemplateJwtProject.Services;

namespace TemplateJwtProject.Controllers;

/// <summary>
/// Controller for authentication operations including registration, login, and token management.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        IJwtService jwtService,
        IRefreshTokenService refreshTokenService,
        ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _jwtService = jwtService;
        _refreshTokenService = refreshTokenService;
        _logger = logger;
    }

    /// <summary>
    /// Registers a new user account with the User role.
    /// </summary>
    /// <param name="model">The registration details including email and password.</param>
    /// <returns>Returns authentication tokens and user details on successful registration.</returns>
    /// <response code="200">Returns the newly created user's authentication tokens.</response>
    /// <response code="400">If the registration data is invalid or the email is already registered.</response>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return BadRequest(ModelState);
        }

        // Wijs standaard de "User" rol toe
        await _userManager.AddToRoleAsync(user, Roles.User);

        _logger.LogInformation("User {Email} created successfully with role {Role}", model.Email, Roles.User);

        var token = await _jwtService.GenerateTokenAsync(user);
        var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user.Id);
        var roles = await _userManager.GetRolesAsync(user);
        
        return Ok(new AuthResponseDto
        {
            Token = token,
            RefreshToken = refreshToken.Token,
            Email = user.Email ?? string.Empty,
            Roles = roles.ToList(),
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        });
    }

    /// <summary>
    /// Authenticates a user and returns JWT tokens.
    /// </summary>
    /// <param name="model">The login credentials including email and password.</param>
    /// <returns>Returns authentication tokens and user details on successful login.</returns>
    /// <response code="200">Returns the user's authentication tokens.</response>
    /// <response code="401">If the credentials are invalid.</response>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
        
        if (!result.Succeeded)
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

        var token = await _jwtService.GenerateTokenAsync(user);
        var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user.Id);
        var roles = await _userManager.GetRolesAsync(user);

        _logger.LogInformation("User {Email} logged in successfully with roles: {Roles}", model.Email, string.Join(", ", roles));

        return Ok(new AuthResponseDto
        {
            Token = token,
            RefreshToken = refreshToken.Token,
            Email = user.Email ?? string.Empty,
            Roles = roles.ToList(),
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        });
    }

    /// <summary>
    /// Refreshes an expired access token using a valid refresh token.
    /// </summary>
    /// <param name="model">The refresh token to validate.</param>
    /// <returns>Returns new authentication tokens if the refresh token is valid.</returns>
    /// <response code="200">Returns new authentication tokens.</response>
    /// <response code="401">If the refresh token is invalid or expired.</response>
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var refreshToken = await _refreshTokenService.ValidateRefreshTokenAsync(model.RefreshToken);

        if (refreshToken == null)
        {
            return Unauthorized(new { message = "Invalid or expired refresh token" });
        }

        var user = refreshToken.User;
        
        // Revoke het oude refresh token
        await _refreshTokenService.RevokeRefreshTokenAsync(
            refreshToken.Token, 
            "Replaced by new token"
        );

        // Genereer nieuwe tokens
        var newAccessToken = await _jwtService.GenerateTokenAsync(user);
        var newRefreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user.Id);
        var roles = await _userManager.GetRolesAsync(user);

        _logger.LogInformation("Refresh token used for user {Email}", user.Email);

        return Ok(new AuthResponseDto
        {
            Token = newAccessToken,
            RefreshToken = newRefreshToken.Token,
            Email = user.Email ?? string.Empty,
            Roles = roles.ToList(),
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        });
    }

    /// <summary>
    /// Revokes a specific refresh token, preventing it from being used again.
    /// </summary>
    /// <param name="model">The refresh token to revoke.</param>
    /// <returns>Returns a success message if the token was revoked.</returns>
    /// <response code="200">Token revoked successfully.</response>
    /// <response code="401">If the user is not authenticated.</response>
    [HttpPost("revoke-token")]
    [Authorize]
    public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _refreshTokenService.RevokeRefreshTokenAsync(model.RefreshToken, "Revoked by user");

        _logger.LogInformation("Refresh token revoked");

        return Ok(new { message = "Token revoked successfully" });
    }

    /// <summary>
    /// Revokes all refresh tokens for the authenticated user, effectively logging them out from all devices.
    /// </summary>
    /// <returns>Returns a success message if all tokens were revoked.</returns>
    /// <response code="200">All tokens revoked successfully.</response>
    /// <response code="401">If the user is not authenticated.</response>
    [HttpPost("logout-all")]
    [Authorize]
    public async Task<IActionResult> LogoutFromAllDevices()
    {
        var userId = _userManager.GetUserId(User);
        
        if (userId == null)
        {
            return Unauthorized();
        }

        await _refreshTokenService.RevokeAllUserRefreshTokensAsync(userId);

        _logger.LogInformation("User {UserId} logged out from all devices", userId);

        return Ok(new { message = "Logged out from all devices successfully" });
    }
}

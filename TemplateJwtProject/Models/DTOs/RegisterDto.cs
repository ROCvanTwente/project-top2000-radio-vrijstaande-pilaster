using System.ComponentModel.DataAnnotations;

namespace TemplateJwtProject.Models.DTOs;

public class RegisterDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6, ErrorMessage = "Wachtwoord moet minstens 6 karakters hebben.")]
    [MaxLength(100, ErrorMessage = "Wachtwoord mag maximaal 100 karakters hebben.")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Compare("Password", ErrorMessage = "Wachtwoorden komen niet overeen.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

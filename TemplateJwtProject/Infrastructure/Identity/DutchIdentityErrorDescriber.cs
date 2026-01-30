using Microsoft.AspNetCore.Identity;

public class DutchIdentityErrorDescriber : IdentityErrorDescriber
{
    public override IdentityError DuplicateEmail(string email)
        => new IdentityError
        {
            Code = nameof(DuplicateEmail),
            Description = $"Dit e-mailadres wordt al gebruikt."
        };

    public override IdentityError PasswordTooShort(int length)
        => new IdentityError
        {
            Code = nameof(PasswordTooShort),
            Description = $"Het wachtwoord moet minimaal {length} tekens lang zijn."
        };

    public override IdentityError PasswordRequiresDigit()
        => new IdentityError
        {
            Code = nameof(PasswordRequiresDigit),
            Description = "Het wachtwoord moet minimaal één cijfer bevatten."
        };

    public override IdentityError PasswordRequiresUpper()
        => new IdentityError
        {
            Code = nameof(PasswordRequiresUpper),
            Description = "Het wachtwoord moet minimaal één hoofdletter bevatten."
        };

    public override IdentityError PasswordRequiresNonAlphanumeric()
        => new IdentityError
        {
            Code = nameof(PasswordRequiresNonAlphanumeric),
            Description = "Het wachtwoord moet minimaal één speciaal teken bevatten."
        };
}

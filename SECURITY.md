# Security Recommendations

## ⚠️ Important Security Considerations

### 1. JWT Secret Key
**Current Issue**: The JWT secret key is stored in plain text in `appsettings.json`.

**Recommendation**: 
- For production, store the JWT secret key in environment variables or Azure Key Vault
- Never commit production secrets to the repository
- Use a strong, randomly generated secret key (at least 32 characters)
- Rotate keys periodically

**How to fix**:
```bash
# Set environment variable
export JwtSettings__SecretKey="YourProductionSecretKeyHere"
```

Or use User Secrets for development:
```bash
dotnet user-secrets set "JwtSettings:SecretKey" "YourDevSecretKeyHere"
```

### 2. Database Credentials
**Current Issue**: Production database credentials are visible in `appsettings.Production.json`.

**Recommendation**:
- Remove hardcoded database credentials from the repository
- Use connection string from environment variables or secure configuration providers
- Consider using Managed Identity for Azure SQL connections

### 3. Admin User Creation
**Current Issue**: Admin user credentials are no longer hardcoded (fixed).

**Best Practice**:
- Set admin credentials via environment variables:
  ```bash
  export AdminUser__Email="admin@yourdomain.com"
  export AdminUser__Password="YourSecurePassword123!"
  ```
- Or leave `AdminUser:Password` empty in production and create admin users manually

### 4. CORS Configuration
**Current Issue**: CORS origins are hardcoded.

**Recommendation**:
- For production, ensure only trusted origins are allowed
- Consider using environment variables for allowed origins
- Review and update CORS settings regularly

### 5. Password Policy
**Current Status**: Good - Password requirements are enforced:
- RequireDigit: true
- RequireLowercase: true
- RequireUppercase: true
- RequiredLength: 6
- RequireNonAlphanumeric: false

**Recommendation**: Consider increasing `RequiredLength` to 8 and enabling `RequireNonAlphanumeric`.

### 6. HTTPS
**Current Status**: HTTPS redirection is enabled in Program.cs.

**Recommendation**: Ensure HTTPS is enforced in production and valid SSL certificates are used.

### 7. Logging
**Current Status**: Basic logging is configured.

**Recommendation**:
- Never log sensitive information (passwords, tokens, personal data)
- Consider using structured logging with Serilog or Application Insights
- Implement log monitoring and alerting for security events

## Production Deployment Checklist

Before deploying to production, ensure:

- [ ] JWT secret key is stored securely (not in appsettings.json)
- [ ] Database credentials are not hardcoded
- [ ] Admin user password is set via secure configuration
- [ ] CORS origins are restricted to production domains only
- [ ] HTTPS is enforced
- [ ] Appropriate password policies are configured
- [ ] Logging is configured for security monitoring
- [ ] Error messages don't expose sensitive information
- [ ] Database migrations are up to date
- [ ] All dependencies are updated to latest secure versions

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Task_Management_App.DTOs.Auth;
using Task_Management_App.Repository.RepositoryInterface;
using TaskManagement.Api.Models;

namespace TaskManagement.API.Services;

public class AuthService :IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IPasswordHasher<Users> _passwordHasher;
    private readonly IConfiguration _configuration;

    public AuthService(
        IAuthRepository authRepository,
        IPasswordHasher<Users> passwordHasher,
        IConfiguration configuration)
    {
        _authRepository = authRepository;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
        
    }
    public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        // Find user
        var user = await _authRepository.GetUserByEmailAsync(email);

        if (user == null)
            return null;

        // Verify password
        var result = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.Password
        );

        if (result == PasswordVerificationResult.Failed)
            return null;

        // Generate JWT
        var token = GenerateJwtToken(user);

        return new AuthResponseDto
        {
            Token = token,
            User = new UserResponseDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email
            }
        };
    }
    public async Task<AuthResponseDto?> RegisterAsync(RegisterRequestDto request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        // Check whether the email already exists
        var existingUser = await _authRepository.GetUserByEmailAsync(email);

        if (existingUser != null)
            return null;

        // Create user
        var user = new Users
        {
            FullName = request.FullName.Trim(),
            Email = email,
            CreatedAt = DateTime.UtcNow
        };

        // Hash password
        user.PasswordHash = _passwordHasher.HashPassword(
            user,
            request.Password
        );

        // Save user to database
        await _authRepository.AddUserAsync(user);

        // Generate JWT immediately after registration
        var token = GenerateJwtToken(user);

        // Return token and user details
        return new AuthResponseDto
        {
            Token = token,
            User = new UserResponseDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email
            }
        };
    }
    private string GenerateJwtToken(Users user)
    {
        var key = _configuration["Jwt:Key"]
                  ?? throw new InvalidOperationException("JWT key is missing.");

        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];

        var expiryMinutes = int.Parse(
            _configuration["Jwt:ExpiryMinutes"] ?? "60"
        );

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key)
        );

        var credentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
}
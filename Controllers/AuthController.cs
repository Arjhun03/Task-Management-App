using Microsoft.AspNetCore.Mvc;
using Task_Management_App.DTOs.Auth;

namespace TaskManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // REGISTER
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterRequestDto request)
    {
        var result = await _authService.RegisterAsync(request);

        if (result == null)
        {
            return Conflict(new
            {
                message = "Email is already registered."
            });
        }

        return StatusCode(201, new
        {
            message = "Registration successful.",
            token = result.Token,
            user = result.User
        });
    }

    // LOGIN
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginRequestDto request)
    {
        var result = await _authService.LoginAsync(request);

        if (result == null)
        {
            return Unauthorized(new
            {
                message = "Invalid email or password."
            });
        }

        return Ok(new
        {
            message = "Login successful.",
            token = result.Token,
            user = result.User
        });
    }
}
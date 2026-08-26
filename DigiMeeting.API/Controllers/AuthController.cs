using DigiMeeting.API.Interfaces;
using DigiMeeting.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DigiMeeting.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public AuthController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpPost("login")]
    [Authorize]  // User has Auth0 JWT token
    public async Task<IActionResult> Login([FromBody] LoginRequest? request)
    {
        // Auth0 access tokens use "sub" for the user id. Email may not be present
        // in API access tokens, so the SPA also sends the Auth0 profile email.
        var auth0Id = User.FindFirst("sub")?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = request?.Email
            ?? User.FindFirst("email")?.Value
            ?? User.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrWhiteSpace(auth0Id))
            return Unauthorized(new { message = "Auth0 ID not found in token" });

        if (string.IsNullOrWhiteSpace(email))
            return BadRequest(new { message = "Email not found in Auth0 profile" });

        var name = request?.Name
            ?? User.FindFirst("name")?.Value
            ?? User.FindFirst(ClaimTypes.Name)?.Value
            ?? email;

        var user = await _unitOfWork.Users.GetByAuth0IdAsync(auth0Id);
        var isNewUser = false;

        if (user == null)
        {
            user = await _unitOfWork.Users.GetByEmailAsync(email);
        }

        if (user == null)
        {
            user = new User
            {
                Auth0Id = auth0Id,
                Email = email,
                Name = name,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
                CreatedBy = email
            };

            await _unitOfWork.Users.AddAsync(user);
            isNewUser = true;
        }
        else
        {
            var shouldUpdateUser = false;

            if (string.IsNullOrWhiteSpace(user.Auth0Id))
            {
                user.Auth0Id = auth0Id;
                shouldUpdateUser = true;
            }

            if (!string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase))
            {
                user.Email = email;
                shouldUpdateUser = true;
            }

            if (!string.IsNullOrWhiteSpace(name) && user.Name != name)
            {
                user.Name = name;
                shouldUpdateUser = true;
            }

            if (shouldUpdateUser)
            {
                user.UpdatedBy = email;
                user.UpdatedOn = DateTime.UtcNow;
                await _unitOfWork.Users.UpdateAsync(user);
            }
        }
        return Ok(new
        {
            message = isNewUser ? "User created and logged in successfully" : "Login successful",
            userId = user.Id,
            auth0Id = user.Auth0Id,
            email = user.Email,
            name = user.Name
        });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Name))
        {
            return BadRequest(new { message = "Email and Name are required" });
        }

        // Check if user already exists by email
        var existingUser = await _unitOfWork.Users.GetByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return BadRequest(new { message = "User with this email already exists" });
        }

        // Create new user (Auth0Id will be set later after Auth0 authentication)
        var newUser = new User
        {
            Auth0Id = string.Empty,  // Will be updated after Auth0 login
            Email = request.Email,
            Name = request.Name,
            CreatedOn = DateTime.UtcNow,
            CreatedBy = request.Email
        };

        await _unitOfWork.Users.AddAsync(newUser);
        await _unitOfWork.CompleteAsync();

        return Ok(new
        {
            message = "Registration complete. Please login with Auth0.",
            userId = newUser.Id,
            email = newUser.Email,
            name = newUser.Name
        });
    }

    [HttpGet("profile")]
    [Authorize]
    public IActionResult GetProfile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var name = User.FindFirst(ClaimTypes.Name)?.Value;

        return Ok(new
        {
            userId,
            email,
            name
        });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return Ok(new { message = "Logout successful" });
    }
}

public class LoginRequest
{
    public string? Email { get; set; }
    public string? Name { get; set; }
}

public class RegisterRequest
{
    public string Email { get; set; }
    public string Name { get; set; }
}

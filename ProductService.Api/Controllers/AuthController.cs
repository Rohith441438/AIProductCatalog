using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.Api.Data;
using ProductService.Api.Dtos;
using ProductService.Api.Models;
using ProductService.Api.Services;

namespace ProductService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AppDbContext dbContext, ITokenService tokenService) : ControllerBase
{
    private readonly PasswordHasher<ApplicationUser> _passwordHasher = new();

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        var username = request.Username.Trim().ToLowerInvariant();

        if (await dbContext.Users.AnyAsync(x => x.Username == username))
        {
            return Conflict("Username already exists.");
        }

        var user = new ApplicationUser
        {
            Username = username
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        return Ok(new AuthResponse
        {
            Username = user.Username,
            Token = tokenService.CreateToken(user)
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var username = request.Username.Trim().ToLowerInvariant();
        var user = await dbContext.Users.SingleOrDefaultAsync(x => x.Username == username);

        if (user is null)
        {
            return Unauthorized("Invalid username or password.");
        }

        var verification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verification == PasswordVerificationResult.Failed)
        {
            return Unauthorized("Invalid username or password.");
        }

        return Ok(new AuthResponse
        {
            Username = user.Username,
            Token = tokenService.CreateToken(user)
        });
    }
}

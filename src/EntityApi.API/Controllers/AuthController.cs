using EntityApi.Core.Entities;
using EntityApi.API.Models;
using EntityApi.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EntityApi.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(RegisterDto request)
    {
        var user = await authService.RegisterAsync(request);
        if (user is null)
        {
            return BadRequest("Username already exists");
        }

        return Ok(user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<TokenResponseDto>> Login(UserDto request)
    {
        var result = await authService.LoginAsync(request);

        if (result is null)
        {
            return BadRequest("Invalid username or password");
        }

        return Ok(result);
    }

    [Authorize]
    [HttpGet("auth-only")]
    public IActionResult AuthenticatedOnlyEndpoint()
    {
        return Ok("You are authenticated");
    }

    [Authorize(Roles = "Candidate")]
    [HttpGet("candidate-only")]
    public IActionResult CandidateOnlyEndpoint()
    {
        return Ok("You are a candidate");
    }

    [Authorize(Roles = "Company")]
    [HttpGet("company-only")]
    public IActionResult CompanyOnlyEndpoint()
    {
        return Ok("You are a company");
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto request)
    {
        var result = await authService.RefreshTokensAsync(request);
        if (result is null || result.AccessToken is null || result.RefreshToken is null)
        {
            return Unauthorized("Invalid refresh token");
        }

        return Ok(result);
    }
}

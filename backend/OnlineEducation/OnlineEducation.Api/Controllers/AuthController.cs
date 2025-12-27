using Microsoft.AspNetCore.Mvc;
using OnlineEducation.Application.DTOs;
using OnlineEducation.Application.Interfaces;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthQuery _authQuery;
    private readonly IJwtTokenService _jwtService;

    public AuthController(IAuthQuery authQuery, IJwtTokenService jwtService)
    {
        _authQuery = authQuery;
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var user = await _authQuery.ValidateUserAsync(
            request.Email, request.Password);

        if (user == null)
            return Unauthorized("Email veya şifre hatalı");

        var token = _jwtService.GenerateToken(user);

        return Ok(new { token });
    }
}

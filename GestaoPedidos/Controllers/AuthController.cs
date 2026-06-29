using GestaoPedidos.Authentication;
using GestaoPedidos.Authentication.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestaoPedidos.API.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IJwtTokenGenerator _tokenGenerator;

    public AuthController(IJwtTokenGenerator tokenGenerator)
    {
        _tokenGenerator = tokenGenerator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _tokenGenerator.GenerateTokenAsync(request, cancellationToken);

        if (result.IsAuthenticated)
        {
            return Ok(result);
        }

        return Unauthorized(new { message = "Invalid credentials" });
    }
}
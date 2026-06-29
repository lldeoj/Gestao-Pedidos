using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GestaoPedidos.Authentication.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace GestaoPedidos.Authentication;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<TokenResponse> GenerateTokenAsync(LoginRequest email, CancellationToken cancellationToken)
    {
        //Check if the email is valid (you can add more complex validation if needed)
        if(await IsValidEmail(email, cancellationToken))
        {
            var token = GenerateJwtToken(email.Email, cancellationToken);
            return new TokenResponse
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddHours(8),
                IsAuthenticated = true
            };
        }
        else
        {
            return new TokenResponse
            {
                Token = null,
                Expiration = DateTime.MinValue,
                IsAuthenticated = false
            };
        }
    }
    
    private async Task<bool> IsValidEmail(LoginRequest login, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(login.Email) || string.IsNullOrWhiteSpace(login.Password))
            return false;

        if (login.Email == "dev@martech.com" && login.Password == "Senha@123")
            return true;

        return false;
    }


    private string GenerateJwtToken(string email, CancellationToken cancellationToken)
    { 
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "SuperSecretKeyForJWT@2024!@#$%123456"));
        
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var claims = new[]
        {
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "GestaoPedidos.Service",
            audience: _configuration["Jwt:Audience"] ?? "GestaoPedidos.Service",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
using GestaoPedidos.Authentication;
using GestaoPedidos.Authentication.Models;
using Microsoft.Extensions.Configuration;
using Moq;

namespace GestaoPedidos.Test
{
    public class JwtTokenGeneratorTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly JwtTokenGenerator _tokenGenerator;

        public JwtTokenGeneratorTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();

            // Setup default JWT configuration
            _mockConfiguration
                .Setup(c => c["Jwt:Key"])
                .Returns("SuperSecretKeyForJWT@2024!@#$%123456");
            _mockConfiguration
                .Setup(c => c["Jwt:Issuer"])
                .Returns("GestaoPedidos.Service");
            _mockConfiguration
                .Setup(c => c["Jwt:Audience"])
                .Returns("GestaoPedidos.Service");

            _tokenGenerator = new JwtTokenGenerator(_mockConfiguration.Object);
        }

        [Fact]
        public async Task GenerateTokenAsync_WithValidCredentials_ReturnsAuthenticatedToken()
        {
            // Arrange
            var loginRequest = new LoginRequest 
            { 
                Email = "dev@martech.com", 
                Password = "Senha@123" 
            };
            var cts = new CancellationTokenSource();

            // Act
            var result = await _tokenGenerator.GenerateTokenAsync(loginRequest, cts.Token);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsAuthenticated);
            Assert.NotNull(result.Token);
            Assert.NotEqual(string.Empty, result.Token);
            Assert.NotEqual(DateTime.MinValue, result.Expiration);
        }

        [Fact]
        public async Task GenerateTokenAsync_WithInvalidCredentials_ReturnsUnAuthenticatedToken()
        {
            // Arrange
            var loginRequest = new LoginRequest 
            { 
                Email = "invalid@email.com", 
                Password = "WrongPassword" 
            };
            var cts = new CancellationTokenSource();

            // Act
            var result = await _tokenGenerator.GenerateTokenAsync(loginRequest, cts.Token);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsAuthenticated);
            Assert.Null(result.Token);
            Assert.Equal(DateTime.MinValue, result.Expiration);
        }

        [Fact]
        public async Task GenerateTokenAsync_WithNullEmail_ReturnsUnAuthenticatedToken()
        {
            // Arrange
            var loginRequest = new LoginRequest 
            { 
                Email = null, 
                Password = "Senha@123" 
            };
            var cts = new CancellationTokenSource();

            // Act
            var result = await _tokenGenerator.GenerateTokenAsync(loginRequest, cts.Token);

            // Assert
            Assert.False(result.IsAuthenticated);
            Assert.Null(result.Token);
        }

        [Fact]
        public async Task GenerateTokenAsync_WithEmptyPassword_ReturnsUnAuthenticatedToken()
        {
            // Arrange
            var loginRequest = new LoginRequest 
            { 
                Email = "dev@martech.com", 
                Password = "" 
            };
            var cts = new CancellationTokenSource();

            // Act
            var result = await _tokenGenerator.GenerateTokenAsync(loginRequest, cts.Token);

            // Assert
            Assert.False(result.IsAuthenticated);
            Assert.Null(result.Token);
        }

        [Fact]
        public async Task GenerateTokenAsync_ValidToken_ContainsEmailClaim()
        {
            // Arrange
            var loginRequest = new LoginRequest 
            { 
                Email = "dev@martech.com", 
                Password = "Senha@123" 
            };
            var cts = new CancellationTokenSource();

            // Act
            var result = await _tokenGenerator.GenerateTokenAsync(loginRequest, cts.Token);

            // Assert
            Assert.NotNull(result.Token);

            // Decode JWT to verify claims (basic check)
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(result.Token);

            // ClaimTypes.Email resolves to "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"
            var emailClaim = token.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email);
            Assert.NotNull(emailClaim);
            Assert.Equal("dev@martech.com", emailClaim.Value);
        }

        [Fact]
        public async Task GenerateTokenAsync_ValidToken_HasExpirationInFuture()
        {
            // Arrange
            var loginRequest = new LoginRequest 
            { 
                Email = "dev@martech.com", 
                Password = "Senha@123" 
            };
            var cts = new CancellationTokenSource();
            var beforeGeneration = DateTime.UtcNow;

            // Act
            var result = await _tokenGenerator.GenerateTokenAsync(loginRequest, cts.Token);
            var afterGeneration = DateTime.UtcNow;

            // Assert
            Assert.True(result.Expiration > afterGeneration);
            // Token should expire in approximately 8 hours
            var expectedExpiration = afterGeneration.AddHours(8);
            Assert.True(Math.Abs((result.Expiration - expectedExpiration).TotalMinutes) < 1);
        }
    }
}


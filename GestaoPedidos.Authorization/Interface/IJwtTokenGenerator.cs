using GestaoPedidos.Authentication.Models;

namespace GestaoPedidos.Authentication;

public interface IJwtTokenGenerator
{
    Task<TokenResponse> GenerateTokenAsync(LoginRequest email, CancellationToken cancellationToken);
}
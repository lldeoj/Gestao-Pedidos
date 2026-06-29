namespace GestaoPedidos.Authentication.Models;

public class TokenResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
    public bool IsAuthenticated { get; set; }
}
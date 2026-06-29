using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

namespace GestaoPedidos.Authentication
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAuthenticationJwt(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });

            // JWT Authentication
            var jwtKey = configuration["Jwt:Key"] ?? "SuperSecretKeyForJWT@2024!@#$%123456";
            var jwtIssuer = configuration["Jwt:Issuer"] ?? "GestaoPedidos.Service";
            var jwtAudience = configuration["Jwt:Audience"] ?? "GestaoPedidos.Service";

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = true;
                    options.SaveToken = true;
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            // Log if Authorization header present
                            var auth = context.Request.Headers["Authorization"].ToString();
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = context =>
                        {

                            if (context.Exception is SecurityTokenExpiredException)
                                Console.WriteLine("[Jwt] Token expirado!");
                            else if (context.Exception is SecurityTokenInvalidSignatureException)
                                Console.WriteLine("[Jwt] Assinatura do token inválida!");
                            else if (context.Exception is SecurityTokenInvalidIssuerException)
                                Console.WriteLine("[Jwt] Issuer inválido!");
                            else if (context.Exception is SecurityTokenInvalidAudienceException)
                                Console.WriteLine("[Jwt] Audience inválido!");

                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            Console.WriteLine($"[Jwt] ✅ Token validated. Claims: {string.Join(',', context.Principal?.Claims?.Select(c => c.Type + ":" + c.Value) ?? Array.Empty<string>())}");
                            return Task.CompletedTask;
                        }
                    };

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtIssuer,
                        ValidAudience = jwtAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                    };
                });

            return services;
        }

        public static void AddAuthenticationJwt(this WebApplication app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }
    }
}

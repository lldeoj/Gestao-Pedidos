using Microsoft.OpenApi;
using GestaoPedidos.Authentication;
using GestaoPedidos.Service;
using Swashbuckle.AspNetCore.SwaggerUI;
using GestaoPedidos.Service.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "GestaoPedidos API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT no formato: Bearer {seu token}"
    });

    c.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer"),
            new List<string>()
        }
    });

});

builder.Services.AddAuthenticationJwt(builder.Configuration);
builder.Services.AddOrder(builder.Configuration);

var app = builder.Build();

// Apply migrations and create database if needed
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
    Console.WriteLine("[DB] Database initialized successfully");
}

//if (app.Environment.IsDevelopment())
//{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "GestaoPedidos API v1");
    });
//}

app.AddAuthenticationJwt();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
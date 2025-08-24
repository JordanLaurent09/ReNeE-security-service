using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.Json;
using security_service.Database.Context;
using security_service.Database.Entities;
using security_service.Database.Repositories;
using security_service.Database.Repositories.Interfaces;
using security_service.Resources.RefreshSessions;
using security_service.Resources.RefreshSessions.Interfaces;
using security_service.Services;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(@"Host=postgresDb-security;Username=maykl;Password=sandman;Database=authSecure_db");
});

builder.Services.AddAuthentication("Bearer").AddJwtBearer(
    options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "example-app",
            ValidAudience = "example-app-users",
            IssuerSigningKey = new
    SymmetricSecurityKey(Encoding.UTF8.GetBytes("super_secret_key_123456789111 2131415")
    )};
});

builder.Services.AddAuthorization();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

    });
builder.Services.AddHttpClient();
builder.Services.AddMvc();

builder.Services.AddScoped<IRepository<RefreshToken>, RefreshTokenRepository>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddTransient<ITempDataDictionaryFactory, TempDataDictionaryFactory>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<AuthService>();


WebApplication app = builder.Build();

app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World!");

app.Run();

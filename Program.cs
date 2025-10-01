using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
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
using System.Reflection;
using System.Reflection.PortableExecutable;
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
    SymmetricSecurityKey(Encoding.UTF8.GetBytes("super_secret_key_1234567891112131415")
    )};
});

builder.Services.AddAuthorization();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    // Тайм-аут неактивности сессии 
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    // Куки доступны только через HTTP 
    options.Cookie.HttpOnly = true;
    // Куки важны для работы приложения 
    options.Cookie.IsEssential = true;
});

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // Запрашивать согласие пользователя на использование куки 
    options.CheckConsentNeeded = context => true;
    // Политика SameSite для защиты куки 
    options.MinimumSameSitePolicy = SameSiteMode.Strict;
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

    });
builder.Services.AddHttpClient();
builder.Services.AddMvc();

builder.Services.AddSwaggerGen(options =>
{
    string xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddScoped<IRepository<RefreshToken>, RefreshTokenRepository>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddTransient<ITempDataDictionaryFactory, TempDataDictionaryFactory>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<AuthService>();


WebApplication app = builder.Build();

app.UseSession();
app.UseCookiePolicy();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "Hello world");

app.Run();

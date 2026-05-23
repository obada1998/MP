using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Tokens;
using Platform.Api;
using Platform.Infrastructure;
using Platform.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorClient", policy =>
        policy.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? ["https://localhost:7163", "http://localhost:5163"])
            .AllowAnyHeader()
            .AllowAnyMethod());
});
builder.Services.AddInfrastructure(builder.Configuration);
var dataProtectionPath = Path.Combine(builder.Environment.ContentRootPath, "App_Data", "Keys");
Directory.CreateDirectory(dataProtectionPath);
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionPath))
    .SetApplicationName("Platform.Api");

var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "replace-this-development-secret-with-at-least-32-characters";
JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
builder.Services.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            NameClaimType = ClaimTypes.NameIdentifier,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.FromMinutes(2)
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                NormalizeJwtClaims(context.Principal?.Identity as ClaimsIdentity);
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("BlazorClient");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

if (app.Configuration.GetValue("SeedData:Enabled", app.Environment.IsDevelopment()))
{
    try
    {
        await SeedData.InitializeAsync(app.Services);
    }
    catch (Exception ex)
    {
        app.Logger.LogCritical(ex, "Database migration/seed failed. Check the DefaultConnection string and SQL Server availability.");
        Environment.ExitCode = 1;
        return;
    }
}

app.Run();

static void NormalizeJwtClaims(ClaimsIdentity? identity)
{
    if (identity is null)
    {
        return;
    }

    var subject = identity.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
        ?? identity.FindFirst("sub")?.Value
        ?? identity.FindFirst("nameid")?.Value
        ?? identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (!string.IsNullOrWhiteSpace(subject) && !identity.HasClaim(claim => claim.Type == ClaimTypes.NameIdentifier))
    {
        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, subject));
    }

    foreach (var role in identity.FindAll("role").Select(claim => claim.Value).ToArray())
    {
        if (!identity.HasClaim(ClaimTypes.Role, role))
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, role));
        }
    }
}

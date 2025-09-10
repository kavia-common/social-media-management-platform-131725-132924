using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation.Processors.Security;
using dotnet.Persistence;
using dotnet.Services;
using dotnet.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Configuration and env
var configuration = builder.Configuration;

// Add services
builder.Services.AddControllers();

// EF Core: management_database dependency via connection string
// PUBLIC_INTERFACE
// The application expects MANAGEMENT_DB_CONNECTION in environment/appsettings.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var conn = configuration.GetConnectionString("ManagementDb")
               ?? Environment.GetEnvironmentVariable("MANAGEMENT_DB_CONNECTION")
               ?? "Server=localhost;Database=management_db;User Id=sa;Password=Your_password123;TrustServerCertificate=True;";
    options.UseSqlServer(conn);
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowCredentials()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// JWT Authentication
var jwtKey = configuration["Jwt:Key"] ?? Environment.GetEnvironmentVariable("JWT_SECRET") ?? "dev-secret-please-change";
var jwtIssuer = configuration["Jwt:Issuer"] ?? "automation_backend";
var jwtAudience = configuration["Jwt:Audience"] ?? "automation_frontend";
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            ClockSkew = TimeSpan.FromMinutes(2)
        };
    });

// DI for services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISocialAccountService, SocialAccountService>();
builder.Services.AddScoped<ISchedulingService, SchedulingService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<IAutomationRuleService, AutomationRuleService>();

// OpenAPI/Swagger with security
builder.Services.AddOpenApiDocument(opt =>
{
    opt.Title = "Automation Backend API";
    opt.Description = "REST API for authentication, social integration, scheduling, analytics, and automation rules.";
    opt.Version = "v1";
    opt.AddSecurity("JWT", new OpenApiSecurityScheme
    {
        Type = OpenApiSecuritySchemeType.ApiKey,
        Name = "Authorization",
        In = OpenApiSecurityApiKeyLocation.Header,
        Description = "Type into the textbox: Bearer {your JWT token}."
    });
    opt.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
    opt.PostProcess = document =>
    {
        document.Tags = new List<OpenApiTag>
        {
            new OpenApiTag { Name = "Auth", Description = "User authentication" },
            new OpenApiTag { Name = "SocialAccounts", Description = "Connect and manage social accounts" },
            new OpenApiTag { Name = "Scheduling", Description = "Create and manage scheduled posts" },
            new OpenApiTag { Name = "Content", Description = "Retrieve scheduled and posted content" },
            new OpenApiTag { Name = "Analytics", Description = "Insights and analytics" },
            new OpenApiTag { Name = "AutomationRules", Description = "Automation rules management" }
        };
    };
});

var app = builder.Build();

app.UseCors("AllowAll");

app.UseOpenApi();
app.UseSwaggerUi(config => { config.Path = "/docs"; });

app.UseAuthentication();
app.UseAuthorization();

// Map sub -> NameIdentifier for robust user id extraction in controllers
app.UseMiddleware<dotnet.Middleware.JwtClaimMappingMiddleware>();

app.MapControllers();

// Health check endpoint
// PUBLIC_INTERFACE
app.MapGet("/", () => new { message = "Healthy" });

// PUBLIC_INTERFACE
/// <summary>
/// WebSocket usage help (placeholder): This API currently does not expose WebSocket endpoints.
/// </summary>
app.MapGet("/ws-help", () => new
{
    message = "No WebSocket endpoints are exposed in this version. All features are REST-based."
});

app.Run();
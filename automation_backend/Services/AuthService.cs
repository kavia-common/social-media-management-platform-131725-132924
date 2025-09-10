using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using dotnet.Contracts;
using dotnet.Persistence;
using dotnet.Persistence.Entities;
using dotnet.Services.Interfaces;

namespace dotnet.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            if (await _db.Users.AnyAsync(u => u.Email == request.Email))
                throw new InvalidOperationException("Email already registered.");

            var user = new User
            {
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                DisplayName = request.DisplayName
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return GenerateAuthResponse(user);
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credentials.");

            return GenerateAuthResponse(user);
        }

        public Task<User?> GetByIdAsync(Guid id)
        {
            return _db.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        private AuthResponse GenerateAuthResponse(User user)
        {
            var key = _config["Jwt:Key"] ?? Environment.GetEnvironmentVariable("JWT_SECRET") ?? "dev-secret-please-change";
            var issuer = _config["Jwt:Issuer"] ?? "automation_backend";
            var audience = _config["Jwt:Audience"] ?? "automation_frontend";

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("name", user.DisplayName ?? user.Email)
            };

            var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(issuer, audience, claims, expires: DateTime.UtcNow.AddDays(7), signingCredentials: creds);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return new AuthResponse
            {
                Token = jwt,
                Email = user.Email,
                DisplayName = user.DisplayName,
                UserId = user.Id
            };
            }
        }
}

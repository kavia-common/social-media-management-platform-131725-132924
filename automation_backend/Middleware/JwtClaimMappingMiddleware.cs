using System.Security.Claims;

namespace dotnet.Middleware
{
    /// <summary>
    /// Middleware to ensure ClaimTypes.NameIdentifier is present by mapping from "sub" if necessary.
    /// </summary>
    public class JwtClaimMappingMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtClaimMappingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var principal = context.User;
            if (principal?.Identity?.IsAuthenticated == true)
            {
                var hasNameId = principal.Claims.Any(c => c.Type == ClaimTypes.NameIdentifier);
                var subClaim = principal.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
                if (!hasNameId && !string.IsNullOrEmpty(subClaim))
                {
                    var identity = principal.Identity as ClaimsIdentity;
                    identity?.AddClaim(new Claim(ClaimTypes.NameIdentifier, subClaim));
                }
            }

            await _next(context);
        }
    }
}

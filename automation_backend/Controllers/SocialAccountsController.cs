using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using dotnet.Contracts;
using dotnet.Services.Interfaces;

namespace dotnet.Controllers
{
    [ApiController]
    [Route("api/social-accounts")]
    [Authorize]
    [OpenApiTag("SocialAccounts")]
    public class SocialAccountsController : ControllerBase
    {
        private readonly ISocialAccountService _svc;

        public SocialAccountsController(ISocialAccountService svc)
        {
            _svc = svc;
        }

        private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue("sub")!);

        // PUBLIC_INTERFACE
        /// <summary>
        /// Connects a social account to the current user.
        /// </summary>
        [HttpPost("connect")]
        [OpenApiOperation("ConnectSocialAccount", "Connect a social account (Facebook, Instagram, Twitter, YouTube).")]
        [ProducesResponseType(typeof(SocialAccountDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Connect([FromBody] ConnectAccountRequest request)
        {
            var dto = await _svc.ConnectAsync(CurrentUserId, request);
            return Ok(dto);
        }

        // PUBLIC_INTERFACE
        /// <summary>
        /// Lists social accounts for current user.
        /// </summary>
        [HttpGet]
        [OpenApiOperation("ListSocialAccounts", "List all connected social accounts.")]
        [ProducesResponseType(typeof(IEnumerable<SocialAccountDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> List()
        {
            var items = await _svc.ListAsync(CurrentUserId);
            return Ok(items);
        }

        // PUBLIC_INTERFACE
        /// <summary>
        /// Disconnect a social account.
        /// </summary>
        [HttpDelete("{accountId:guid}")]
        [OpenApiOperation("DisconnectSocialAccount", "Disconnect a social account.")]
        public async Task<IActionResult> Disconnect([FromRoute] Guid accountId)
        {
            var ok = await _svc.DisconnectAsync(CurrentUserId, accountId);
            return ok ? NoContent() : NotFound();
        }
    }
}

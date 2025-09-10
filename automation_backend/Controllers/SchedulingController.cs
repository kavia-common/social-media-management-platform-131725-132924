using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using dotnet.Contracts;
using dotnet.Services.Interfaces;

namespace dotnet.Controllers
{
    [ApiController]
    [Route("api/scheduling")]
    [Authorize]
    [OpenApiTag("Scheduling")]
    public class SchedulingController : ControllerBase
    {
        private readonly ISchedulingService _svc;
        public SchedulingController(ISchedulingService svc) { _svc = svc; }

        private Guid CurrentUserId => Guid.Parse(User.FindFirstValue("sub") ?? throw new UnauthorizedAccessException());

        // PUBLIC_INTERFACE
        /// <summary>
        /// Schedule a new post for a connected social account.
        /// </summary>
        [HttpPost]
        [OpenApiOperation("SchedulePost", "Create a scheduled post.")]
        [ProducesResponseType(typeof(ScheduledPostDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Schedule([FromBody] SchedulePostRequest request)
        {
            var dto = await _svc.ScheduleAsync(CurrentUserId, request);
            return Ok(dto);
        }

        // PUBLIC_INTERFACE
        /// <summary>
        /// List scheduled posts for the current user.
        /// </summary>
        [HttpGet]
        [OpenApiOperation("ListScheduledPosts", "List scheduled posts, optionally filtered by socialAccountId.")]
        [ProducesResponseType(typeof(IEnumerable<ScheduledPostDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> List([FromQuery] Guid? socialAccountId = null)
        {
            var items = await _svc.ListScheduledAsync(CurrentUserId, socialAccountId);
            return Ok(items);
        }

        // PUBLIC_INTERFACE
        /// <summary>
        /// Cancel a scheduled post.
        /// </summary>
        [HttpDelete("{scheduledPostId:guid}")]
        [OpenApiOperation("CancelScheduledPost", "Cancel a scheduled post by Id.")]
        public async Task<IActionResult> Cancel([FromRoute] Guid scheduledPostId)
        {
            var ok = await _svc.CancelAsync(CurrentUserId, scheduledPostId);
            return ok ? NoContent() : NotFound();
        }

        // PUBLIC_INTERFACE
        /// <summary>
        /// List published posts for the current user.
        /// </summary>
        [HttpGet("published")]
        [OpenApiOperation("ListPublishedPosts", "List posted content, optionally filtered by socialAccountId.")]
        [ProducesResponseType(typeof(IEnumerable<PublishedPostDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListPublished([FromQuery] Guid? socialAccountId = null)
        {
            var items = await _svc.ListPublishedAsync(CurrentUserId, socialAccountId);
            return Ok(items);
        }
    }
}

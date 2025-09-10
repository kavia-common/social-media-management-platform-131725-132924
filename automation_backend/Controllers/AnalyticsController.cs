using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using dotnet.Contracts;
using dotnet.Services.Interfaces;

namespace dotnet.Controllers
{
    [ApiController]
    [Route("api/analytics")]
    [Authorize]
    [OpenApiTag("Analytics")]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _svc;
        public AnalyticsController(IAnalyticsService svc) { _svc = svc; }

        private Guid CurrentUserId => Guid.Parse(User.FindFirstValue("sub") ?? throw new UnauthorizedAccessException());

        // PUBLIC_INTERFACE
        /// <summary>
        /// Get summary analytics for dashboard.
        /// </summary>
        [HttpGet("summary")]
        [OpenApiOperation("GetAnalyticsSummary", "Get summary metrics for the dashboard.")]
        [ProducesResponseType(typeof(AnalyticsSummaryDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Summary()
        {
            var summary = await _svc.GetSummaryAsync(CurrentUserId);
            return Ok(summary);
        }

        // PUBLIC_INTERFACE
        /// <summary>
        /// Get time series chart data for a metric.
        /// </summary>
        [HttpGet("timeseries")]
        [OpenApiOperation("GetAnalyticsTimeSeries", "Get chart data points for a given metric (followers, impressions, engagements, clicks).")]
        [ProducesResponseType(typeof(IEnumerable<ChartPointDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> TimeSeries([FromQuery] string metric = "impressions", [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            var series = await _svc.GetTimeSeriesAsync(CurrentUserId, metric, from, to);
            return Ok(series);
        }
    }
}

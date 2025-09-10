using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using dotnet.Contracts;
using dotnet.Services.Interfaces;

namespace dotnet.Controllers
{
    [ApiController]
    [Route("api/automation-rules")]
    [Authorize]
    [OpenApiTag("AutomationRules")]
    public class AutomationRulesController : ControllerBase
    {
        private readonly IAutomationRuleService _svc;
        public AutomationRulesController(IAutomationRuleService svc) { _svc = svc; }

        private Guid CurrentUserId => Guid.Parse(User.FindFirstValue("sub") ?? throw new UnauthorizedAccessException());

        // PUBLIC_INTERFACE
        /// <summary>
        /// Create a new automation rule.
        /// </summary>
        [HttpPost]
        [OpenApiOperation("CreateAutomationRule", "Create a new automation rule.")]
        [ProducesResponseType(typeof(AutomationRuleDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromBody] AutomationRuleRequest request)
        {
            var dto = await _svc.CreateAsync(CurrentUserId, request);
            return Ok(dto);
        }

        // PUBLIC_INTERFACE
        /// <summary>
        /// List automation rules.
        /// </summary>
        [HttpGet]
        [OpenApiOperation("ListAutomationRules", "List all automation rules for the current user.")]
        [ProducesResponseType(typeof(IEnumerable<AutomationRuleDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> List()
        {
            var items = await _svc.ListAsync(CurrentUserId);
            return Ok(items);
        }

        // PUBLIC_INTERFACE
        /// <summary>
        /// Update an automation rule.
        /// </summary>
        [HttpPut("{ruleId:guid}")]
        [OpenApiOperation("UpdateAutomationRule", "Update an existing automation rule.")]
        [ProducesResponseType(typeof(AutomationRuleDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromRoute] Guid ruleId, [FromBody] AutomationRuleRequest request)
        {
            var dto = await _svc.UpdateAsync(CurrentUserId, ruleId, request);
            return dto is not null ? Ok(dto) : NotFound();
        }

        // PUBLIC_INTERFACE
        /// <summary>
        /// Delete an automation rule.
        /// </summary>
        [HttpDelete("{ruleId:guid}")]
        [OpenApiOperation("DeleteAutomationRule", "Delete an automation rule.")]
        public async Task<IActionResult> Delete([FromRoute] Guid ruleId)
        {
            var ok = await _svc.DeleteAsync(CurrentUserId, ruleId);
            return ok ? NoContent() : NotFound();
        }

        // PUBLIC_INTERFACE
        /// <summary>
        /// Enable/disable an automation rule.
        /// </summary>
        [HttpPost("{ruleId:guid}/toggle")]
        [OpenApiOperation("ToggleAutomationRule", "Enable or disable an automation rule.")]
        public async Task<IActionResult> Toggle([FromRoute] Guid ruleId, [FromQuery] bool enabled)
        {
            var ok = await _svc.ToggleAsync(CurrentUserId, ruleId, enabled);
            return ok ? NoContent() : NotFound();
        }
    }
}

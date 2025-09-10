using Microsoft.EntityFrameworkCore;
using dotnet.Contracts;
using dotnet.Persistence;
using dotnet.Persistence.Entities;
using dotnet.Services.Interfaces;

namespace dotnet.Services
{
    public class AutomationRuleService : IAutomationRuleService
    {
        private readonly AppDbContext _db;

        public AutomationRuleService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<AutomationRuleDto> CreateAsync(Guid userId, AutomationRuleRequest request)
        {
            var rule = new AutomationRule
            {
                UserId = userId,
                Name = request.Name,
                Trigger = request.Trigger,
                Action = request.Action,
                ConditionsJson = request.ConditionsJson,
                ParametersJson = request.ParametersJson,
                Enabled = request.Enabled
            };
            _db.AutomationRules.Add(rule);
            await _db.SaveChangesAsync();

            return Map(rule);
        }

        public async Task<IEnumerable<AutomationRuleDto>> ListAsync(Guid userId)
        {
            return await _db.AutomationRules
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => Map(r))
                .ToListAsync();
        }

        public async Task<AutomationRuleDto?> UpdateAsync(Guid userId, Guid ruleId, AutomationRuleRequest request)
        {
            var rule = await _db.AutomationRules.FirstOrDefaultAsync(r => r.Id == ruleId && r.UserId == userId);
            if (rule == null) return null;
            rule.Name = request.Name;
            rule.Trigger = request.Trigger;
            rule.Action = request.Action;
            rule.ConditionsJson = request.ConditionsJson;
            rule.ParametersJson = request.ParametersJson;
            rule.Enabled = request.Enabled;
            await _db.SaveChangesAsync();
            return Map(rule);
        }

        public async Task<bool> DeleteAsync(Guid userId, Guid ruleId)
        {
            var rule = await _db.AutomationRules.FirstOrDefaultAsync(r => r.Id == ruleId && r.UserId == userId);
            if (rule == null) return false;
            _db.AutomationRules.Remove(rule);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleAsync(Guid userId, Guid ruleId, bool enabled)
        {
            var rule = await _db.AutomationRules.FirstOrDefaultAsync(r => r.Id == ruleId && r.UserId == userId);
            if (rule == null) return false;
            rule.Enabled = enabled;
            await _db.SaveChangesAsync();
            return true;
        }

        private static AutomationRuleDto Map(AutomationRule r) => new AutomationRuleDto
        {
            Id = r.Id,
            Name = r.Name,
            Trigger = r.Trigger,
            Action = r.Action,
            ConditionsJson = r.ConditionsJson,
            ParametersJson = r.ParametersJson,
            Enabled = r.Enabled,
            CreatedAt = r.CreatedAt
        };
    }
}

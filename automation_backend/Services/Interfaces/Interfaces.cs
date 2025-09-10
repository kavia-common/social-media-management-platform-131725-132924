using dotnet.Contracts;
using dotnet.Persistence.Entities;

namespace dotnet.Services.Interfaces
{
    // PUBLIC_INTERFACE
    public interface IAuthService
    {
        /// <summary>
        /// Registers a new user and returns an auth response with JWT.
        /// </summary>
        Task<AuthResponse> RegisterAsync(RegisterRequest request);

        /// <summary>
        /// Logs in an existing user and returns JWT token.
        /// </summary>
        Task<AuthResponse> LoginAsync(LoginRequest request);

        /// <summary>
        /// Gets the user by id.
        /// </summary>
        Task<User?> GetByIdAsync(Guid id);
    }

    // PUBLIC_INTERFACE
    public interface ISocialAccountService
    {
        Task<SocialAccountDto> ConnectAsync(Guid userId, ConnectAccountRequest request);
        Task<IEnumerable<SocialAccountDto>> ListAsync(Guid userId);
        Task<bool> DisconnectAsync(Guid userId, Guid accountId);
    }

    // PUBLIC_INTERFACE
    public interface ISchedulingService
    {
        Task<ScheduledPostDto> ScheduleAsync(Guid userId, SchedulePostRequest request);
        Task<IEnumerable<ScheduledPostDto>> ListScheduledAsync(Guid userId, Guid? socialAccountId = null);
        Task<bool> CancelAsync(Guid userId, Guid scheduledPostId);
        Task<IEnumerable<PublishedPostDto>> ListPublishedAsync(Guid userId, Guid? socialAccountId = null);
    }

    // PUBLIC_INTERFACE
    public interface IAnalyticsService
    {
        Task<AnalyticsSummaryDto> GetSummaryAsync(Guid userId);
        Task<IEnumerable<ChartPointDto>> GetTimeSeriesAsync(Guid userId, string metric, DateTime? from = null, DateTime? to = null);
    }

    // PUBLIC_INTERFACE
    public interface IAutomationRuleService
    {
        Task<AutomationRuleDto> CreateAsync(Guid userId, AutomationRuleRequest request);
        Task<IEnumerable<AutomationRuleDto>> ListAsync(Guid userId);
        Task<AutomationRuleDto?> UpdateAsync(Guid userId, Guid ruleId, AutomationRuleRequest request);
        Task<bool> DeleteAsync(Guid userId, Guid ruleId);
        Task<bool> ToggleAsync(Guid userId, Guid ruleId, bool enabled);
    }
}

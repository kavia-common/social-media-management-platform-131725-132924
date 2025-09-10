using dotnet.Persistence.Entities;

namespace dotnet.Contracts
{
    public class ConnectAccountRequest
    {
        public SocialProvider Provider { get; set; }
        public string ProviderUserId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }

    public class SocialAccountDto
    {
        public Guid Id { get; set; }
        public SocialProvider Provider { get; set; }
        public string ProviderUserId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public DateTime ConnectedAt { get; set; }
    }

    public class SchedulePostRequest
    {
        public Guid SocialAccountId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime ScheduledFor { get; set; }
        public string? MediaUrl { get; set; }
    }

    public class ScheduledPostDto
    {
        public Guid Id { get; set; }
        public Guid SocialAccountId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime ScheduledFor { get; set; }
        public string? MediaUrl { get; set; }
        public string Status { get; set; } = "scheduled";
    }

    public class PublishedPostDto
    {
        public Guid Id { get; set; }
        public Guid SocialAccountId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? MediaUrl { get; set; }
        public DateTime PostedAt { get; set; }
        public int Likes { get; set; }
        public int Comments { get; set; }
        public int Shares { get; set; }
        public int Views { get; set; }
    }

    public class AnalyticsSummaryDto
    {
        public int Accounts { get; set; }
        public int TotalFollowers { get; set; }
        public int TotalImpressions { get; set; }
        public int TotalEngagements { get; set; }
        public int TotalClicks { get; set; }
    }

    public class ChartPointDto
    {
        public DateTime Timestamp { get; set; }
        public int Value { get; set; }
    }

    public class AutomationRuleRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Trigger { get; set; } = "time";
        public string Action { get; set; } = "post";
        public string? ConditionsJson { get; set; }
        public string? ParametersJson { get; set; }
        public bool Enabled { get; set; } = true;
    }

    public class AutomationRuleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Trigger { get; set; } = "time";
        public string Action { get; set; } = "post";
        public string? ConditionsJson { get; set; }
        public string? ParametersJson { get; set; }
        public bool Enabled { get; set; } = true;
        public DateTime CreatedAt { get; set; }
    }
}

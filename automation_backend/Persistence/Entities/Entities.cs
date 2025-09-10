namespace dotnet.Persistence.Entities
{
    public enum SocialProvider
    {
        Facebook,
        Instagram,
        Twitter,
        YouTube
    }

    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<SocialAccount> SocialAccounts { get; set; } = new List<SocialAccount>();
        public ICollection<AutomationRule> AutomationRules { get; set; } = new List<AutomationRule>();
    }

    public class SocialAccount
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public SocialProvider Provider { get; set; }
        public string ProviderUserId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public DateTime ConnectedAt { get; set; } = DateTime.UtcNow;

        public User? User { get; set; }
        public ICollection<ScheduledPost> ScheduledPosts { get; set; } = new List<ScheduledPost>();
        public ICollection<PublishedPost> PublishedPosts { get; set; } = new List<PublishedPost>();
    }

    public class ScheduledPost
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid SocialAccountId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime ScheduledFor { get; set; }
        public string? MediaUrl { get; set; }
        public string? Status { get; set; } = "scheduled"; // scheduled, canceled, posted
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public SocialAccount? SocialAccount { get; set; }
    }

    public class PublishedPost
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid SocialAccountId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? MediaUrl { get; set; }
        public DateTime PostedAt { get; set; } = DateTime.UtcNow;
        public string ProviderPostId { get; set; } = string.Empty;
        public int Likes { get; set; }
        public int Comments { get; set; }
        public int Shares { get; set; }
        public int Views { get; set; }

        public SocialAccount? SocialAccount { get; set; }
    }

    public class AnalyticsSnapshot
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid SocialAccountId { get; set; }
        public DateTime CapturedAt { get; set; } = DateTime.UtcNow;
        public int Followers { get; set; }
        public int Impressions { get; set; }
        public int Engagements { get; set; }
        public int Clicks { get; set; }

        public SocialAccount? SocialAccount { get; set; }
    }

    public class AutomationRule
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Trigger { get; set; } = "time"; // time, event
        public string Action { get; set; } = "post"; // post, reshare, notify
        public string? ConditionsJson { get; set; } // arbitrary JSON for conditions
        public string? ParametersJson { get; set; } // arbitrary JSON for parameters
        public bool Enabled { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User? User { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;
using dotnet.Contracts;
using dotnet.Persistence;
using dotnet.Services.Interfaces;

namespace dotnet.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly AppDbContext _db;

        public AnalyticsService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<AnalyticsSummaryDto> GetSummaryAsync(Guid userId)
        {
            var accountIds = await _db.SocialAccounts.Where(a => a.UserId == userId).Select(a => a.Id).ToListAsync();
            var snapshots = await _db.AnalyticsSnapshots.Where(s => accountIds.Contains(s.SocialAccountId)).ToListAsync();

            return new AnalyticsSummaryDto
            {
                Accounts = accountIds.Count,
                TotalFollowers = snapshots.Sum(s => s.Followers),
                TotalImpressions = snapshots.Sum(s => s.Impressions),
                TotalEngagements = snapshots.Sum(s => s.Engagements),
                TotalClicks = snapshots.Sum(s => s.Clicks)
            };
        }

        public async Task<IEnumerable<ChartPointDto>> GetTimeSeriesAsync(Guid userId, string metric, DateTime? from = null, DateTime? to = null)
        {
            var accountIds = await _db.SocialAccounts.Where(a => a.UserId == userId).Select(a => a.Id).ToListAsync();
            var query = _db.AnalyticsSnapshots.Where(s => accountIds.Contains(s.SocialAccountId));
            if (from.HasValue) query = query.Where(s => s.CapturedAt >= from.Value);
            if (to.HasValue) query = query.Where(s => s.CapturedAt <= to.Value);

            var data = await query
                .GroupBy(s => new { d = new DateTime(s.CapturedAt.Year, s.CapturedAt.Month, s.CapturedAt.Day, 0, 0, 0, DateTimeKind.Utc) })
                .Select(g => new
                {
                    g.Key.d,
                    Followers = g.Sum(x => x.Followers),
                    Impressions = g.Sum(x => x.Impressions),
                    Engagements = g.Sum(x => x.Engagements),
                    Clicks = g.Sum(x => x.Clicks)
                })
                .OrderBy(x => x.d)
                .ToListAsync();

            return metric.ToLower() switch
            {
                "followers" => data.Select(x => new ChartPointDto { Timestamp = x.d, Value = x.Followers }),
                "impressions" => data.Select(x => new ChartPointDto { Timestamp = x.d, Value = x.Impressions }),
                "engagements" => data.Select(x => new ChartPointDto { Timestamp = x.d, Value = x.Engagements }),
                "clicks" => data.Select(x => new ChartPointDto { Timestamp = x.d, Value = x.Clicks }),
                _ => data.Select(x => new ChartPointDto { Timestamp = x.d, Value = x.Impressions })
            };
        }
    }
}

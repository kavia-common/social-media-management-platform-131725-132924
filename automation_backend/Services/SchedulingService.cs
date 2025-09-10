using Microsoft.EntityFrameworkCore;
using dotnet.Contracts;
using dotnet.Persistence;
using dotnet.Persistence.Entities;
using dotnet.Services.Interfaces;

namespace dotnet.Services
{
    public class SchedulingService : ISchedulingService
    {
        private readonly AppDbContext _db;

        public SchedulingService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<ScheduledPostDto> ScheduleAsync(Guid userId, SchedulePostRequest request)
        {
            var account = await _db.SocialAccounts.FirstOrDefaultAsync(a => a.Id == request.SocialAccountId && a.UserId == userId);
            if (account == null) throw new InvalidOperationException("Social account not found.");

            var post = new ScheduledPost
            {
                SocialAccountId = request.SocialAccountId,
                Content = request.Content,
                ScheduledFor = request.ScheduledFor.ToUniversalTime(),
                MediaUrl = request.MediaUrl,
                Status = "scheduled"
            };
            _db.ScheduledPosts.Add(post);
            await _db.SaveChangesAsync();

            return new ScheduledPostDto
            {
                Id = post.Id,
                SocialAccountId = post.SocialAccountId,
                Content = post.Content,
                ScheduledFor = post.ScheduledFor,
                MediaUrl = post.MediaUrl,
                Status = post.Status!
            };
        }

        public async Task<IEnumerable<ScheduledPostDto>> ListScheduledAsync(Guid userId, Guid? socialAccountId = null)
        {
            var query = _db.ScheduledPosts
                .Include(p => p.SocialAccount)
                .Where(p => p.SocialAccount!.UserId == userId);

            if (socialAccountId.HasValue)
            {
                query = query.Where(p => p.SocialAccountId == socialAccountId.Value);
            }

            return await query
                .OrderBy(p => p.ScheduledFor)
                .Select(p => new ScheduledPostDto
                {
                    Id = p.Id,
                    SocialAccountId = p.SocialAccountId,
                    Content = p.Content,
                    ScheduledFor = p.ScheduledFor,
                    MediaUrl = p.MediaUrl,
                    Status = p.Status ?? "scheduled"
                })
                .ToListAsync();
        }

        public async Task<bool> CancelAsync(Guid userId, Guid scheduledPostId)
        {
            var post = await _db.ScheduledPosts
                .Include(p => p.SocialAccount)
                .FirstOrDefaultAsync(p => p.Id == scheduledPostId && p.SocialAccount!.UserId == userId);

            if (post == null) return false;
            post.Status = "canceled";
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<PublishedPostDto>> ListPublishedAsync(Guid userId, Guid? socialAccountId = null)
        {
            var query = _db.PublishedPosts
                .Include(p => p.SocialAccount)
                .Where(p => p.SocialAccount!.UserId == userId);

            if (socialAccountId.HasValue)
            {
                query = query.Where(p => p.SocialAccountId == socialAccountId.Value);
            }

            return await query
                .OrderByDescending(p => p.PostedAt)
                .Select(p => new PublishedPostDto
                {
                    Id = p.Id,
                    SocialAccountId = p.SocialAccountId,
                    Content = p.Content,
                    MediaUrl = p.MediaUrl,
                    PostedAt = p.PostedAt,
                    Likes = p.Likes,
                    Comments = p.Comments,
                    Shares = p.Shares,
                    Views = p.Views
                }).ToListAsync();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using dotnet.Contracts;
using dotnet.Persistence;
using dotnet.Persistence.Entities;
using dotnet.Services.Interfaces;

namespace dotnet.Services
{
    public class SocialAccountService : ISocialAccountService
    {
        private readonly AppDbContext _db;

        public SocialAccountService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<SocialAccountDto> ConnectAsync(Guid userId, ConnectAccountRequest request)
        {
            var acct = new SocialAccount
            {
                UserId = userId,
                Provider = request.Provider,
                ProviderUserId = request.ProviderUserId,
                DisplayName = request.DisplayName,
                AccessToken = request.AccessToken,
                RefreshToken = request.RefreshToken,
                ExpiresAt = request.ExpiresAt
            };

            _db.SocialAccounts.Add(acct);
            await _db.SaveChangesAsync();

            return new SocialAccountDto
            {
                Id = acct.Id,
                Provider = acct.Provider,
                ProviderUserId = acct.ProviderUserId,
                DisplayName = acct.DisplayName,
                ConnectedAt = acct.ConnectedAt
            };
        }

        public async Task<IEnumerable<SocialAccountDto>> ListAsync(Guid userId)
        {
            return await _db.SocialAccounts
                .Where(a => a.UserId == userId)
                .Select(a => new SocialAccountDto
                {
                    Id = a.Id,
                    Provider = a.Provider,
                    ProviderUserId = a.ProviderUserId,
                    DisplayName = a.DisplayName,
                    ConnectedAt = a.ConnectedAt
                }).ToListAsync();
        }

        public async Task<bool> DisconnectAsync(Guid userId, Guid accountId)
        {
            var acct = await _db.SocialAccounts.FirstOrDefaultAsync(a => a.Id == accountId && a.UserId == userId);
            if (acct == null) return false;
            _db.SocialAccounts.Remove(acct);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}

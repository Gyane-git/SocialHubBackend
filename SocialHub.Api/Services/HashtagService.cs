using Microsoft.EntityFrameworkCore;
using SocialHub.Api.Common;
using SocialHub.Api.Data;
using SocialHub.Api.DTOs;
using SocialHub.Api.Interfaces;
using SocialHub.Api.Models;

namespace SocialHub.Api.Services;

public class HashtagService : IHashtagService
{
    private readonly SocialHubDbContext _context;

    public HashtagService(SocialHubDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<HashtagResponse>> GetAllAsync()
    {
        return await _context.Hashtags
            .AsNoTracking()
            .OrderBy(h => h.Name)
            .Select(h => new HashtagResponse
            {
                Id = h.Id,
                Name = h.Name,
                CreatedAt = h.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<HashtagResponse> CreateAsync(CreateHashtagRequest request)
    {
        var rawName = request.Name.Trim();
        var normalizedName = rawName.StartsWith('#') ? rawName.ToLowerInvariant() : "#" + rawName.ToLowerInvariant();

        var existing = await _context.Hashtags.FirstOrDefaultAsync(h => h.Name == normalizedName);
        if (existing != null)
        {
            return new HashtagResponse
            {
                Id = existing.Id,
                Name = existing.Name,
                CreatedAt = existing.CreatedAt
            };
        }

        var hashtag = new Hashtag
        {
            Name = normalizedName,
            CreatedAt = DateTime.UtcNow
        };

        _context.Hashtags.Add(hashtag);
        await _context.SaveChangesAsync();

        return new HashtagResponse
        {
            Id = hashtag.Id,
            Name = hashtag.Name,
            CreatedAt = hashtag.CreatedAt
        };
    }

    public async Task DeleteAsync(int id)
    {
        var hashtag = await _context.Hashtags.FindAsync(id);
        if (hashtag == null)
        {
            throw new NotFoundException($"Hashtag with ID {id} was not found.");
        }

        _context.Hashtags.Remove(hashtag);
        await _context.SaveChangesAsync();
    }
}

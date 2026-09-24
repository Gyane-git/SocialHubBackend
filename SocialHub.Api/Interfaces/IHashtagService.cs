using SocialHub.Api.DTOs;

namespace SocialHub.Api.Interfaces;

public interface IHashtagService
{
    Task<IEnumerable<HashtagResponse>> GetAllAsync();
    Task<HashtagResponse> CreateAsync(CreateHashtagRequest request);
    Task DeleteAsync(int id);
}

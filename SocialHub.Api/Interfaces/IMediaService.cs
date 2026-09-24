using SocialHub.Api.DTOs;

namespace SocialHub.Api.Interfaces;

public interface IMediaService
{
    Task<IEnumerable<MediaResponse>> GetAllAsync();
    Task<MediaResponse> GetByIdAsync(int id);
    Task<IEnumerable<MediaResponse>> GetByWorkspaceIdAsync(int workspaceId);
    Task<MediaResponse> CreateAsync(CreateMediaRequest request);
    Task<MediaResponse> UpdateAsync(int id, UpdateMediaRequest request);
    Task DeleteAsync(int id);
}

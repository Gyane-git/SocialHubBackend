using SocialHub.Api.DTOs;

namespace SocialHub.Api.Interfaces;

public interface IPostService
{
    Task<IEnumerable<PostResponse>> GetAllAsync(PostQueryParameters parameters);
    Task<PostResponse> GetByIdAsync(int id);
    Task<IEnumerable<PostResponse>> GetByWorkspaceIdAsync(int workspaceId);
    Task<PostResponse> CreateAsync(CreatePostRequest request);
    Task<PostResponse> UpdateAsync(int id, UpdatePostRequest request);
    Task DeleteAsync(int id);
    Task<PostResponse> PublishAsync(int id);
    Task<PostResponse> ScheduleAsync(int id, SchedulePostRequest request);
    Task<PostResponse> DuplicateAsync(int id);
    Task<PostResponse> RetryAsync(int id);
}

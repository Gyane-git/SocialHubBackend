using SocialHub.Api.DTOs;

namespace SocialHub.Api.Interfaces;

public interface ISocialAccountService
{
    Task<IEnumerable<SocialAccountResponse>> GetAllAsync();
    Task<SocialAccountResponse> GetByIdAsync(int id);
    Task<IEnumerable<SocialAccountResponse>> GetByWorkspaceIdAsync(int workspaceId);
    Task<SocialAccountResponse> CreateAsync(CreateSocialAccountRequest request);
    Task<SocialAccountResponse> UpdateAsync(int id, UpdateSocialAccountRequest request);
    Task DeleteAsync(int id);
    Task<SocialAccountResponse> ConnectAsync(int id);
    Task<SocialAccountResponse> DisconnectAsync(int id);
}

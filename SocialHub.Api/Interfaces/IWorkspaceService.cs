using SocialHub.Api.DTOs;

namespace SocialHub.Api.Interfaces;

public interface IWorkspaceService
{
    Task<IEnumerable<WorkspaceResponse>> GetAllAsync();
    Task<WorkspaceResponse> GetByIdAsync(int id);
    Task<WorkspaceResponse> CreateAsync(CreateWorkspaceRequest request);
    Task<WorkspaceResponse> UpdateAsync(int id, UpdateWorkspaceRequest request);
    Task DeleteAsync(int id);
}

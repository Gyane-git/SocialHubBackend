using SocialHub.Api.DTOs;

namespace SocialHub.Api.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserResponse>> GetAllAsync();
    Task<UserResponse> GetByIdAsync(int id);
    Task<UserResponse> CreateAsync(CreateUserRequest request);
    Task<UserResponse> UpdateAsync(int id, UpdateUserRequest request);
    Task DeleteAsync(int id);
}

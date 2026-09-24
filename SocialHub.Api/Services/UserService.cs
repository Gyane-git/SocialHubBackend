using Microsoft.EntityFrameworkCore;
using SocialHub.Api.Common;
using SocialHub.Api.Data;
using SocialHub.Api.DTOs;
using SocialHub.Api.Interfaces;
using SocialHub.Api.Models;

namespace SocialHub.Api.Services;

public class UserService : IUserService
{
    private readonly SocialHubDbContext _context;

    public UserService(SocialHubDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserResponse>> GetAllAsync()
    {
        return await _context.Users
            .AsNoTracking()
            .OrderByDescending(u => u.CreatedAt)
            .Select(u => MapToResponse(u))
            .ToListAsync();
    }

    public async Task<UserResponse> GetByIdAsync(int id)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            throw new NotFoundException($"User with ID {id} was not found.");
        }

        return MapToResponse(user);
    }

    public async Task<UserResponse> CreateAsync(CreateUserRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var emailExists = await _context.Users.AnyAsync(u => u.Email == email);
        if (emailExists)
        {
            throw new ConflictException($"A user with email '{email}' already exists.");
        }

        if (request.WorkspaceId.HasValue)
        {
            var workspaceExists = await _context.Workspaces.AnyAsync(w => w.Id == request.WorkspaceId.Value);
            if (!workspaceExists)
            {
                throw new NotFoundException($"Workspace with ID {request.WorkspaceId.Value} was not found.");
            }
        }

        var user = new User
        {
            FullName = request.FullName.Trim(),
            Email = email,
            AvatarUrl = request.AvatarUrl?.Trim(),
            WorkspaceId = request.WorkspaceId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return MapToResponse(user);
    }

    public async Task<UserResponse> UpdateAsync(int id, UpdateUserRequest request)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            throw new NotFoundException($"User with ID {id} was not found.");
        }

        var email = request.Email.Trim().ToLowerInvariant();
        var emailExists = await _context.Users.AnyAsync(u => u.Email == email && u.Id != id);
        if (emailExists)
        {
            throw new ConflictException($"A user with email '{email}' already exists.");
        }

        if (request.WorkspaceId.HasValue)
        {
            var workspaceExists = await _context.Workspaces.AnyAsync(w => w.Id == request.WorkspaceId.Value);
            if (!workspaceExists)
            {
                throw new NotFoundException($"Workspace with ID {request.WorkspaceId.Value} was not found.");
            }
        }

        user.FullName = request.FullName.Trim();
        user.Email = email;
        user.AvatarUrl = request.AvatarUrl?.Trim();
        user.IsActive = request.IsActive;
        user.WorkspaceId = request.WorkspaceId;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapToResponse(user);
    }

    public async Task DeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            throw new NotFoundException($"User with ID {id} was not found.");
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    private static UserResponse MapToResponse(User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            WorkspaceId = user.WorkspaceId,
            FullName = user.FullName,
            Email = user.Email,
            AvatarUrl = user.AvatarUrl,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}

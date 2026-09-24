using Microsoft.EntityFrameworkCore;
using SocialHub.Api.Common;
using SocialHub.Api.Data;
using SocialHub.Api.DTOs;
using SocialHub.Api.Interfaces;
using SocialHub.Api.Models;

namespace SocialHub.Api.Services;

public class WorkspaceService : IWorkspaceService
{
    private readonly SocialHubDbContext _context;

    public WorkspaceService(SocialHubDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<WorkspaceResponse>> GetAllAsync()
    {
        return await _context.Workspaces
            .AsNoTracking()
            .OrderByDescending(w => w.CreatedAt)
            .Select(w => MapToResponse(w))
            .ToListAsync();
    }

    public async Task<WorkspaceResponse> GetByIdAsync(int id)
    {
        var workspace = await _context.Workspaces
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == id);

        if (workspace == null)
        {
            throw new NotFoundException($"Workspace with ID {id} was not found.");
        }

        return MapToResponse(workspace);
    }

    public async Task<WorkspaceResponse> CreateAsync(CreateWorkspaceRequest request)
    {
        var slug = request.Slug.Trim().ToLowerInvariant();

        var slugExists = await _context.Workspaces.AnyAsync(w => w.Slug == slug);
        if (slugExists)
        {
            throw new ConflictException($"A workspace with slug '{slug}' already exists.");
        }

        var workspace = new Workspace
        {
            Name = request.Name.Trim(),
            Slug = slug,
            Description = request.Description?.Trim(),
            LogoUrl = request.LogoUrl?.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Workspaces.Add(workspace);
        await _context.SaveChangesAsync();

        return MapToResponse(workspace);
    }

    public async Task<WorkspaceResponse> UpdateAsync(int id, UpdateWorkspaceRequest request)
    {
        var workspace = await _context.Workspaces.FindAsync(id);
        if (workspace == null)
        {
            throw new NotFoundException($"Workspace with ID {id} was not found.");
        }

        var slug = request.Slug.Trim().ToLowerInvariant();
        var slugExists = await _context.Workspaces.AnyAsync(w => w.Slug == slug && w.Id != id);
        if (slugExists)
        {
            throw new ConflictException($"A workspace with slug '{slug}' already exists.");
        }

        workspace.Name = request.Name.Trim();
        workspace.Slug = slug;
        workspace.Description = request.Description?.Trim();
        workspace.LogoUrl = request.LogoUrl?.Trim();
        workspace.IsActive = request.IsActive;
        workspace.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapToResponse(workspace);
    }

    public async Task DeleteAsync(int id)
    {
        var workspace = await _context.Workspaces.FindAsync(id);
        if (workspace == null)
        {
            throw new NotFoundException($"Workspace with ID {id} was not found.");
        }

        _context.Workspaces.Remove(workspace);
        await _context.SaveChangesAsync();
    }

    private static WorkspaceResponse MapToResponse(Workspace workspace)
    {
        return new WorkspaceResponse
        {
            Id = workspace.Id,
            Name = workspace.Name,
            Slug = workspace.Slug,
            Description = workspace.Description,
            LogoUrl = workspace.LogoUrl,
            IsActive = workspace.IsActive,
            CreatedAt = workspace.CreatedAt,
            UpdatedAt = workspace.UpdatedAt
        };
    }
}

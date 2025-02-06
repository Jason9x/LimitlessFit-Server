using Microsoft.EntityFrameworkCore;
using LimitlessFit.Data;
using LimitlessFit.Interfaces;
using LimitlessFit.Models;
using LimitlessFit.Models.Requests;
using LimitlessFit.Models.Dtos;
using LimitlessFit.Services.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace LimitlessFit.Services;

public class UserService(ApplicationDbContext context, IAuthService authService, IHubContext<UserHub> hubContext)
    : IUserService
{
    public async Task<(List<UserDto> users, int totalPages)> GetUsersAsync(UserSearchRequest request)
    {
        var query = context.Users.AsQueryable();
        var userId = authService.GetUserIdFromClaims();

        var totalCount = await query.Where(user => user.Id != userId).CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        var users = await ApplySearchFilter(query, request.SearchTerm)
            .Where(user => user.Id != userId)
            .AsNoTracking()
            .OrderBy(user => user.Name)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(user => new UserDto(
                user.Id,
                user.Name,
                user.Email ?? string.Empty,
                user.RoleId))
            .ToListAsync();

        return (users, totalPages);
    }

    public async Task UpdateUserRoleAsync(int id, int role)
    {
        var user = await context.Users
            .Include(user => user.Role)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            throw new KeyNotFoundException($"User with Id {id} was not found.");

        user.RoleId = role;

        context.Users.Update(user);

        await context.SaveChangesAsync();

        await hubContext.Clients.All.SendAsync("RoleUpdated", user.Id, role);
    }

    public async Task<string> GetUserNameByIdAsync(int id)
    {
        var user = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Id == id);

        return user?.Name ?? string.Empty;
    }

    public async Task<string> GetUserRoleByIdAsync(int id)
    {
        var role = await context.Users
            .AsNoTracking()
            .Where(user => user.Id == id)
            .Select(user => user.Role != null ? user.Role.Name : string.Empty)
            .FirstOrDefaultAsync();

        return role ?? string.Empty;
    }

    private IQueryable<User> ApplySearchFilter(IQueryable<User> query, string? searchTerm)
    {
        return string.IsNullOrWhiteSpace(searchTerm)
            ? query
            : context.Users.Where(user => user.Name.Contains(searchTerm.Trim()));
    }
}
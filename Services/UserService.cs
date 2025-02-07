using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using LimitlessFit.Data;
using LimitlessFit.Interfaces;
using LimitlessFit.Models;
using LimitlessFit.Models.Requests;
using LimitlessFit.Models.Dtos;
using LimitlessFit.Models.Enums;
using LimitlessFit.Services.Hubs;

namespace LimitlessFit.Services;

public class UserService(
    ApplicationDbContext context,
    IAuthService authService,
    INotificationService notificationService,
    IHubContext<UserHub> hubContext)
    : IUserService
{
    public async Task<(List<UserDto> users, int totalPages)> GetUsersAsync(UserSearchRequest request)
    {
        var query = context.Users.AsQueryable();
        var userId = authService.GetUserIdFromClaims();

        query = ApplySearchFilter(query, request.SearchTerm).Where(user => user.Id != userId);

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        var users = await query
            .AsNoTracking()
            .OrderBy(user => user.Role)
            .ThenBy(user => user.Name)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(user => new UserDto(
                user.Id,
                user.Name,
                user.Email ?? string.Empty,
                (RoleEnum)user.RoleId))
            .ToListAsync();

        return (users, totalPages);
    }

    public async Task UpdateUserRoleAsync(int id, RoleEnum role)
    {
        var user = await context.Users
            .Include(user => user.Role)
            .FirstOrDefaultAsync(user => user.Id == id);

        if (user == null)
            throw new KeyNotFoundException($"User with Id {id} was not found.");

        user.RoleId = (int)role;

        context.Users.Update(user);

        await context.SaveChangesAsync();

        await hubContext.Clients.All.SendAsync("RoleUpdated", user.Id, role);

        await notificationService.CreateNotificationAsync(
            user.Id,
            "roleUpdated",
            new Dictionary<string, object>
            {
                { "role", role }
            });
    }

    public async Task<string> GetUserNameByIdAsync(int id)
    {
        var user = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Id == id);

        return user?.Name ?? string.Empty;
    }

    public async Task<RoleEnum> GetUserRoleIdByIdAsync(int id)
    {
        var roleId = await context.Users
            .AsNoTracking()
            .Where(user => user.Id == id)
            .Select(user => user.RoleId)
            .FirstOrDefaultAsync();

        return (RoleEnum)roleId;
    }

    private IQueryable<User> ApplySearchFilter(IQueryable<User> query, string? searchTerm)
    {
        return string.IsNullOrWhiteSpace(searchTerm)
            ? query
            : context.Users.Where(user => user.Name.Contains(searchTerm.Trim()));
    }
}
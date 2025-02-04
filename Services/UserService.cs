using Microsoft.EntityFrameworkCore;
using LimitlessFit.Data;
using LimitlessFit.Interfaces;
using LimitlessFit.Models;
using LimitlessFit.Models.Requests;
using LimitlessFit.Models.Dtos;

namespace LimitlessFit.Services;

public class UserService(ApplicationDbContext context) : IUserService
{
    public async Task<(List<UserDto> users, int totalPages)> GetUsersAsync(UserSearchRequest request)
    {
        var query = context.Users.AsNoTracking().AsQueryable();

        query = ApplySearchFilter(query, request.SearchTerm);

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        var users = await query
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
        var user = await context.Users.FindAsync(id);

        if (user == null)
            throw new KeyNotFoundException($"User with Id {id} was not found.");

        user.RoleId = role;

        context.Users.Update(user);

        await context.SaveChangesAsync();
    }

    public async Task<string> GetUserNameByIdAsync(int id)
    {
        var user = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Id == id);

        return user?.Name ?? string.Empty;
    }

    private IQueryable<User> ApplySearchFilter(IQueryable<User> query, string? searchTerm)
    {
        return string.IsNullOrWhiteSpace(searchTerm)
            ? query
            : context.Users.Where(user => user.Name.Contains(searchTerm.Trim()));
    }
}
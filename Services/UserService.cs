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

    private IQueryable<User> ApplySearchFilter(IQueryable<User> query, string? searchTerm)
    {
        return string.IsNullOrWhiteSpace(searchTerm) ? query : context.Users.Where(user => user.Name.Contains(searchTerm.Trim()));
    }
}
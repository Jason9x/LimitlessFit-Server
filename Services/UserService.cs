using Microsoft.EntityFrameworkCore;
using LimitlessFit.Data;
using LimitlessFit.Interfaces;
using LimitlessFit.Models;
using LimitlessFit.Models.Requests;
using LimitlessFit.Models.Dtos;

namespace LimitlessFit.Services;

public class UserService(ApplicationDbContext context) : IUserService
{
    public async Task<(List<UserDto> orders, int totalPages)> GetUsersAsync(UserSearchRequest request)
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
                user.Name,
                user.Email ?? string.Empty,
                user.RoleId))
            .ToListAsync();

        return (users, totalPages);
    }

    private static IQueryable<User> ApplySearchFilter(IQueryable<User> query, string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return query;

        const string collation = "SQL_Latin1_General_CP1_CI_AI";
        var cleanSearchTerm = searchTerm.Trim();

        return query.Where(user =>
            EF.Functions.Collate(user.Name, collation).Contains(cleanSearchTerm));
    }
}
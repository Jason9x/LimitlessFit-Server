using LimitlessFit.Models.Dtos;
using LimitlessFit.Models.Requests;

namespace LimitlessFit.Interfaces;

public interface IUserService
{
    Task<(List<UserDto> users, int totalPages)> GetUsersAsync(UserSearchRequest request);
    Task UpdateUserRoleAsync(int id, int role);
    Task<string> GetUserNameByIdAsync(int id);
}
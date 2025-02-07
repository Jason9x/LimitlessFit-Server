using LimitlessFit.Models.Dtos;
using LimitlessFit.Models.Enums;
using LimitlessFit.Models.Requests;

namespace LimitlessFit.Interfaces;

public interface IUserService
{
    Task<(List<UserDto> users, int totalPages)> GetUsersAsync(UserSearchRequest request);

    Task UpdateUserRoleAsync(int id, RoleEnum role);

    Task<string> GetUserNameByIdAsync(int id);
    Task<RoleEnum> GetUserRoleIdByIdAsync(int id);
}
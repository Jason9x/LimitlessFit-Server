using LimitlessFit.Models;
using LimitlessFit.Models.Requests;

namespace LimitlessFit.Interfaces;

public interface IItemsService
{
    Task<(List<Item> Items, int TotalPages)> GetAllItemsAsync(PagingRequest request);
}
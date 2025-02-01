using LimitlessFit.Models;
using LimitlessFit.Models.Requests;

namespace LimitlessFit.Interfaces;

public interface IItemService
{
    Task<(List<Item> Items, int TotalPages)> GetAllItemsAsync(PagingRequest request);
}
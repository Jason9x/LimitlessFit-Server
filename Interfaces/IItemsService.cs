using LimitlessFit.Models;
using LimitlessFit.Models.Requests;

namespace LimitlessFit.Interfaces;

public interface IItemsService
{
    Task<List<Item>> GetAllItemsAsync(PagingRequest request);
    Task<int> GetTotalItemsCountAsync();
}
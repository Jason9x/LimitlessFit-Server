using LimitlessFit.Models;

namespace LimitlessFit.Interfaces;

public interface IItemsService
{
    Task<List<Item>> GetAllItemsAsync(int pageNumber, int pageSize);
    Task<int> GetTotalItemsCountAsync();
}
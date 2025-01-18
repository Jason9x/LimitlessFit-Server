using LimitlessFit.Models;

namespace LimitlessFit.Interfaces;

public interface IItemsService
{
    Task<List<Item>> GetAllItemsAsync();
}

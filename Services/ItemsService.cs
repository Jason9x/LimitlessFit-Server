using Microsoft.EntityFrameworkCore;
using LimitlessFit.Data;
using LimitlessFit.Interfaces;
using LimitlessFit.Models;

namespace LimitlessFit.Services;

public class ItemsService(ApplicationDbContext context) : IItemsService
{
    public async Task<List<Item>> GetAllItemsAsync(int pageNumber, int pageSize)
    {
        return await context.Items
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetTotalItemsCountAsync()
    {
        return await context.Items.CountAsync();
    }
}
using Microsoft.EntityFrameworkCore;
using LimitlessFit.Data;
using LimitlessFit.Interfaces;
using LimitlessFit.Models;
using LimitlessFit.Models.Requests;

namespace LimitlessFit.Services;

public class ItemsService(ApplicationDbContext context) : IItemsService
{
    public async Task<List<Item>> GetAllItemsAsync(PagingRequest request)
    {
        return await context.Items
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();
    }

    public async Task<int> GetTotalItemsCountAsync()
    {
        return await context.Items.CountAsync();
    }
}
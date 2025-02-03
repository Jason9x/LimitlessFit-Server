using Microsoft.EntityFrameworkCore;
using LimitlessFit.Data;
using LimitlessFit.Interfaces;
using LimitlessFit.Models;
using LimitlessFit.Models.Requests;

namespace LimitlessFit.Services;

public class ItemService(ApplicationDbContext context) : IItemService
{
    public async Task<(List<Item> Items, int TotalPages)> GetAllItemsAsync(PagingRequest request)
    {
        var totalItems = await context.Items.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize);

        var items = await context.Items
            .AsNoTracking()
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return (items, totalPages);
    }
}
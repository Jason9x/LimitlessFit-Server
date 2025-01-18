using Microsoft.EntityFrameworkCore;

using LimitlessFit.Data;
using LimitlessFit.Interfaces;
using LimitlessFit.Models;

namespace LimitlessFit.Services;

public class ItemsService(ApplicationDbContext context) : IItemsService
{
    public async Task<List<Item>> GetAllItemsAsync()
    {
        return await context.Items.ToListAsync();
    }
}
using Microsoft.EntityFrameworkCore;

using LimitlessFit.Models;

namespace LimitlessFit.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Item> Items { get; set; }
}
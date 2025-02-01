using Microsoft.EntityFrameworkCore;
using LimitlessFit.Models;
using LimitlessFit.Models.Orders;

namespace LimitlessFit.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }

    public DbSet<Item> Items { get; set; }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    
    public DbSet<Notification> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(user => user.Email)
            .IsUnique();

        modelBuilder.Entity<Order>()
            .Property(order => order.Status)
            .HasConversion<string>()
            .HasColumnType("enum('Pending', 'Processing', 'Shipping', 'Delivered')");
    }
}
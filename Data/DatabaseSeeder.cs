using Microsoft.EntityFrameworkCore;

namespace LimitlessFit.Data;

public static class DatabaseSeeder
{
    public static async Task SeedDatabaseAsync(ApplicationDbContext context, IHostEnvironment env)
    {
        if (context.Items.Any() || context.Orders.Any()) return;

        var rootPath = env.ContentRootPath;
        var sqlFilePath = Path.Combine(rootPath, "Data", "SeedData", "seed-data.sql");

        if (!File.Exists(sqlFilePath)) throw new FileNotFoundException("Seed SQL file not found.", sqlFilePath);

        var sqlScript = await File.ReadAllTextAsync(sqlFilePath);

        await context.Database.ExecuteSqlRawAsync(sqlScript);
    }
}
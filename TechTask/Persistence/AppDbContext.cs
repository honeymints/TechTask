using Microsoft.EntityFrameworkCore;

namespace TechTask.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options) { }

    public DbSet<ItemInfo> Items { get; set; }

    public DbSet<GroupedItem> GroupedItems { get; set; }

}

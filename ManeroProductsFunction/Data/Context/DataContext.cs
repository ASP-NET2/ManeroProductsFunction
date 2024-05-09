using ManeroProductsFunction.Data.Entitys;
using Microsoft.EntityFrameworkCore;

namespace ManeroProductsFunction.Data.Context;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<ProductsEntity> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductsEntity>()
            .ToContainer("Products").HasPartitionKey(x => x.PartitionKey);
    }
}

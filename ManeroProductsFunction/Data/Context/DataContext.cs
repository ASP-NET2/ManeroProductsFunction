using ManeroProductsFunction.Data.Entitys;
using Microsoft.EntityFrameworkCore;

namespace ManeroProductsFunction.Data.Context;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<ProductsEntity> Products { get; set; }
    public DbSet<CategoryEntity> Category { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.Entity<ProductsEntity>()
        //    .ToContainer("Products").HasPartitionKey(x => x.PartitionKey);
        modelBuilder.Entity<CategoryEntity>()
            .ToContainer("Category")
            .HasPartitionKey(x => x.PartitionKey);

    }
}

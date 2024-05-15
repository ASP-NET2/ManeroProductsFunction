using ManeroProductsFunction.Data.Entitys;
using Microsoft.EntityFrameworkCore;

namespace ManeroProductsFunction.Data.Context;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<ProductEntity> Product { get; set; }
    public DbSet<CategoryEntity> Category { get; set; }
    public DbSet<SubCategoryEntity> SubCategory { get; set; }
    


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductEntity>()
            .ToContainer("Products")
            .HasPartitionKey(x => x.PartitionKey);

        modelBuilder.Entity<CategoryEntity>()
            .ToContainer("Category")
            .HasPartitionKey(x => x.PartitionKey);

        modelBuilder.Entity<SubCategoryEntity>()
            .ToContainer("SubCategorys")
            .HasPartitionKey(x => x.PartitionKey);



    }
}

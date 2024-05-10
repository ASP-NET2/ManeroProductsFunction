using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace ManeroProductsFunction.Data.Entitys;

public class CategoryEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string CategoryName { get; set; } = null!;
    public string PartitionKey { get; set; } = "Category";
    public string Category { get; set; } = "Category";
}
    
using System.Globalization;

namespace ManeroProductsFunction.Data.Entitys;

public class CategoryEntity
{
    public string CategoryName { get; set; } = null!;
    public string PartitionKey { get; set; } = "Category";
}

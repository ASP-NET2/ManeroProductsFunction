using System.ComponentModel.DataAnnotations;

namespace ManeroProductsFunction.Data.Entitys;

public class SubCategoryEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string PartitionKey { get; set; } = "SubCategory";
    public string SubCategory { get; set; } = null!;
}

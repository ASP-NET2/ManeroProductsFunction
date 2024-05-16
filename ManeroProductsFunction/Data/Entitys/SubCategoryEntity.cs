using System.ComponentModel.DataAnnotations;

namespace ManeroProductsFunction.Data.Entitys;

public class SubCategoryEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string PartitionKey { get; set; } = "SubCategory";
    public string SubCategory { get; set; } = "SubCategory";
    public string SubCategoryName { get; set; } = null!;
    public string ImageLink { get; set; } = null!;
}

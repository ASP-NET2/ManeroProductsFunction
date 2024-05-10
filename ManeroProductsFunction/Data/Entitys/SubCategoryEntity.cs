namespace ManeroProductsFunction.Data.Entitys;

public class SubCategoryEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string SubCategoryName { get; set; } = null!;
    public string PartitionKey { get; set; } = "SubCategory";
    public string SubCategory { get; set; } = "SubCategory";
}

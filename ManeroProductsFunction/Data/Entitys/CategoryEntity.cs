﻿
namespace ManeroProductsFunction.Data.Entitys;

public class CategoryEntity
{    
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string PartitionKey { get; set; } = "Category";
    public string Category { get; set; } = "Category";
    public string CategoryName { get; set; } = null!;
    public string ImageLink { get; set; } = null!;
  
}
    
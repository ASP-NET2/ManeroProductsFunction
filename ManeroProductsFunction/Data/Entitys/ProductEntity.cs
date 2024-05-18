using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManeroProductsFunction.Data.Entitys;

public class ProductEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime Created { get; set; } = DateTime.Now;
    public string PartitionKey { get; set; } = "Products";
    public string Products { get; set; } = "Products";
    public string Author { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Price { get; set; } = null!;
    public string? DiscountPrice { get; set; }
    public string? ShortDescription { get; set; }
    public string? LongDescription { get; set; }
    public string Language { get; set; } = "Swedish";
    public string? Pages { get; set; }
    public string? PublishDate { get; set; }
    public string? Publisher { get; set; }
    public string? ISBN { get; set; }
    public string ImageUrl { get; set; } = null!;
    public string Category { get; set; } = null!;
    public string SubCategory { get; set; } = null!;
    public string Format { get; set; } = null!;
    public bool OnSale { get; set; } = false;
    public bool BestSeller { get; set; } = false;
    public bool FeaturedProduct { get; set; } = false;
}

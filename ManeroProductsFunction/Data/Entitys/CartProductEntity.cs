using System.ComponentModel.DataAnnotations;

namespace ManeroProductsFunction.Data.Entitys;

public partial class CartProductEntity
{
    [Key]
    public string ProductId { get; set; }=Guid.NewGuid().ToString();
    public string ProductName { get; set; } = null!;
    public int Quantity { get; set; } = 0!;
    public int Price { get; set; } = 0!;
}

namespace ManeroProductsFunction.Data.Entitys;

public partial class CartEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string PartitionKey { get; set; } = "Cart";
    public string Cart { get; set; } = "Cart";
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public List<CartProductEntity> Products { get; set; } = [];
}

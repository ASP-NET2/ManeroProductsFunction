namespace ManeroProductsFunction.Data.Entitys
{
    public class ProductsEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime Created { get; set; } = DateTime.Now;
        public string PartitionKey { get; set; } = "Products";
        public string Products { get; set; } = "Products";
        public string Author { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? ShortDescription { get; set; }
        public string? LongDescription { get; set; }
        public string Language { get; set; } = "Swedish";
        public string? Pages { get; set; }
        public string? PublishDate { get; set; }
        public string? Publisher { get; set; }
        public string? ISBN { get; set; }
    }
}

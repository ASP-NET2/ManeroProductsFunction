namespace ManeroProductsFunction.Data.Entitys
{
    public class FormatEntity 
    {
        public string FormatType { get; set; } = null!;
        public string Price { get; set; } = null!;
        public string? DiscountPrice { get; set; }
    }
}

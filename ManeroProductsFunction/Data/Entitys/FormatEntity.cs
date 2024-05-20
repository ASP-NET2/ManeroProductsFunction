namespace ManeroProductsFunction.Data.Entitys;

public class FormatEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string PartitionKey { get; set; } = "Format";
    public string Format { get; set; } = "Format";
    public string FormatName { get; set; } = null!;  
    }

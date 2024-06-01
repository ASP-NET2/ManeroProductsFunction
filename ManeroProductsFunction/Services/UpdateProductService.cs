using ManeroProductsFunction.Data.Context;
using ManeroProductsFunction.Data.Entitys;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

public class UpdateProductService
{
    private readonly ILogger<UpdateProductService> _logger;
    private readonly DataContext _context;

    public UpdateProductService(ILogger<UpdateProductService> logger, DataContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<ProductEntity?> UpdateProductAsync(ProductEntity updatedProduct)
    {
        try
        {
            var productToUpdate = await _context.Product
                .FirstOrDefaultAsync(p => p.Id == updatedProduct.Id && p.PartitionKey == updatedProduct.PartitionKey);

            if (productToUpdate == null)
            {
                _logger.LogWarning($"Product with ID {updatedProduct.Id} and PartitionKey {updatedProduct.PartitionKey} not found.");
                return null;
            }

            productToUpdate.Title = updatedProduct.Title;
            productToUpdate.Author = updatedProduct.Author;
            productToUpdate.ImageUrl = updatedProduct.ImageUrl;
            productToUpdate.ShortDescription = updatedProduct.ShortDescription;
            productToUpdate.LongDescription = updatedProduct.LongDescription;
            productToUpdate.Language = updatedProduct.Language;
            productToUpdate.Pages = updatedProduct.Pages;
            productToUpdate.PublishDate = updatedProduct.PublishDate;
            productToUpdate.Publisher = updatedProduct.Publisher;
            productToUpdate.ISBN = updatedProduct.ISBN;
            productToUpdate.CategoryName = updatedProduct.CategoryName;
            productToUpdate.SubCategoryName = updatedProduct.SubCategoryName;
            productToUpdate.FormatName = updatedProduct.FormatName;
            productToUpdate.IsFavorite = updatedProduct.IsFavorite;
            productToUpdate.Rating = updatedProduct.Rating;

            _context.Product.Update(productToUpdate);
            await _context.SaveChangesAsync();

            return productToUpdate;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred.");
            throw;
        }
    }
}

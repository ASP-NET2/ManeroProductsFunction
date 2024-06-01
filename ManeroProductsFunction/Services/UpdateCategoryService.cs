using ManeroProductsFunction.Data.Context;
using ManeroProductsFunction.Data.Entitys;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

public class UpdateCategoryService
{
    private readonly ILogger<UpdateCategoryService> _logger;
    private readonly DataContext _context;

    public UpdateCategoryService(ILogger<UpdateCategoryService> logger, DataContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<CategoryEntity?> UpdateCategoryAsync(CategoryEntity updatedCategory)
    {
        try
        {
            var categoryToUpdate = await _context.Category
                .FirstOrDefaultAsync(c => c.Id == updatedCategory.Id && c.PartitionKey == updatedCategory.PartitionKey);

            if (categoryToUpdate == null)
            {
                _logger.LogWarning($"Category with ID {updatedCategory.Id} and PartitionKey {updatedCategory.PartitionKey} not found.");
                return null;
            }

            categoryToUpdate.CategoryName = updatedCategory.CategoryName;
            categoryToUpdate.ImageLink = updatedCategory.ImageLink;

            _context.Category.Update(categoryToUpdate);
            await _context.SaveChangesAsync();

            return categoryToUpdate;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred.");
            throw;
        }
    }
}

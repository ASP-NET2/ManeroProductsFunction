using ManeroProductsFunction.Data.Context;
using ManeroProductsFunction.Data.Entitys;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

public class UpdateSubCategoryService
{
    private readonly ILogger<UpdateSubCategoryService> _logger;
    private readonly DataContext _context;

    public UpdateSubCategoryService(ILogger<UpdateSubCategoryService> logger, DataContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<SubCategoryEntity?> UpdateSubCategoryAsync(SubCategoryEntity updatedSubCategory)
    {
        try
        {
            var subCategoryToUpdate = await _context.SubCategory
                .FirstOrDefaultAsync(c => c.Id == updatedSubCategory.Id && c.PartitionKey == updatedSubCategory.PartitionKey);

            if (subCategoryToUpdate == null)
            {
                _logger.LogWarning($"SubCategory with ID {updatedSubCategory.Id} and PartitionKey {updatedSubCategory.PartitionKey} not found.");
                return null;
            }

            subCategoryToUpdate.SubCategoryName = updatedSubCategory.SubCategoryName;
            subCategoryToUpdate.ImageLink = updatedSubCategory.ImageLink;

            _context.SubCategory.Update(subCategoryToUpdate);
            await _context.SaveChangesAsync();

            return subCategoryToUpdate;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred.");
            throw;
        }
    }
}

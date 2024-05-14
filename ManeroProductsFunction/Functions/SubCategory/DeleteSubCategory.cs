using ManeroProductsFunction.Data.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ManeroProductsFunction.Functions.SubCategory;

public class DeleteSubCategory(ILogger<DeleteSubCategory> logger, DataContext context)
{
    private readonly ILogger<DeleteSubCategory> _logger = logger;
    private readonly DataContext _context = context;


    [Function("DeleteSubCategory")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "subcategory/{id}")] HttpRequest req, string id)
    {
        _logger.LogInformation("HTTP trigger function processed a delete request.");

        if (string.IsNullOrEmpty(id))
        {
            _logger.LogWarning("DeleteSubCategory function was called without a category ID.");
            return new BadRequestObjectResult("SubCategory ID must be provided.");
        }

        try
        {
            var subcategory = await _context.SubCategory
                                         .SingleOrDefaultAsync(c => c.Id == id && c.PartitionKey == "SubCategory");
            if (subcategory == null)
            {
                _logger.LogInformation($"SubCategory with ID: {id} not found in partition 'SubCategory'.");
                return new NotFoundResult();
            }

            _context.SubCategory.Remove(subcategory);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"SubCategory with ID: {id} has been deleted.");
            return new OkObjectResult($"SubCategory with ID: {id} deleted successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error occurred while trying to delete subcategory with ID: {id} in partition 'SubCategory'. Error: {ex.Message}", ex);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}
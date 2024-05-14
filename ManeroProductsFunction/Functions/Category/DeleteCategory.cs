using ManeroProductsFunction.Data.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace ManeroProductsFunction.Functions.Category;

public class DeleteCategory(ILogger<DeleteCategory> logger, DataContext context)
{
    private readonly ILogger<DeleteCategory> _logger = logger;
    private readonly DataContext _context = context;


    [Function("DeleteCategory")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "category/{id}")] HttpRequest req, string id)
    {
        _logger.LogInformation("HTTP trigger function processed a delete request.");

        if (string.IsNullOrEmpty(id))
        {
            _logger.LogWarning("DeleteCategory function was called without a category ID.");
            return new BadRequestObjectResult("Category ID must be provided.");
        }

        try
        {
            var category = await _context.Category
                                         .SingleOrDefaultAsync(c => c.Id == id && c.PartitionKey == "Category");
            if (category == null)
            {
                _logger.LogInformation($"Category with ID: {id} not found in partition 'Category'.");
                return new NotFoundResult();
            }

            _context.Category.Remove(category);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Category with ID: {id} has been deleted.");
            return new OkObjectResult($"Category with ID: {id} deleted successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error occurred while trying to delete category with ID: {id} in partition 'Category'. Error: {ex.Message}", ex);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}

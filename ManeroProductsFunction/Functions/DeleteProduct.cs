using ManeroProductsFunction.Data.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace ManeroProductsFunction.Functions;

public class DeleteProduct(ILogger<DeleteProduct> logger, DataContext context)
{
    private readonly ILogger<DeleteProduct> _logger = logger;
    private readonly DataContext _context = context;

    [FunctionName("DeleteProduct")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "product/{id}")] HttpRequest req,
        string id)
    {
        _logger.LogInformation("HTTP trigger function processed a delete request.");

        var partitionKey = "Products";

        if (string.IsNullOrEmpty(id))
        {
            _logger.LogWarning("DeleteProduct function was called without a product ID.");
            return new BadRequestObjectResult("Product ID must be provided.");
        }

        try
        {
            var product = await _context.Product.FindAsync(new object[] { id, partitionKey });
            if (product == null)
            {
                _logger.LogInformation($"Product with ID: {id} not found in partition {partitionKey}.");
                return new NotFoundResult();
            }

            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Product with ID: {id} has been deleted.");
            return new OkObjectResult($"Product with ID: {id} deleted successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error occurred while trying to delete product with ID: {id} in partition {partitionKey}. Error: {ex.Message}");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}
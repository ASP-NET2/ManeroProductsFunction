using ManeroProductsFunction.Data.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ManeroProductsFunction.Functions.SubCategory;

public class GetSubCategory(ILogger<GetSubCategory> logger, DataContext context)
{
    private readonly ILogger<GetSubCategory> _logger = logger;
    private readonly DataContext _context = context;

    [Function("GetSubCategory")]
    public async Task<IActionResult> RunGetAll([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
    {
        try
        {
            var subcategories = await _context.SubCategory.ToListAsync();

            if (subcategories == null || subcategories.Count == 0)
            {
                _logger.LogInformation("No subcategories found.");
                return new NoContentResult();
            }

            return new OkObjectResult(subcategories);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while getting subcategories: {ex.Message}", ex);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}

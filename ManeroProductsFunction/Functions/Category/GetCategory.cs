using ManeroProductsFunction.Data.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ManeroProductsFunction.Functions.Category
{
    public class GetCategory(ILogger<GetCategory> logger, DataContext context)
    {
        private readonly ILogger<GetCategory> _logger = logger;
        private readonly DataContext _context = context;

        [Function("GetCategory")]
        public async Task<IActionResult> RunGetAll([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            try
            {
                var categories = await _context.Category.ToListAsync();

                if (categories == null || categories.Count == 0)
                {
                    _logger.LogInformation("No categories found.");
                    return new NoContentResult();
                }

                return new OkObjectResult(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while getting categories: {ex.Message}", ex);
                return new ObjectResult("An error occurred while processing your request.")
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
    }
}

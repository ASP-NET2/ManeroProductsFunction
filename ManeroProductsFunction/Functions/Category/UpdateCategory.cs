using ManeroProductsFunction.Data.Context;
using ManeroProductsFunction.Data.Entitys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ManeroProductsFunction.Functions.Category;

public class UpdateCategory(ILogger<UpdateCategory> logger, DataContext context)
{
    private readonly ILogger<UpdateCategory> _logger = logger;
    private readonly DataContext _context = context;

    [Function("UpdateCategory")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = null)] HttpRequest req)
    {
        _logger.LogInformation("Processing update request for category.");

        try
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            CategoryEntity updatedCategory = JsonConvert.DeserializeObject<CategoryEntity>(requestBody);

            if (updatedCategory == null)
            {
                _logger.LogWarning("Invalid category data received.");
                return new BadRequestObjectResult("Invalid category data.");
            }

            
            var categoryToUpdate = await _context.Category
                .SingleOrDefaultAsync(c => c.Id == updatedCategory.Id && c.PartitionKey == updatedCategory.PartitionKey);

            if (categoryToUpdate == null)
            {
                _logger.LogWarning($"Category with ID {updatedCategory.Id} and PartitionKey {updatedCategory.PartitionKey} not found.");
                return new NotFoundResult();
            }

           
            categoryToUpdate.Category = updatedCategory.Category;
          

            _context.Category.Update(categoryToUpdate);
            await _context.SaveChangesAsync();

            return new OkObjectResult(categoryToUpdate);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error parsing request body.");
            return new BadRequestObjectResult("Error in request format or data.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred.");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}
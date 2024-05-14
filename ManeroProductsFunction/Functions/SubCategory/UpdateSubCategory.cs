using ManeroProductsFunction.Data.Context;
using ManeroProductsFunction.Data.Entitys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ManeroProductsFunction.Functions.SubCategory;

public class UpdateSubCategory(ILogger<UpdateSubCategory> logger, DataContext context)
{
    private readonly ILogger<UpdateSubCategory> _logger = logger;
    private readonly DataContext _context = context;

    [Function("UpdateSubCategory")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = null)] HttpRequest req)
    {
        _logger.LogInformation("Processing update request for subcategory.");

        try
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            SubCategoryEntity updatedSubCategory = JsonConvert.DeserializeObject<SubCategoryEntity>(requestBody);

            if (updatedSubCategory == null)
            {
                _logger.LogWarning("Invalid subcategory data received.");
                return new BadRequestObjectResult("Invalid subcategory data.");
            }


            var subcategoryToUpdate = await _context.SubCategory
                .SingleOrDefaultAsync(c => c.Id == updatedSubCategory.Id && c.PartitionKey == updatedSubCategory.PartitionKey);

            if (subcategoryToUpdate == null)
            {
                _logger.LogWarning($"Subcategory with ID {updatedSubCategory.Id} and PartitionKey {updatedSubCategory.PartitionKey} not found.");
                return new NotFoundResult();
            }


            subcategoryToUpdate.SubCategory = updatedSubCategory.SubCategory;


            _context.SubCategory.Update(subcategoryToUpdate);
            await _context.SaveChangesAsync();

            return new OkObjectResult(subcategoryToUpdate);
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
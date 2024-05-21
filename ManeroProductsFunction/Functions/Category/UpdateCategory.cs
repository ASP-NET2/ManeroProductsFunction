using ManeroProductsFunction.Data.Context;
using ManeroProductsFunction.Data.Entitys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;

namespace ManeroProductsFunction.Functions.Category;
public class UpdateCategory(ILogger<UpdateCategory> logger, DataContext context)
{
    private readonly ILogger<UpdateCategory> _logger = logger;
    private readonly DataContext _context = context;

    [Function("UpdateCategory")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "put")] HttpRequestData req)
    {
        _logger.LogInformation("Processing update request for category.");

        var response = req.CreateResponse();

        try
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            CategoryEntity updatedCategory = JsonConvert.DeserializeObject<CategoryEntity>(requestBody);

            if (updatedCategory == null)
            {
                _logger.LogWarning("Invalid category data received.");
                response.StatusCode = HttpStatusCode.BadRequest;
                await response.WriteStringAsync("Invalid category data.");
                return response;
            }

            var categoryToUpdate = await _context.Category.FindAsync(updatedCategory.Id, updatedCategory.PartitionKey);
            if (categoryToUpdate == null)
            {
                _logger.LogWarning($"Category with ID {updatedCategory.Id} and PartitionKey {updatedCategory.PartitionKey} not found.");
                response.StatusCode = HttpStatusCode.NotFound;
                return response;
            }

            categoryToUpdate.CategoryName = updatedCategory.CategoryName;
            categoryToUpdate.ImageLink = updatedCategory.ImageLink;

            _context.Category.Update(categoryToUpdate);
            await _context.SaveChangesAsync();

            response.StatusCode = HttpStatusCode.OK;
            await response.WriteAsJsonAsync(categoryToUpdate);
            return response;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error parsing request body.");
            response.StatusCode = HttpStatusCode.BadRequest;
            await response.WriteStringAsync("Error in request format or data.");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred.");
            response.StatusCode = HttpStatusCode.InternalServerError;
            await response.WriteStringAsync("Unexpected error occurred.");
            return response;
        }
    }
}
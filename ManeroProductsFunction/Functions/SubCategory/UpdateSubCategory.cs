using ManeroProductsFunction.Data.Context;
using ManeroProductsFunction.Data.Entitys;
using ManeroProductsFunction.Functions.Category;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;

namespace ManeroProductsFunction.Functions.SubCategory;

public class UpdateSubCategory(ILogger<UpdateSubCategory> logger, DataContext context)
{
    private readonly ILogger<UpdateSubCategory> _logger = logger;
    private readonly DataContext _context = context;

    [Function("UpdateSubCategory")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "put")] HttpRequestData req)
    {
        _logger.LogInformation("Processing update request for subcategory.");

        var response = req.CreateResponse();

        try
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            SubCategoryEntity updatedSubCategory = JsonConvert.DeserializeObject<SubCategoryEntity>(requestBody);

            if (updatedSubCategory == null)
            {
                _logger.LogWarning("Invalid subcategory data received.");
                response.StatusCode = HttpStatusCode.BadRequest;
                await response.WriteStringAsync("Invalid subcategory data.");
                return response;
            }

            var subCategoryToUpdate = await _context.SubCategory.FindAsync(updatedSubCategory.Id, updatedSubCategory.PartitionKey);
            if (subCategoryToUpdate == null)
            {
                _logger.LogWarning($"SubCategory with ID {updatedSubCategory.Id} and PartitionKey {updatedSubCategory.PartitionKey} not found.");
                response.StatusCode = HttpStatusCode.NotFound;
                return response;
            }

            subCategoryToUpdate.SubCategoryName = updatedSubCategory.SubCategoryName;
            subCategoryToUpdate.ImageLink = updatedSubCategory.ImageLink;

            _context.SubCategory.Update(subCategoryToUpdate);
            await _context.SaveChangesAsync();

            response.StatusCode = HttpStatusCode.OK;
            await response.WriteAsJsonAsync(subCategoryToUpdate);
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
using ManeroProductsFunction.Data.Context;
using ManeroProductsFunction.Data.Entitys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;

namespace ManeroProductsFunction.Functions;
public class UpdateProduct(ILogger<UpdateProduct> logger, DataContext context)
{
    private readonly ILogger<UpdateProduct> _logger = logger;
    private readonly DataContext _context = context;

    [Function("UpdateProduct")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "put")] HttpRequestData req)
    {
        _logger.LogInformation("Processing update request for product.");

        var response = req.CreateResponse();

        try
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            _logger.LogInformation($"Request body: {requestBody}");
            ProductEntity updatedProduct = JsonConvert.DeserializeObject<ProductEntity>(requestBody);

            if (updatedProduct == null)
            {
                _logger.LogWarning("Invalid product data received.");
                response.StatusCode = HttpStatusCode.BadRequest;
                await response.WriteStringAsync("Invalid product data.");
                return response;
            }

            _logger.LogInformation($"Finding product with ID {updatedProduct.Id} and PartitionKey {updatedProduct.PartitionKey}");
            var productToUpdate = await _context.Product.FindAsync(updatedProduct.Id, updatedProduct.PartitionKey);
            if (productToUpdate == null)
            {
                _logger.LogWarning($"Product with ID {updatedProduct.Id} and PartitionKey {updatedProduct.PartitionKey} not found.");
                response.StatusCode = HttpStatusCode.NotFound;
                return response;
            }

            productToUpdate.Title = updatedProduct.Title;
            productToUpdate.Author = updatedProduct.Author;
            productToUpdate.ImageUrl = updatedProduct.ImageUrl;
            productToUpdate.ShortDescription = updatedProduct.ShortDescription;
            productToUpdate.LongDescription = updatedProduct.LongDescription;
            productToUpdate.Language = updatedProduct.Language;
            productToUpdate.Pages = updatedProduct.Pages;
            productToUpdate.PublishDate = updatedProduct.PublishDate;
            productToUpdate.Publisher = updatedProduct.Publisher;
            productToUpdate.ISBN = updatedProduct.ISBN;
            productToUpdate.CategoryName = updatedProduct.CategoryName;
            productToUpdate.SubCategoryName = updatedProduct.SubCategoryName;
            productToUpdate.FormatName = updatedProduct.FormatName;

            _context.Product.Update(productToUpdate);
            await _context.SaveChangesAsync();

            response.StatusCode = HttpStatusCode.OK;
            await response.WriteAsJsonAsync(productToUpdate);
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
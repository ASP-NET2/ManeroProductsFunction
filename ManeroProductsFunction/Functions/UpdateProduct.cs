using ManeroProductsFunction.Data.Context;
using ManeroProductsFunction.Data.Entitys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ManeroProductsFunction.Functions;
public class UpdateProduct(ILogger<UpdateProduct> logger, DataContext context)
{
    private readonly ILogger<UpdateProduct> _logger = logger;
    private readonly DataContext _context = context;

    [Function("UpdateProduct")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = null)] HttpRequest req)
    {
        _logger.LogInformation("Processing update request for product.");

        try
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            ProductEntity updatedProduct = JsonConvert.DeserializeObject<ProductEntity>(requestBody);

            if (updatedProduct == null)
            {
                _logger.LogWarning("Invalid product data received.");
                return new BadRequestObjectResult("Invalid product data.");
            }

            var productToUpdate = await _context.Product.FindAsync(new object[] { updatedProduct.Id, updatedProduct.PartitionKey });
            if (productToUpdate == null)
            {
                _logger.LogWarning($"Product with ID {updatedProduct.Id} and PartitionKey {updatedProduct.PartitionKey} not found.");
                return new NotFoundResult();
            }


            productToUpdate.Title = updatedProduct.Title;
            productToUpdate.Author = updatedProduct.Author;
            productToUpdate.ShortDescription = updatedProduct.ShortDescription;
            productToUpdate.LongDescription = updatedProduct.LongDescription;
            productToUpdate.Language = updatedProduct.Language;
            productToUpdate.Pages = updatedProduct.Pages;
            productToUpdate.PublishDate = updatedProduct.PublishDate;
            productToUpdate.Publisher = updatedProduct.Publisher;
            productToUpdate.ISBN = updatedProduct.ISBN;
            productToUpdate.Category = updatedProduct.Category;
            productToUpdate.SubCategory = updatedProduct.SubCategory;

            _context.Product.Update(productToUpdate);
            await _context.SaveChangesAsync();

            return new OkObjectResult(productToUpdate);
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

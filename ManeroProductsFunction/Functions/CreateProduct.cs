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
public class CreateProduct(ILogger<CreateProduct> logger, DataContext context)
{
    private readonly ILogger<CreateProduct> _logger = logger;
    private readonly DataContext _context = context;

    [Function("CreateProduct")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        try
        {
           
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var product = JsonConvert.DeserializeObject<ProductEntity>(body);

            
            _context.Product.Add(product);
            await _context.SaveChangesAsync();

         
            var response = new OkObjectResult(product);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating product: {ex.Message}");

            
            var errorResponse = new ObjectResult(new { error = ex.Message })
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };
            return errorResponse;
        }
    }
}

using ManeroProductsFunction.Data.Context;
using ManeroProductsFunction.Data.Entitys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ManeroProductsFunction.Functions;

public class GetAllProducts(ILogger<GetAllProducts> logger, DataContext context)
{
    private readonly ILogger<GetAllProducts> _logger = logger;
    private readonly DataContext _context = context;

    [Function("GetAllProducts")]
    public async Task<IActionResult> RunGetAll([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        try
        {
            _logger.LogInformation("Fetching products from the database.");

            // Kontrollera att _context är korrekt initialiserad
            if (_context == null)
            {
                _logger.LogError("DataContext is null.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            // Kontrollera att Product DbSet är korrekt
            if (_context.Product == null)
            {
                _logger.LogError("Product DbSet is null.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            var products = await _context.Product.ToListAsync();

            // Lägg till en säkerhetskontroll innan du loggar antalet produkter
            if (products == null)
            {
                _logger.LogError("Products list is null.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            _logger.LogInformation($"Products fetched: {products.Count}");

            // Kontrollera om products är en tom lista
            if (products.Count == 0)
            {
                _logger.LogInformation("Products list is empty.");
                return new NoContentResult();
            }

            _logger.LogInformation($"Retrieved {products.Count} products.");
            return new OkObjectResult(products);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while getting products: {ex.Message}", ex);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}
using ManeroProductsFunction.Data.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ManeroProductsFunction.Functions;


    public class GetAllProducts(ILogger<GetAllProducts> logger, DataContext context)
{
        private readonly ILogger<GetAllProducts> _logger = logger;
        private readonly DataContext _context = context;

    [Function("GetAllProducts")]
        public async Task<IActionResult> RunGetAll([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
        {
            
            var products = await _context.Product.ToListAsync();

            if (products == null || products.Count == 0)
            {
                
                return new NoContentResult();
            }

            
            return new OkObjectResult(products);
        }
    }


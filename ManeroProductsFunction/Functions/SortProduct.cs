using ManeroProductsFunction.Data.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ManeroProductsFunction.Functions
{
    public class SortProduct(ILogger<SortProduct> logger, DataContext context)
    {
        private readonly ILogger<SortProduct> _logger = logger;
        private readonly DataContext _context = context;

        [Function("SortProduct")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            var category = req.Query["category"].ToString().ToLower();
            var products = await _context.Product.ToListAsync();

            if (products == null || products.Count == 0)
            {
                return new NoContentResult();
            }

            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.CategoryName.ToLower() == category).ToList();
            }

            return new OkObjectResult(products);
        }
    }
}
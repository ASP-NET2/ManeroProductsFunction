using ManeroProductsFunction.Data.Context;
using ManeroProductsFunction.Data.Entitys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ManeroProductsFunction.Functions
{
    public class ProductFunction(ILogger<ProductFunction> logger, DataContext context)
    {
        private readonly ILogger<ProductFunction> _logger = logger;
        private readonly DataContext _context = context;

        [Function("CreateProduct")]
        public async Task <IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var product = JsonConvert.DeserializeObject<ProductsEntity>(body);

            _context.Product.Add(product);
            await _context.SaveChangesAsync();

            return new OkObjectResult(product);
        }
    }
}
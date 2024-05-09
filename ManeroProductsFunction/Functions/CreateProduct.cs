using ManeroProductsFunction.Data.Context;
using ManeroProductsFunction.Data.Entitys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ManeroProductsFunction.Functions
{
    public class CreateProduct(ILogger<CreateProduct> logger, DataContext context)
    {
        private readonly ILogger<CreateProduct> _logger = logger;
        private readonly DataContext _context = context;

        [Function("CreateProduct")]
        public async Task <IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var product = JsonConvert.DeserializeObject<ProductsEntity>(body);

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return new OkObjectResult(product);
        }
    }
}

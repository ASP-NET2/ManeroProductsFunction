using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ManeroProductsFunction.Data.Context;

namespace ManeroProductsFunction.Data.Functions
{
    public class GetCustomerCart(ILogger<GetCustomerCart> logger, DataContext context)
    {
        private readonly ILogger<GetCustomerCart> _logger = logger;
        private readonly DataContext _context = context;

        [Function("GetCustomerCart")]
        public async Task <IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetCustomerCart/{id}")] HttpRequest req, string id)
        {
            try
            {
                var result = await _context.CartEntity.FirstOrDefaultAsync(c => c.Id == id);
                if (result == null)
                {
                    return new NoContentResult();
                }
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("C# HTTP trigger function processed a request.");
                return new BadRequestResult();
            }
        }
    }
}

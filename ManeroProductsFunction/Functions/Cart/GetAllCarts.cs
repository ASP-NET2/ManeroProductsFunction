using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ManeroProductsFunction.Data.Context;

namespace ManeroProductsFunction.Data.Functions
{
    public class GetAllCarts(ILogger<GetAllCarts> logger, DataContext context)
    {
        private readonly ILogger<GetAllCarts> _logger = logger;
        private readonly DataContext _context = context;

        [Function("GetAllCarts")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            try
            {
                var result = await _context.CartEntity.ToListAsync();
                if (result.Count == 0)
                {
                    return new NoContentResult();
                }
                else
                {
                    return new OkObjectResult(result);    
                }
            }
            catch (Exception ex) { }
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new BadRequestResult();
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ManeroProductsFunction.Data.Context;
using ManeroProductsFunction.Data.Entitys;
using System.Net;
using Microsoft.Azure.Amqp.Framing;

namespace ManeroProductsFunction.Data.Functions
{
    public class CreateUserCart(ILogger<CreateUserCart> logger, DataContext context)
    {
        private readonly ILogger<CreateUserCart> _logger = logger;
        private readonly DataContext _context = context;

        [Function("CreateUserCart")]
        public async Task <IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            try
            {
                var body = await new StreamReader(req.Body).ReadToEndAsync();
                var userCart = JsonConvert.DeserializeObject<CartEntity>(body);

                if (userCart == null)
                {
                    return new BadRequestResult();
                }

                _context.CartEntity.Add(userCart);
                await _context.SaveChangesAsync();
               
                return new OkObjectResult(userCart);
                 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating product: {ex.Message}");
                return new BadRequestResult();
            }

        }
    }
}

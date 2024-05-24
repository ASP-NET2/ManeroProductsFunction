using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ManeroProductsFunction.Data.Context;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ManeroProductsFunction.Data.Functions
{
    public class DeleteProductFromUserCart
    {
        private readonly ILogger<DeleteProductFromUserCart> _logger;
        private readonly DataContext _context;

        public DeleteProductFromUserCart(ILogger<DeleteProductFromUserCart> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Function("DeleteProductFromCart")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "DeleteProduct/{cartId}/Product/{productId}")] HttpRequest req,
            string cartId, string productId)
        {
            try
            {
                var cart = await _context.CartEntity.Include(c => c.Products).FirstOrDefaultAsync(x => x.Id == cartId);
                if (cart == null)
                {
                    return new NotFoundObjectResult("Cart not found.");
                }

                var prod = cart.Products.FirstOrDefault(x => x.ProductId == productId);
                if (prod == null)
                {
                    return new NotFoundObjectResult("Product not found in the cart.");
                }

                cart.Products.Remove(prod);
                await _context.SaveChangesAsync();

                return new OkObjectResult(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ManeroProductsFunction.Data.Context;

namespace ManeroProductsFunction.Data.Functions
{
    public class DecreaseQuantityFromUserCart(ILogger<DecreaseQuantityFromUserCart> logger, DataContext context)
    {
        private readonly ILogger<DecreaseQuantityFromUserCart> _logger = logger;
        private readonly DataContext _context = context;

        [Function("DecreaseProductQuantityInCart")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "DecreaseCart/{cartId}/product/{productId}")] HttpRequest req,
            string cartId, string productId)
        {
            try
            {
                var cart = await _context.CartEntity.Include(c => c.Products).FirstOrDefaultAsync(x => x.Id == cartId);
                if (cart == null)
                {
                    return new NotFoundObjectResult("Cart not found.");
                }

                var product = cart.Products.FirstOrDefault(x => x.ProductId == productId);
                if (product == null)
                {
                    return new NotFoundObjectResult("Product not found in the cart.");
                }

                product.Quantity--;

                if (product.Quantity <= 0)
                {
                    cart.Products.Remove(product);
                }

                _context.CartEntity.Update(cart);
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


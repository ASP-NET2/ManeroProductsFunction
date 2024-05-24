using ManeroProductsFunction.Data.Context;
using ManeroProductsFunction.Data.Entitys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ManeroProductsFunction.Functions.Cart
{

    public class AddproductToUserCart(ILogger<AddproductToUserCart> logger, DataContext context)
    {
        private readonly ILogger<AddproductToUserCart> _logger = logger;
        private readonly DataContext _context = context;

        [Function("Addproduct")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "AddProdToCart/{id}")] HttpRequest req, string Id)
        {
            try
            {
                var result = await _context.CartEntity.Include(c => c.Products).FirstOrDefaultAsync(x => x.Id == Id);
                if (result == null)
                {
                    return new NotFoundObjectResult("Cart not found");
                }

                var body = await new StreamReader(req.Body).ReadToEndAsync();
                var userProduct = JsonConvert.DeserializeObject<CartProductEntity>(body);

                if (userProduct == null)
                {
                    return new BadRequestObjectResult("Invalid product data");
                }

                var existingProduct = result.Products.FirstOrDefault(p => p.ProductName == userProduct.ProductName);

                if (existingProduct != null)
                {
                    // If product exists, increase the quantity
                    existingProduct.Quantity += userProduct.Quantity;
                }
                else
                { 
                    // If product does not exist, add it to the cart
                    result.Products.Add(userProduct);
                }
                _context.CartEntity.Update(result);
                await _context.SaveChangesAsync();

                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

        }
    }
}

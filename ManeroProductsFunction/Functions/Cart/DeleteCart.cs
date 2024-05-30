using ManeroProductsFunction.Data.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace ManeroProductsFunction.Functions.Cart
{
    public class DeleteCart
    {
        private readonly ILogger<DeleteCart> _logger;
        private readonly DataContext _context;

        public DeleteCart(ILogger<DeleteCart> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Function("DeleteCart")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "cart/{cartId}")] HttpRequestData req,
            string cartId)
        {
            var response = req.CreateResponse();

            try
            {
                var entity = _context.CartEntity.FirstOrDefault(x => x.Id == cartId);
                if (entity != null)
                {
                    _context.CartEntity.Remove(entity);
                    await _context.SaveChangesAsync();
                    response.StatusCode = System.Net.HttpStatusCode.NoContent;
                }
                else
                {
                    response.StatusCode = System.Net.HttpStatusCode.NotFound;
                    await response.WriteStringAsync("Cart not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting cart with ID {CartId}", cartId);
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                await response.WriteStringAsync("An error occurred while deleting the cart.");
            }

            return response;
        }
    }
}

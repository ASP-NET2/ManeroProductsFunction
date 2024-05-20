using ManeroProductsFunction.Data.Context;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Net;

namespace ManeroProductsFunction.Functions
{
    public class GetAllProducts
    {
        private readonly ILogger<GetAllProducts> _logger;
        private readonly DataContext _context;

        public GetAllProducts(ILogger<GetAllProducts> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Function("GetAllProducts")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var products = await _context.Product.ToListAsync();

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(products);

            return response;
        }
    }
}

using ManeroProductsFunction.Data.Context;
using ManeroProductsFunction.Data.Entitys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;

namespace ManeroProductsFunction.Functions;
public class CreateProduct(ILogger<CreateProduct> logger, DataContext context)
{
    private readonly ILogger<CreateProduct> _logger = logger;
    private readonly DataContext _context = context;

    [Function("CreateProduct")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        var body = await new StreamReader(req.Body).ReadToEndAsync();
        var product = JsonConvert.DeserializeObject<ProductEntity>(body);

        _context.Product.Add(product);
        await _context.SaveChangesAsync();

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(product);

        return response;
    }
}

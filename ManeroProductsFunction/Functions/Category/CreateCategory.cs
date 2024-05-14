using ManeroProductsFunction.Data.Context;
using ManeroProductsFunction.Data.Entitys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ManeroProductsFunction.Functions.Category;

public class CreateCategory(ILogger<CreateCategory> logger, DataContext context)
{
    private readonly ILogger<CreateCategory> _logger = logger;
    private readonly DataContext _context = context;

    [Function("CreateCategory")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        var body = await new StreamReader(req.Body).ReadToEndAsync();
        var category = JsonConvert.DeserializeObject<CategoryEntity>(body);

        _context.Category.Add(category);
        await _context.SaveChangesAsync();

        return new OkObjectResult(category);
    }
}
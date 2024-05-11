using ManeroProductsFunction.Data.Context;
using ManeroProductsFunction.Data.Entitys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ManeroProductsFunction.Functions;

public class SubCategoryFunction(ILogger<SubCategoryFunction> logger, DataContext context)
{
    private readonly ILogger<SubCategoryFunction> _logger = logger;
    private readonly DataContext _context = context;

    [Function("CreateSubCategory")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        var body = await new StreamReader(req.Body).ReadToEndAsync();
        var subcategory = JsonConvert.DeserializeObject<SubCategoryEntity>(body);

        _context.SubCategory.Add(subcategory);
        await _context.SaveChangesAsync();

        return new OkObjectResult(subcategory);
    }
}
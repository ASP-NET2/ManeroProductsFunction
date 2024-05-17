using ManeroProductsFunction.Data.Context;
using ManeroProductsFunction.Data.Entitys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ManeroProductsFunction.Functions.Format;

public class CreateFormat(ILogger<CreateFormat> logger, DataContext context)
{
    private readonly ILogger<CreateFormat> _logger = logger;
    private readonly DataContext _context = context;

    [Function("CreateFormat")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        var body = await new StreamReader(req.Body).ReadToEndAsync();
        var format = JsonConvert.DeserializeObject<FormatEntity>(body);

        _context.Format.Add(format);
        await _context.SaveChangesAsync();

        return new OkObjectResult(format);
    }
}

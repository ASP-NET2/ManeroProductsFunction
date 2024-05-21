using ManeroProductsFunction.Data.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ManeroProductsFunction.Functions.Format;


public class GetFormat(ILogger<GetFormat> logger, DataContext context)
{
    private readonly ILogger<GetFormat> _logger = logger;
    private readonly DataContext _context = context;

    [Function("GetFormat")]
    public async Task<IActionResult> RunGetAll([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        try
        {
            var formats = await _context.Format.ToListAsync();

            if (formats == null || formats.Count == 0)
            {
                _logger.LogInformation("No formats found.");
                return new NoContentResult();
            }

            return new OkObjectResult(formats);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while getting formats: {ex.Message}", ex);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}

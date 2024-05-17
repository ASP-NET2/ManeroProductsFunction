using ManeroProductsFunction.Data.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ManeroProductsFunction.Functions.Format;
public class DeleteFormat(ILogger<DeleteFormat> logger, DataContext context)
{
    private readonly ILogger<DeleteFormat> _logger = logger;
    private readonly DataContext _context = context;


    [Function("DeleteFormat")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "format/{id}")] HttpRequest req, string id)
    {
        _logger.LogInformation("HTTP trigger function processed a delete request.");

        if (string.IsNullOrEmpty(id))
        {
            _logger.LogWarning("DeleteFormat function was called without a format ID.");
            return new BadRequestObjectResult("Format ID must be provided.");
        }

        try
        {
            var format = await _context.Format
                                         .SingleOrDefaultAsync(c => c.Id == id && c.PartitionKey == "Format");
            if (format == null)
            {
                _logger.LogInformation($"Format with ID: {id} not found in partition 'Format'.");
                return new NotFoundResult();
            }

            _context.Format.Remove(format);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Format with ID: {id} has been deleted.");
            return new OkObjectResult($"Format with ID: {id} deleted successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error occurred while trying to delete format with ID: {id} in partition 'Format'. Error: {ex.Message}", ex);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}

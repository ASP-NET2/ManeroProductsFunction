using ManeroProductsFunction.Data.Context;
using ManeroProductsFunction.Data.Entitys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;

namespace ManeroProductsFunction.Functions.Format;

public class UpdateFormat(ILogger<UpdateFormat> logger, DataContext context)
{
    private readonly ILogger<UpdateFormat> _logger = logger;
    private readonly DataContext _context = context;

    [Function("UpdateFormat")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "put")] HttpRequestData req)
    {
        _logger.LogInformation("Processing update request for format.");

        var response = req.CreateResponse();

        try
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            FormatEntity updatedFormat = JsonConvert.DeserializeObject<FormatEntity>(requestBody);

            if (updatedFormat == null)
            {
                _logger.LogWarning("Invalid format data received.");
                response.StatusCode = HttpStatusCode.BadRequest;
                await response.WriteStringAsync("Invalid format data.");
                return response;
            }

            var formatToUpdate = await _context.Format.FindAsync(updatedFormat.Id, updatedFormat.PartitionKey);
            if (formatToUpdate == null)
            {
                _logger.LogWarning($"Format with ID {updatedFormat.Id} and PartitionKey {updatedFormat.PartitionKey} not found.");
                response.StatusCode = HttpStatusCode.NotFound;
                return response;
            }

            formatToUpdate.FormatName = updatedFormat.FormatName;

            _context.Format.Update(formatToUpdate);
            await _context.SaveChangesAsync();

            response.StatusCode = HttpStatusCode.OK;
            await response.WriteAsJsonAsync(formatToUpdate);
            return response;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error parsing request body.");
            response.StatusCode = HttpStatusCode.BadRequest;
            await response.WriteStringAsync("Error in request format or data.");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred.");
            response.StatusCode = HttpStatusCode.InternalServerError;
            await response.WriteStringAsync("Unexpected error occurred.");
            return response;
        }
    }
}

using System;
using System.IO;
using System.Threading.Tasks;
using ManeroProductsFunction.Data.Context;
using ManeroProductsFunction.Data.Entitys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ManeroProductsFunction.Functions.Format
{
    public class CreateFormat
    {
        private readonly ILogger<CreateFormat> _logger;
        private readonly DataContext _context;

        public CreateFormat(ILogger<CreateFormat> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Function("CreateFormat")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            try
            {
                var body = await new StreamReader(req.Body).ReadToEndAsync();
                var format = JsonConvert.DeserializeObject<FormatEntity>(body);

                _context.Format.Add(format);
                await _context.SaveChangesAsync();

                return new OkObjectResult(format);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating format: {ex.Message}");
                return new BadRequestObjectResult(new { error = ex.Message });
            }
        }
    }
}

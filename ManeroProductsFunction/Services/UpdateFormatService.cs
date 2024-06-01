using ManeroProductsFunction.Data.Context;
using ManeroProductsFunction.Data.Entitys;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ManeroProductsFunction.Services
{
    public class UpdateFormatService
    {
        private readonly DataContext _context;
        private readonly ILogger<UpdateFormatService> _logger;

        public UpdateFormatService(DataContext context, ILogger<UpdateFormatService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<FormatEntity?> UpdateFormatAsync(FormatEntity updatedFormat)
        {
            if (updatedFormat == null)
            {
                _logger.LogWarning("Invalid format data received.");
                return null;
            }

            var formatToUpdate = await _context.Format.FindAsync(updatedFormat.Id, updatedFormat.PartitionKey);
            if (formatToUpdate == null)
            {
                _logger.LogWarning($"Format with ID {updatedFormat.Id} and PartitionKey {updatedFormat.PartitionKey} not found.");
                return null;
            }

            formatToUpdate.FormatName = updatedFormat.FormatName;

            _context.Format.Update(formatToUpdate);
            await _context.SaveChangesAsync();

            return formatToUpdate;
        }
    }
}

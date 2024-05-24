using System;
using ManeroProductsFunction.Data.Context;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ManeroProductsFunction.Functions.Cart
{
    public class CleanUpOldCarts(ILogger<CleanUpOldCarts> logger, DataContext context)
    {
        private readonly ILogger<CleanUpOldCarts> _logger = logger;
        private readonly DataContext _context = context;

        [Function("CleanUpOldCarts")]
        public async Task Run([TimerTrigger("0 0 0 * * *")] TimerInfo myTimer)
        {
            try
            {
                var cutOfTime = DateTime.Now.AddHours(-24);
                var oldCarts = await _context.CartEntity
                    .Where(c => c.CreatedDate < cutOfTime)
                    .ToListAsync();

                if (oldCarts.Any())
                {
                    _context.CartEntity.RemoveRange(oldCarts);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"{oldCarts.Count} Old carts where Deleted");
                }
                else
                {
                    _logger.LogInformation("No old Carts found");
                }

            }
            catch (Exception ex) 
            {
                _logger.LogError($"Error occurred during cleanup: {ex.Message}");
            }
        }
    }
}

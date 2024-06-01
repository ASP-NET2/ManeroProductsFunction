using System;
using System.Threading.Tasks;
using ManeroProductsFunction.Data.Context;
using ManeroProductsFunction.Data.Entitys;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Test.FormatTest
{
    public class UpdateFormatServiceTests
    {
        private readonly Mock<ILogger<UpdateFormatService>> _loggerMock;
        private readonly DbContextOptions<DataContext> _options;
        private DataContext _context;
        private UpdateFormatService _service;

        public UpdateFormatServiceTests()
        {
            _loggerMock = new Mock<ILogger<UpdateFormatService>>();
            _options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            InitializeContext();
        }

        private void InitializeContext()
        {
            _context = new DataContext(_options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            _service = new UpdateFormatService(_loggerMock.Object, _context);
        }

        private async Task ClearDatabaseAsync()
        {
            _context.Format.RemoveRange(_context.Format);
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateFormatAsync_ValidFormatId_ShouldReturnUpdatedFormat()
        {
            // Arrange
            await ClearDatabaseAsync();
            InitializeContext();
            var format = new FormatEntity
            {
                Id = "1",
                FormatName = "Original Format"
            };
            _context.Format.Add(format);
            await _context.SaveChangesAsync();

            var updatedFormat = new FormatEntity
            {
                Id = "1",
                FormatName = "Updated Format"
            };

            // Act
            var result = await _service.UpdateFormatAsync(updatedFormat);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Format", result.FormatName);
        }

        [Fact]
        public async Task UpdateFormatAsync_InvalidFormatId_ShouldReturnNull()
        {
            // Arrange
            await ClearDatabaseAsync();
            InitializeContext();
            var updatedFormat = new FormatEntity
            {
                Id = "2",
                FormatName = "Non-Existent Format"
            };

            // Act
            var result = await _service.UpdateFormatAsync(updatedFormat);

            // Assert
            Assert.Null(result);
        }
    }

    public class UpdateFormatService
    {
        private readonly ILogger<UpdateFormatService> _logger;
        private readonly DataContext _context;

        public UpdateFormatService(ILogger<UpdateFormatService> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<FormatEntity?> UpdateFormatAsync(FormatEntity updatedFormat)
        {
            try
            {
                var formatToUpdate = await _context.Format
                    .FirstOrDefaultAsync(f => f.Id == updatedFormat.Id && f.PartitionKey == updatedFormat.PartitionKey);

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred.");
                throw;
            }
        }
    }
}

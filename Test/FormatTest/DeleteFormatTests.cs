using System;
using System.Threading.Tasks;
using ManeroProductsFunction.Data.Context;
using ManeroProductsFunction.Data.Entitys;
using ManeroProductsFunction.Functions.Format;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Test.FormatTest
{
    public class DeleteFormatTests
    {
        private readonly Mock<ILogger<DeleteFormat>> _mockLogger;
        private readonly DbContextOptions<DataContext> _options;
        private DataContext _context;
        private DeleteFormat _function;

        public DeleteFormatTests()
        {
            _mockLogger = new Mock<ILogger<DeleteFormat>>();
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
            _function = new DeleteFormat(_mockLogger.Object, _context);
        }

        private async Task ClearDatabaseAsync()
        {
            _context.Format.RemoveRange(_context.Format);
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task Run_ReturnsNotFound_WhenFormatDoesNotExist()
        {
            // Arrange
            await ClearDatabaseAsync();
            InitializeContext();
            var req = new Mock<HttpRequest>();
            string formatId = Guid.NewGuid().ToString();

            // Act
            var result = await _function.Run(req.Object, formatId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Run_ReturnsBadRequest_WhenIdIsNull()
        {
            // Arrange
            InitializeContext();
            var req = new Mock<HttpRequest>();
            string formatId = null;

            // Act
            var result = await _function.Run(req.Object, formatId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Format ID must be provided.", badRequestResult.Value);
        }

        [Fact]
        public async Task Run_ReturnsOk_WhenFormatIsDeleted()
        {
            // Arrange
            await ClearDatabaseAsync();
            InitializeContext();
            var format = new FormatEntity
            {
                Id = Guid.NewGuid().ToString(),
                PartitionKey = "Format",
                FormatName = "Test Format"
            };
            _context.Format.Add(format);
            await _context.SaveChangesAsync();

            var req = new Mock<HttpRequest>();
            string formatId = format.Id;

            // Act
            var result = await _function.Run(req.Object, formatId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal($"Format with ID: {formatId} deleted successfully.", okResult.Value);
        }
    }
}

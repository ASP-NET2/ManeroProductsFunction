using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManeroProductsFunction.Data.Context;
using ManeroProductsFunction.Data.Entitys;
using ManeroProductsFunction.Functions.Category;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Test.ProductTest
{
    public class DeleteProductTests
    {
        private readonly Mock<ILogger<DeletePRoduct>> _mockLogger;
        private readonly DbContextOptions<DataContext> _options;
        private DataContext _context;
        private DeletePRoduct _function;

        public DeleteProductTests()
        {
            _mockLogger = new Mock<ILogger<DeletePRoduct>>();
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
            _function = new DeletePRoduct(_mockLogger.Object, _context);
        }

        private async Task ClearDatabaseAsync()
        {
            _context.Product.RemoveRange(_context.Product);
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task Run_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            await ClearDatabaseAsync();
            InitializeContext();
            var req = new Mock<HttpRequest>();
            string productId = Guid.NewGuid().ToString();

            // Act
            var result = await _function.Run(req.Object, productId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Run_ReturnsBadRequest_WhenIdIsNull()
        {
            // Arrange
            InitializeContext();
            var req = new Mock<HttpRequest>();
            string productId = null;

            // Act
            var result = await _function.Run(req.Object, productId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Product ID must be provided.", badRequestResult.Value);
        }

        [Fact]
        public async Task Run_ReturnsOk_WhenProductIsDeleted()
        {
            // Arrange
            await ClearDatabaseAsync();
            InitializeContext();
            var product = new ProductEntity
            {
                Id = Guid.NewGuid().ToString(),
                PartitionKey = "Products",
                Title = "Test Product",
                Author = "Test Author"
            };
            _context.Product.Add(product);
            await _context.SaveChangesAsync();

            var req = new Mock<HttpRequest>();
            string productId = product.Id;

            // Act
            var result = await _function.Run(req.Object, productId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal($"Product with ID: {productId} deleted successfully.", okResult.Value);
        }
    }
}

using System;
using System.Threading.Tasks;
using ManeroProductsFunction.Data.Context;
using ManeroProductsFunction.Data.Entitys;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Test.ProductTest
{
    public class UpdateProductServiceTests
    {
        private readonly Mock<ILogger<UpdateProductService>> _loggerMock;
        private readonly DbContextOptions<DataContext> _options;
        private DataContext _context;
        private UpdateProductService _service;

        public UpdateProductServiceTests()
        {
            _loggerMock = new Mock<ILogger<UpdateProductService>>();
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
            _service = new UpdateProductService(_loggerMock.Object, _context);
        }

        private async Task ClearDatabaseAsync()
        {
            _context.Product.RemoveRange(_context.Product);
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateProductAsync_ValidProductId_ShouldReturnUpdatedProduct()
        {
            // Arrange
            await ClearDatabaseAsync();
            InitializeContext();
            var product = new ProductEntity
            {
                Id = "1",
                PartitionKey = "Products",
                Title = "Original Title",
                Author = "Original Author"
            };
            _context.Product.Add(product);
            await _context.SaveChangesAsync();

            var updatedProduct = new ProductEntity
            {
                Id = "1",
                PartitionKey = "Products",
                Title = "Updated Title",
                Author = "Updated Author"
            };

            // Act
            var result = await _service.UpdateProductAsync(updatedProduct);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Title", result.Title);
            Assert.Equal("Updated Author", result.Author);
        }

        [Fact]
        public async Task UpdateProductAsync_InvalidProductId_ShouldReturnNull()
        {
            // Arrange
            await ClearDatabaseAsync();
            InitializeContext();
            var updatedProduct = new ProductEntity
            {
                Id = "2",
                PartitionKey = "Products",
                Title = "Non-Existent Product",
                Author = "Non-Existent Author"
            };

            // Act
            var result = await _service.UpdateProductAsync(updatedProduct);

            // Assert
            Assert.Null(result);
        }
    }
}

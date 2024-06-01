using System;
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

namespace Test.CategoryTest
{
    public class DeleteCategoryTests
    {
        private readonly Mock<ILogger<DeleteCategory>> _mockLogger;
        private readonly DbContextOptions<DataContext> _options;
        private DataContext _context;
        private DeleteCategory _function;

        public DeleteCategoryTests()
        {
            _mockLogger = new Mock<ILogger<DeleteCategory>>();
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
            _function = new DeleteCategory(_mockLogger.Object, _context);
        }

        private async Task ClearDatabaseAsync()
        {
            _context.Category.RemoveRange(_context.Category);
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task Run_ReturnsNotFound_WhenCategoryDoesNotExist()
        {
            // Arrange
            await ClearDatabaseAsync();
            InitializeContext();
            var req = new Mock<HttpRequest>();
            string categoryId = Guid.NewGuid().ToString();

            // Act
            var result = await _function.Run(req.Object, categoryId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Run_ReturnsBadRequest_WhenIdIsNull()
        {
            // Arrange
            InitializeContext();
            var req = new Mock<HttpRequest>();
            string categoryId = null;

            // Act
            var result = await _function.Run(req.Object, categoryId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Category ID must be provided.", badRequestResult.Value);
        }

        [Fact]
        public async Task Run_ReturnsOk_WhenCategoryIsDeleted()
        {
            // Arrange
            await ClearDatabaseAsync();
            InitializeContext();
            var category = new CategoryEntity
            {
                Id = Guid.NewGuid().ToString(),
                PartitionKey = "Category",
                CategoryName = "Test Category",
                ImageLink = "http://example.com/image.jpg" // Add required field
            };
            _context.Category.Add(category);
            await _context.SaveChangesAsync();

            var req = new Mock<HttpRequest>();
            string categoryId = category.Id;

            // Act
            var result = await _function.Run(req.Object, categoryId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal($"Category with ID: {categoryId} deleted successfully.", okResult.Value);
        }
    }
}

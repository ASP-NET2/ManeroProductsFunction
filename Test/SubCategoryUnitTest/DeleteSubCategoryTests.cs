using System;
using System.Threading.Tasks;
using ManeroProductsFunction.Data.Context;
using ManeroProductsFunction.Data.Entitys;
using ManeroProductsFunction.Functions.SubCategory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Test.SubCategoryTest
{
    public class DeleteSubCategoryTests
    {
        private readonly Mock<ILogger<DeleteSubCategory>> _mockLogger;
        private readonly DbContextOptions<DataContext> _options;
        private DataContext _context;
        private DeleteSubCategory _function;

        public DeleteSubCategoryTests()
        {
            _mockLogger = new Mock<ILogger<DeleteSubCategory>>();
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
            _function = new DeleteSubCategory(_mockLogger.Object, _context);
        }

        private async Task ClearDatabaseAsync()
        {
            _context.SubCategory.RemoveRange(_context.SubCategory);
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task Run_ReturnsNotFound_WhenSubCategoryDoesNotExist()
        {
            // Arrange
            await ClearDatabaseAsync();
            InitializeContext();
            var req = new Mock<HttpRequest>();
            string subCategoryId = Guid.NewGuid().ToString();

            // Act
            var result = await _function.Run(req.Object, subCategoryId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Run_ReturnsBadRequest_WhenIdIsNull()
        {
            // Arrange
            InitializeContext();
            var req = new Mock<HttpRequest>();
            string subCategoryId = null;

            // Act
            var result = await _function.Run(req.Object, subCategoryId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("SubCategory ID must be provided.", badRequestResult.Value);
        }

        [Fact]
        public async Task Run_ReturnsOk_WhenSubCategoryIsDeleted()
        {
            // Arrange
            await ClearDatabaseAsync();
            InitializeContext();
            var subCategory = new SubCategoryEntity
            {
                Id = Guid.NewGuid().ToString(),
                PartitionKey = "SubCategory",
                SubCategoryName = "Test SubCategory",
                ImageLink = "http://example.com/image.jpg" // Add required field
            };
            _context.SubCategory.Add(subCategory);
            await _context.SaveChangesAsync();

            var req = new Mock<HttpRequest>();
            string subCategoryId = subCategory.Id;

            // Act
            var result = await _function.Run(req.Object, subCategoryId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal($"SubCategory with ID: {subCategoryId} deleted successfully.", okResult.Value);
        }
    }
}

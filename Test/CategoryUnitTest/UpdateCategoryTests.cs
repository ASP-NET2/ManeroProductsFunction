using System;
using System.Threading.Tasks;
using ManeroProductsFunction.Data.Context;
using ManeroProductsFunction.Data.Entitys;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Test.CategoryTest
{
    public class UpdateCategoryServiceTests
    {
        private readonly Mock<ILogger<UpdateCategoryService>> _loggerMock;
        private readonly DbContextOptions<DataContext> _options;
        private DataContext _context;
        private UpdateCategoryService _service;

        public UpdateCategoryServiceTests()
        {
            _loggerMock = new Mock<ILogger<UpdateCategoryService>>();
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
            _service = new UpdateCategoryService(_loggerMock.Object, _context);
        }

        private async Task ClearDatabaseAsync()
        {
            _context.Category.RemoveRange(_context.Category);
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateCategoryAsync_ValidCategoryId_ShouldReturnUpdatedCategory()
        {
            // Arrange
            await ClearDatabaseAsync();
            InitializeContext();
            var category = new CategoryEntity
            {
                Id = "1",
                PartitionKey = "Category",
                CategoryName = "Original Category",
                ImageLink = "original.jpg"
            };
            _context.Category.Add(category);
            await _context.SaveChangesAsync();

            var updatedCategory = new CategoryEntity
            {
                Id = "1",
                PartitionKey = "Category",
                CategoryName = "Updated Category",
                ImageLink = "updated.jpg"
            };

            // Act
            var result = await _service.UpdateCategoryAsync(updatedCategory);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Category", result.CategoryName);
            Assert.Equal("updated.jpg", result.ImageLink);
        }

        [Fact]
        public async Task UpdateCategoryAsync_InvalidCategoryId_ShouldReturnNull()
        {
            // Arrange
            await ClearDatabaseAsync();
            InitializeContext();
            var updatedCategory = new CategoryEntity
            {
                Id = "2",
                PartitionKey = "Category",
                CategoryName = "Non-Existent Category",
                ImageLink = "nonexistent.jpg"
            };

            // Act
            var result = await _service.UpdateCategoryAsync(updatedCategory);

            // Assert
            Assert.Null(result);
        }
    }
}

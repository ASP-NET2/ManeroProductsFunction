using System;
using System.Threading.Tasks;
using ManeroProductsFunction.Data.Context;
using ManeroProductsFunction.Data.Entitys;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Test.SubCategoryTest
{
    public class UpdateSubCategoryServiceTests
    {
        private readonly Mock<ILogger<UpdateSubCategoryService>> _loggerMock;
        private readonly DbContextOptions<DataContext> _options;
        private DataContext _context;
        private UpdateSubCategoryService _service;

        public UpdateSubCategoryServiceTests()
        {
            _loggerMock = new Mock<ILogger<UpdateSubCategoryService>>();
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
            _service = new UpdateSubCategoryService(_loggerMock.Object, _context);
        }

        private async Task ClearDatabaseAsync()
        {
            _context.SubCategory.RemoveRange(_context.SubCategory);
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateSubCategoryAsync_ValidSubCategoryId_ShouldReturnUpdatedSubCategory()
        {
            // Arrange
            await ClearDatabaseAsync();
            InitializeContext();
            var subCategory = new SubCategoryEntity
            {
                Id = "1",
                PartitionKey = "SubCategory",
                SubCategoryName = "Original SubCategory",
                ImageLink = "original.jpg"
            };
            _context.SubCategory.Add(subCategory);
            await _context.SaveChangesAsync();

            var updatedSubCategory = new SubCategoryEntity
            {
                Id = "1",
                PartitionKey = "SubCategory",
                SubCategoryName = "Updated SubCategory",
                ImageLink = "updated.jpg"
            };

            // Act
            var result = await _service.UpdateSubCategoryAsync(updatedSubCategory);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated SubCategory", result.SubCategoryName);
            Assert.Equal("updated.jpg", result.ImageLink);
        }

        [Fact]
        public async Task UpdateSubCategoryAsync_InvalidSubCategoryId_ShouldReturnNull()
        {
            // Arrange
            await ClearDatabaseAsync();
            InitializeContext();
            var updatedSubCategory = new SubCategoryEntity
            {
                Id = "2",
                PartitionKey = "SubCategory",
                SubCategoryName = "Non-Existent SubCategory",
                ImageLink = "nonexistent.jpg"
            };

            // Act
            var result = await _service.UpdateSubCategoryAsync(updatedSubCategory);

            // Assert
            Assert.Null(result);
        }
    }
}

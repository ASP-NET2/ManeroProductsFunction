using ManeroProductsFunction.Data.Context;
using ManeroProductsFunction.Data.Entitys;
using ManeroProductsFunction.Functions.Category;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Test.CategoryUnitTest
{
    public class GetCategoryTests : IDisposable
    {
        private readonly Mock<ILogger<GetCategory>> _mockLogger;
        private readonly DataContext _context;
        private readonly GetCategory _function;
        private bool _disposed;

        public GetCategoryTests()
        {
            _mockLogger = new Mock<ILogger<GetCategory>>();

            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new DataContext(options);

            _function = new GetCategory(_mockLogger.Object, _context);
        }

        private void SeedDatabase()
        {
            var categories = new List<CategoryEntity>
            {
                new CategoryEntity { Id = "1", CategoryName = "Category1", ImageLink = "link1" },
                new CategoryEntity { Id = "2", CategoryName = "Category2", ImageLink = "link2" }
            };
            _context.Category.AddRange(categories);
            _context.SaveChanges();
        }

        [Fact]
        public async Task RunGetAll_ReturnsOkObjectResult_WhenCategoriesExist()
        {
            // Arrange
            SeedDatabase();
            var request = new Mock<HttpRequest>();

            // Act
            var result = await _function.RunGetAll(request.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<CategoryEntity>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task RunGetAll_ReturnsNoContentResult_WhenNoCategoriesExist()
        {
            // Arrange
            var request = new Mock<HttpRequest>();

            // Act
            var result = await _function.RunGetAll(request.Object);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

       

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Database.EnsureDeleted();
                    _context.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

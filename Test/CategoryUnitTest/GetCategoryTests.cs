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
        private readonly Mock<ILogger<GetCategory>> _getMockLogger;
        private readonly DataContext _getContext;
        private readonly GetCategory _getFunction;
        private bool _disposed;

        public GetCategoryTests()
        {
            _getMockLogger = new Mock<ILogger<GetCategory>>();

            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "GetTestDatabase")
                .Options;
            _getContext = new DataContext(options);

            _getFunction = new GetCategory(_getMockLogger.Object, _getContext);
        }

        private void SeedDatabase()
        {
            var categories = new List<CategoryEntity>
            {
                new CategoryEntity { Id = "1", CategoryName = "Category1", ImageLink = "link1" },
                new CategoryEntity { Id = "2", CategoryName = "Category2", ImageLink = "link2" }
            };
            _getContext.Category.AddRange(categories);
            _getContext.SaveChanges();
        }

        [Fact]
        public async Task RunGetAll_ReturnsOkObjectResult_WhenCategoriesExist()
        {
            // Arrange
            SeedDatabase();
            var request = new Mock<HttpRequest>();

            // Act
            var result = await _getFunction.RunGetAll(request.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult);
            var returnValue = Assert.IsType<List<CategoryEntity>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task RunGetAll_ReturnsNoContentResult_WhenNoCategoriesExist()
        {
            // Arrange
            var request = new Mock<HttpRequest>();

            // Act
            var result = await _getFunction.RunGetAll(request.Object);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _getContext.Database.EnsureDeleted();
                    _getContext.Dispose();
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

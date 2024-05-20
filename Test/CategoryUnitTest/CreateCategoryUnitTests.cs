using ManeroProductsFunction.Data.Context;
using ManeroProductsFunction.Data.Entitys;
using ManeroProductsFunction.Functions.Category;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Test.CategoryUnitTest
{
    public class CreateCategoryTests : IDisposable
    {
        private readonly Mock<ILogger<CreateCategory>> _createMockLogger;
        private readonly DataContext _createContext;
        private readonly CreateCategory _createFunction;
        private bool _disposed;

        public CreateCategoryTests()
        {
            _createMockLogger = new Mock<ILogger<CreateCategory>>();

            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "CreateTestDatabase")
                .Options;
            _createContext = new DataContext(options);

            _createFunction = new CreateCategory(_createMockLogger.Object, _createContext);
        }

        [Fact]
        public async Task Run_CreatesCategory_ReturnsOkObjectResult()
        {
            // Arrange
            var category = new CategoryEntity
            {
                CategoryName = "New Category",
                ImageLink = "http://example.com/image.jpg"
            };
            var jsonCategory = JsonConvert.SerializeObject(category);
            var request = new Mock<HttpRequest>();
            request.Setup(r => r.Body).Returns(new MemoryStream(Encoding.UTF8.GetBytes(jsonCategory)));

            // Act
            var result = await _createFunction.Run(request.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<CategoryEntity>(okResult.Value);
            Assert.Equal("New Category", returnValue.CategoryName);
            Assert.Equal("http://example.com/image.jpg", returnValue.ImageLink);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _createContext.Database.EnsureDeleted();
                    _createContext.Dispose();
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

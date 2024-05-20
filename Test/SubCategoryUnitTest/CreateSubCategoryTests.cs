using ManeroProductsFunction.Data.Context;
using ManeroProductsFunction.Data.Entitys;
using ManeroProductsFunction.Functions.SubCategory;
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

namespace Test.SubCategoryUnitTest
{
    public class CreateSubCategoryTests : IDisposable
    {
        private readonly Mock<ILogger<CreateSubCategory>> _createMockLogger;
        private readonly DataContext _createContext;
        private readonly CreateSubCategory _createFunction;
        private bool _disposed;

        public CreateSubCategoryTests()
        {
            _createMockLogger = new Mock<ILogger<CreateSubCategory>>();

            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "CreateTestDatabase")
                .Options;
            _createContext = new DataContext(options);

            _createFunction = new CreateSubCategory(_createMockLogger.Object, _createContext);
        }

        [Fact]
        public async Task Run_CreatesSubCategory_ReturnsOkObjectResult()
        {
            // Arrange
            var subCategory = new SubCategoryEntity
            {
                SubCategoryName = "New SubCategory",
                ImageLink = "http://example.com/image.jpg"
            };
            var jsonSubCategory = JsonConvert.SerializeObject(subCategory);
            var request = new Mock<HttpRequest>();
            request.Setup(r => r.Body).Returns(new MemoryStream(Encoding.UTF8.GetBytes(jsonSubCategory)));

            // Act
            var result = await _createFunction.Run(request.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<SubCategoryEntity>(okResult.Value);
            Assert.Equal("New SubCategory", returnValue.SubCategoryName);
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

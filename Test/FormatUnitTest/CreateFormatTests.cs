using ManeroProductsFunction.Data.Context;
using ManeroProductsFunction.Data.Entitys;
using ManeroProductsFunction.Functions.Format;
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

namespace Test.FormatUnitTest
{
    public class CreateFormatTests : IDisposable
    {
        private readonly Mock<ILogger<CreateFormat>> _createMockLogger;
        private readonly DataContext _createContext;
        private readonly CreateFormat _createFunction;
        private bool _disposed;

        public CreateFormatTests()
        {
            _createMockLogger = new Mock<ILogger<CreateFormat>>();

            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "CreateTestDatabase")
                .Options;
            _createContext = new DataContext(options);

            _createFunction = new CreateFormat(_createMockLogger.Object, _createContext);
        }

        [Fact]
        public async Task Run_CreatesFormat_ReturnsOkObjectResult()
        {
            // Arrange
            var format = new FormatEntity
            {
                FormatName = "New Format"
            };
            var jsonFormat = JsonConvert.SerializeObject(format);
            var request = new Mock<HttpRequest>();
            request.Setup(r => r.Body).Returns(new MemoryStream(Encoding.UTF8.GetBytes(jsonFormat)));

            // Act
            var result = await _createFunction.Run(request.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<FormatEntity>(okResult.Value);
            Assert.Equal("New Format", returnValue.FormatName);
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

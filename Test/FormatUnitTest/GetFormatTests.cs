using ManeroProductsFunction.Data.Context;
using ManeroProductsFunction.Data.Entitys;
using ManeroProductsFunction.Functions.Format;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Test.FormatUnitTest
{
    public class GetFormatTests : IDisposable
    {
        private readonly Mock<ILogger<GetFormat>> _mockLogger;
        private readonly DataContext _context;
        private readonly GetFormat _function;
        private bool _disposed;

        public GetFormatTests()
        {
            _mockLogger = new Mock<ILogger<GetFormat>>();

            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new DataContext(options);

            _function = new GetFormat(_mockLogger.Object, _context);
        }

        private void SeedDatabase()
        {
            var formats = new List<FormatEntity>
            {
                new FormatEntity { Id = "1", FormatName = "Format1" },
                new FormatEntity { Id = "2", FormatName = "Format2"}
            };
            _context.Format.AddRange(formats);
            _context.SaveChanges();
        }

        [Fact]
        public async Task RunGetAll_ReturnsOkObjectResult_WhenFormatsExist()
        {
            // Arrange
            SeedDatabase();
            var request = new Mock<HttpRequest>();

            // Act
            var result = await _function.RunGetAll(request.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<FormatEntity>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task RunGetAll_ReturnsNoContentResult_WhenNoFormatsExist()
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

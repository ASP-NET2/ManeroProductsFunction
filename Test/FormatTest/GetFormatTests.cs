using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ManeroProductsFunction.Data.Context;
using ManeroProductsFunction.Data.Entitys;
using ManeroProductsFunction.Functions.Format;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Test.FormatUnitTest
{
    public class GetFormatTests
    {
        private readonly Mock<ILogger<GetFormat>> _logger;
        private readonly DbContextOptions<DataContext> _options;
        private DataContext _testContext;
        private GetFormat _testFunction;

        public GetFormatTests()
        {
            _logger = new Mock<ILogger<GetFormat>>();
            _options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            InitializeContext();
        }

        private void InitializeContext()
        {
            _testContext = new DataContext(_options);
            _testContext.Database.EnsureDeleted();
            _testContext.Database.EnsureCreated();
            _testFunction = new GetFormat(_logger.Object, _testContext);
        }

        [Fact]
        public async Task RunGetAll_ReturnsNoContent_WhenNoFormatsExist()
        {
            // Arrange
            InitializeContext();
            var req = new Mock<HttpRequest>();

            // Act
            var result = await _testFunction.RunGetAll(req.Object);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task RunGetAll_ReturnsOk_WithListOfFormats()
        {
            // Arrange
            InitializeContext();
            var req = new Mock<HttpRequest>();

            var formats = new List<FormatEntity>
            {
                new FormatEntity { Id = Guid.NewGuid().ToString(), FormatName = "Format1" },
                new FormatEntity { Id = Guid.NewGuid().ToString(), FormatName = "Format2" }
            };

            _testContext.Format.AddRange(formats);
            await _testContext.SaveChangesAsync();

            // Act
            var result = await _testFunction.RunGetAll(req.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<FormatEntity>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }
    }
}

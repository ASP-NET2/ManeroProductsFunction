using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ManeroProductsFunction.Data.Context;
using ManeroProductsFunction.Data.Entitys;
using ManeroProductsFunction.Functions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Test.ProductTest
{
    public class GetAllProductsTests
    {
        private readonly Mock<ILogger<GetAllProducts>> _logger;
        private readonly DbContextOptions<DataContext> _options;
        private DataContext _testContext;
        private GetAllProducts _testFunction;

        public GetAllProductsTests()
        {
            _logger = new Mock<ILogger<GetAllProducts>>();
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
            _testFunction = new GetAllProducts(_logger.Object, _testContext);
        }

        [Fact]
        public async Task RunGetAll_ReturnsNoContent_WhenNoProductsExist()
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
        public async Task RunGetAll_ReturnsOk_WithListOfProducts()
        {
            // Arrange
            InitializeContext();
            var req = new Mock<HttpRequest>();

            var products = new List<ProductEntity>
            {
                new ProductEntity { Id = Guid.NewGuid().ToString(), Title = "Product1", Author = "Author1" },
                new ProductEntity { Id = Guid.NewGuid().ToString(), Title = "Product2", Author = "Author2" }
            };

            _testContext.Product.AddRange(products);
            await _testContext.SaveChangesAsync();

            // Act
            var result = await _testFunction.RunGetAll(req.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<ProductEntity>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }
    }
}

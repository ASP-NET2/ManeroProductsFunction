using System;
using System.IO;
using System.Threading.Tasks;
using ManeroProductsFunction.Data.Context;
using ManeroProductsFunction.Data.Entitys;
using ManeroProductsFunction.Data.Functions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Test.CartTest
{
    public class CreateUserCartTests
    {
        private readonly Mock<ILogger<CreateUserCart>> _mockLogger;
        private readonly DbContextOptions<DataContext> _dbContextOptions;

        public CreateUserCartTests()
        {
            _mockLogger = new Mock<ILogger<CreateUserCart>>();
            _dbContextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
        }

        private DataContext GetInMemoryContext()
        {
            var context = new DataContext(_dbContextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        public async Task CreateUserCart_Returns_OkObjectResult()
        {
            // Arrange
            var context = GetInMemoryContext();
            var function = new CreateUserCart(_mockLogger.Object, context);

            var userCart = new CartEntity
            {
                PartitionKey = "Cart",
                Cart = "Cart",
                CreatedDate = DateTime.Now,
                Products = new System.Collections.Generic.List<CartProductEntity>
                {
                    new CartProductEntity { ProductName = "Test Product", Quantity = 1, Price = 10 }
                }
            };

            var requestBody = JsonConvert.SerializeObject(userCart);
            var request = new Mock<HttpRequest>();
            request.Setup(r => r.Body).Returns(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(requestBody)));

            // Act
            var response = await function.Run(request.Object);

            // Assert
            var result = Assert.IsType<OkObjectResult>(response);
            var createdCart = Assert.IsType<CartEntity>(result.Value);

            Assert.Equal("Cart", createdCart.PartitionKey);
            Assert.Equal("Cart", createdCart.Cart);
            Assert.Single(createdCart.Products);
            Assert.Equal(1, createdCart.Products[0].Quantity);
            Assert.Equal("Test Product", createdCart.Products[0].ProductName);
            Assert.Equal(10, createdCart.Products[0].Price);
        }

        [Fact]
        public async Task CreateUserCart_Returns_BadRequest_On_Invalid_Data()
        {
            // Arrange
            var context = GetInMemoryContext();
            var function = new CreateUserCart(_mockLogger.Object, context);

            var requestBody = "invalid data";
            var request = new Mock<HttpRequest>();
            request.Setup(r => r.Body).Returns(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(requestBody)));

            // Act
            var response = await function.Run(request.Object);

            // Assert
            Assert.IsType<BadRequestResult>(response);
        }
    }
}

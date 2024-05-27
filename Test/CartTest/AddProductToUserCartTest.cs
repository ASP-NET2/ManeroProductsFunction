using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ManeroProductsFunction.Data.Context;
using ManeroProductsFunction.Data.Entitys;
using ManeroProductsFunction.Functions.Cart;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Test.CartTest
{
    public class AddproductToUserCartTests
    {
        private readonly Mock<ILogger<AddproductToUserCart>> _mockLogger;
        private readonly DbContextOptions<DataContext> _dbContextOptions;

        public AddproductToUserCartTests()
        {
            _mockLogger = new Mock<ILogger<AddproductToUserCart>>();
            _dbContextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        private DataContext CreateContext()
        {
            var context = new DataContext(_dbContextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        public async Task AddProductToUserCart_ProductAdded_ReturnsOkObjectResult()
        {
            // Arrange
            var context = CreateContext();
            var function = new AddproductToUserCart(_mockLogger.Object, context);

            var cart = new CartEntity
            {
                Id = Guid.NewGuid().ToString(),
                PartitionKey = "Cart",
                Cart = "Cart",
                CreatedDate = DateTime.Now,
                Products = new List<CartProductEntity>
                {
                    new CartProductEntity { ProductId = Guid.NewGuid().ToString(), ProductName = "Existing Product", Quantity = 1, Price = 10 }
                }
            };
            context.CartEntity.Add(cart);
            await context.SaveChangesAsync();

            var newProduct = new CartProductEntity { ProductName = "New Product", Quantity = 2, Price = 20 };
            var requestBody = JsonConvert.SerializeObject(newProduct);
            var request = new Mock<HttpRequest>();
            request.Setup(r => r.Body).Returns(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(requestBody)));

            // Act
            var response = await function.Run(request.Object, cart.Id);

            // Assert
            var result = Assert.IsType<OkObjectResult>(response);
            var updatedCart = Assert.IsType<CartEntity>(result.Value);
            Assert.Equal(2, updatedCart.Products.Count);
            Assert.Contains(updatedCart.Products, p => p.ProductName == "New Product" && p.Quantity == 2);
        }

        [Fact]
        public async Task AddProductToUserCart_ProductQuantityIncreased_ReturnsOkObjectResult()
        {
            // Arrange
            var context = CreateContext();
            var function = new AddproductToUserCart(_mockLogger.Object, context);

            var cart = new CartEntity
            {
                Id = Guid.NewGuid().ToString(),
                PartitionKey = "Cart",
                Cart = "Cart",
                CreatedDate = DateTime.Now,
                Products = new List<CartProductEntity>
                {
                    new CartProductEntity { ProductId = Guid.NewGuid().ToString(), ProductName = "Existing Product", Quantity = 1, Price = 10 }
                }
            };
            context.CartEntity.Add(cart);
            await context.SaveChangesAsync();

            var existingProduct = new CartProductEntity { ProductId = cart.Products.First().ProductId, ProductName = "Existing Product", Quantity = 2, Price = 10 };
            var requestBody = JsonConvert.SerializeObject(existingProduct);
            var request = new Mock<HttpRequest>();
            request.Setup(r => r.Body).Returns(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(requestBody)));

            // Act
            var response = await function.Run(request.Object, cart.Id);

            // Assert
            var result = Assert.IsType<OkObjectResult>(response);
            var updatedCart = Assert.IsType<CartEntity>(result.Value);
            var product = updatedCart.Products.First(p => p.ProductName == "Existing Product");
            Assert.Equal(3, product.Quantity);
        }

        [Fact]
        public async Task AddProductToUserCart_CartNotFound_ReturnsNotFoundObjectResult()
        {
            // Arrange
            var context = CreateContext();
            var function = new AddproductToUserCart(_mockLogger.Object, context);

            var newProduct = new CartProductEntity { ProductName = "New Product", Quantity = 2, Price = 20 };
            var requestBody = JsonConvert.SerializeObject(newProduct);
            var request = new Mock<HttpRequest>();
            request.Setup(r => r.Body).Returns(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(requestBody)));

            // Act
            var response = await function.Run(request.Object, "non-existing-cart-id");

            // Assert
            var result = Assert.IsType<NotFoundObjectResult>(response);
            Assert.Equal("Cart not found", result.Value);
        }

        [Fact]
        public async Task AddProductToUserCart_InvalidProduct_ReturnsBadRequestObjectResult()
        {
            // Arrange
            var context = CreateContext();
            var function = new AddproductToUserCart(_mockLogger.Object, context);

            var cart = new CartEntity
            {
                Id = Guid.NewGuid().ToString(),
                PartitionKey = "Cart",
                Cart = "Cart",
                CreatedDate = DateTime.Now,
                Products = new List<CartProductEntity>()
            };
            context.CartEntity.Add(cart);
            await context.SaveChangesAsync();

            var request = new Mock<HttpRequest>();
            request.Setup(r => r.Body).Returns(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(""))); // Empty request body

            // Act
            var response = await function.Run(request.Object, cart.Id);

            // Assert
            var result = Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal("Invalid product data", result.Value);
        }
    }
}

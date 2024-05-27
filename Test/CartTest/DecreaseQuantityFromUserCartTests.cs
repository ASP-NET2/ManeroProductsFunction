using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManeroProductsFunction.Data.Context;
using ManeroProductsFunction.Data.Entitys;
using ManeroProductsFunction.Data.Functions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Test.CartTest
{
    public class DecreaseQuantityFromUserCartTests : IDisposable
    {
        private readonly Mock<ILogger<DecreaseQuantityFromUserCart>> _mockLogger;
        private readonly DbContextOptions<DataContext> _dbContextOptions;
        private readonly DataContext _context;

        public DecreaseQuantityFromUserCartTests()
        {
            _mockLogger = new Mock<ILogger<DecreaseQuantityFromUserCart>>();
            _dbContextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new DataContext(_dbContextOptions);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        private DataContext CreateContext()
        {
            return new DataContext(_dbContextOptions);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        private void LogDatabaseState(string phase)
        {
            var cartCount = _context.CartEntity.Count();
            var productCount = _context.CartEntity.SelectMany(c => c.Products).Count();
            Console.WriteLine($"{phase} - Cart Count: {cartCount}, Product Count: {productCount}");
        }

        [Fact]
        public async Task DecreaseQuantityFromUserCart_ProductQuantityDecreased_ReturnsOkObjectResult()
        {
            // Arrange
            LogDatabaseState("Before Arrange");
            var context = CreateContext();
            var function = new DecreaseQuantityFromUserCart(_mockLogger.Object, context);

            var productId = Guid.NewGuid().ToString();
            var cart = new CartEntity
            {
                Id = Guid.NewGuid().ToString(),
                PartitionKey = "Cart",
                Cart = "Cart",
                CreatedDate = DateTime.Now,
                Products = new List<CartProductEntity>
                {
                    new CartProductEntity { ProductId = productId, ProductName = "Existing Product", Quantity = 2, Price = 10 }
                }
            };
            context.CartEntity.Add(cart);
            await context.SaveChangesAsync();
            LogDatabaseState("After Arrange");

            var request = new Mock<HttpRequest>();

            // Act
            var response = await function.Run(request.Object, cart.Id, productId);

            // Assert
            LogDatabaseState("After Act");
            var result = Assert.IsType<OkObjectResult>(response);
            var updatedCart = Assert.IsType<CartEntity>(result.Value);
            var product = updatedCart.Products.First(p => p.ProductId == productId);
            Assert.Equal(1, product.Quantity);
        }

        [Fact]
        public async Task DecreaseQuantityFromUserCart_ProductRemovedWhenQuantityIsZero_ReturnsOkObjectResult()
        {
            // Arrange
            LogDatabaseState("Before Arrange");
            var context = CreateContext();
            var function = new DecreaseQuantityFromUserCart(_mockLogger.Object, context);

            var productId = Guid.NewGuid().ToString();
            var cart = new CartEntity
            {
                Id = Guid.NewGuid().ToString(),
                PartitionKey = "Cart",
                Cart = "Cart",
                CreatedDate = DateTime.Now,
                Products = new List<CartProductEntity>
                {
                    new CartProductEntity { ProductId = productId, ProductName = "Existing Product", Quantity = 1, Price = 10 }
                }
            };
            context.CartEntity.Add(cart);
            await context.SaveChangesAsync();
            LogDatabaseState("After Arrange");

            var request = new Mock<HttpRequest>();

            // Act
            var response = await function.Run(request.Object, cart.Id, productId);

            // Assert
            LogDatabaseState("After Act");
            var result = Assert.IsType<OkObjectResult>(response);
            var updatedCart = Assert.IsType<CartEntity>(result.Value);
            Assert.DoesNotContain(updatedCart.Products, p => p.ProductId == productId);
        }

        [Fact]
        public async Task DecreaseQuantityFromUserCart_CartNotFound_ReturnsNotFoundObjectResult()
        {
            // Arrange
            LogDatabaseState("Before Arrange");
            var context = CreateContext();
            var function = new DecreaseQuantityFromUserCart(_mockLogger.Object, context);

            var request = new Mock<HttpRequest>();

            // Act
            var response = await function.Run(request.Object, "non-existing-cart-id", Guid.NewGuid().ToString());

            // Assert
            LogDatabaseState("After Act");
            var result = Assert.IsType<NotFoundObjectResult>(response);
            Assert.Equal("Cart not found.", result.Value);
        }

        [Fact]
        public async Task DecreaseQuantityFromUserCart_ProductNotFound_ReturnsNotFoundObjectResult()
        {
            // Arrange
            LogDatabaseState("Before Arrange");
            var context = CreateContext();
            var function = new DecreaseQuantityFromUserCart(_mockLogger.Object, context);

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
            LogDatabaseState("After Arrange");

            var request = new Mock<HttpRequest>();

            // Act
            var response = await function.Run(request.Object, cart.Id, "non-existing-product-id");

            // Assert
            LogDatabaseState("After Act");
            var result = Assert.IsType<NotFoundObjectResult>(response);
            Assert.Equal("Product not found in the cart.", result.Value);
        }
    }
}

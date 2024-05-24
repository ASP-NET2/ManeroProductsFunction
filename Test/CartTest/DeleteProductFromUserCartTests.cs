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
    public class DeleteProductFromUserCartTests
    {
        private readonly Mock<ILogger<DeleteProductFromUserCart>> _mockLogger;
        private readonly DbContextOptions<DataContext> _options;
        private DataContext _context;
        private DeleteProductFromUserCart _function;

        public DeleteProductFromUserCartTests()
        {
            _mockLogger = new Mock<ILogger<DeleteProductFromUserCart>>();
            _options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            InitializeContext();
        }

        private void InitializeContext()
        {
            _context = new DataContext(_options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            _function = new DeleteProductFromUserCart(_mockLogger.Object, _context);
        }

        private async Task ClearDatabaseAsync()
        {
            _context.CartEntity.RemoveRange(_context.CartEntity);
            await _context.SaveChangesAsync();
        }

        //[Fact]
        //public async Task Run_ReturnsNotFound_WhenCartDoesNotExist()
        //{
        //    // Arrange
        //    await ClearDatabaseAsync();
        //    InitializeContext();
        //    var req = new Mock<HttpRequest>();
        //    string cartId = Guid.NewGuid().ToString();
        //    string productId = Guid.NewGuid().ToString();

        //    // Act
        //    var result = await _function.Run(req.Object, cartId, productId);

        //    // Assert
        //    var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        //    Assert.Equal("Cart not found.", notFoundResult.Value);
        //}

        //[Fact]
        //public async Task Run_ReturnsNotFound_WhenProductDoesNotExistInCart()
        //{
        //    // Arrange
        //    await ClearDatabaseAsync();
        //    InitializeContext();
        //    var cart = new CartEntity { Id = Guid.NewGuid().ToString(), Products = new List<CartProductEntity>() };
        //    _context.CartEntity.Add(cart);
        //    await _context.SaveChangesAsync();

        //    var req = new Mock<HttpRequest>();
        //    string cartId = cart.Id;
        //    string productId = Guid.NewGuid().ToString();

        //    // Act
        //    var result = await _function.Run(req.Object, cartId, productId);

        //    // Assert
        //    var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        //    Assert.Equal("Product not found in the cart.", notFoundResult.Value);
        //}

        //[Fact]
        //public async Task Run_ReturnsOk_WhenProductIsRemoved()
        //{
        //    // Arrange
        //    await ClearDatabaseAsync();
        //    InitializeContext();
        //    var cart = new CartEntity
        //    {
        //        Id = Guid.NewGuid().ToString(),
        //        Products = new List<CartProductEntity>
        //        {
        //            new CartProductEntity { ProductId = Guid.NewGuid().ToString(), ProductName = "Test Product", Quantity = 1, Price = 100 }
        //        }
        //    };
        //    _context.CartEntity.Add(cart);
        //    await _context.SaveChangesAsync();

        //    var req = new Mock<HttpRequest>();
        //    string cartId = cart.Id;
        //    string productId = cart.Products.First().ProductId;

        //    // Act
        //    var result = await _function.Run(req.Object, cartId, productId);

        //    // Assert
        //    var okResult = Assert.IsType<OkObjectResult>(result);
        //    var returnedCart = Assert.IsType<CartEntity>(okResult.Value);
        //    Assert.DoesNotContain(returnedCart.Products, p => p.ProductId == productId);
        //}
    }
}

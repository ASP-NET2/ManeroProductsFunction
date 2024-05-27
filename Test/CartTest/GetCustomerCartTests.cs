using System;
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
    public class GetCustomerCartTests
    {
        private readonly Mock<ILogger<GetCustomerCart>> _mockLogger;
        private readonly DbContextOptions<DataContext> _options;
        private DataContext _context;
        private GetCustomerCart _function;

        public GetCustomerCartTests()
        {
            _mockLogger = new Mock<ILogger<GetCustomerCart>>();
            _options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            InitializeContext();
        }

        private void InitializeContext()
        {
            _context = new DataContext(_options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            _function = new GetCustomerCart(_mockLogger.Object, _context);
        }

        [Fact]
        public async Task Run_ReturnsOk_WhenCartExists()
        {
            LogDatabaseState("Before Add Cart");
            // Arrange
            InitializeContext();
            var req = new Mock<HttpRequest>();

            var cart = new CartEntity { Id = Guid.NewGuid().ToString() };
            _context.CartEntity.Add(cart);
            await _context.SaveChangesAsync();
            string cartId = cart.Id;

            // Act
            var result = await _function.Run(req.Object, cartId);
            LogDatabaseState("After Add Cart");
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<CartEntity>(okResult.Value);
            Assert.Equal(cartId, returnValue.Id);
        }

        [Fact]
        public async Task Run_ReturnsNoContent_WhenCartDoesNotExist()
        {
            // Arrange
            InitializeContext();
            var req = new Mock<HttpRequest>();
            string cartId = Guid.NewGuid().ToString();

            // Act
            var result = await _function.Run(req.Object, cartId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        private void LogDatabaseState(string phase)
        {
            var cartCount = _context.CartEntity.Count();
            var cartProd = _context.CartEntity.ToListAsync();
            var productCount = _context.CartEntity.SelectMany(c => c.Products).Count();
            Console.WriteLine($"{phase} - Cart Count: {cartCount}, Product Count: {productCount}");
        }
    }
}

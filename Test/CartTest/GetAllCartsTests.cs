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
    public class GetAllCartsTests
    {
        private readonly Mock<ILogger<GetAllCarts>> _mockLogger;
        private readonly DbContextOptions<DataContext> _options;
        private DataContext _context;
        private GetAllCarts _function;

        public GetAllCartsTests()
        {
            _mockLogger = new Mock<ILogger<GetAllCarts>>();
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
            _function = new GetAllCarts(_mockLogger.Object, _context);
        }

        //[Fact]
        //public async Task Run_ReturnsNoContent_WhenNoCartsExist()
        //{
        //    // Arrange
        //    InitializeContext();
        //    var req = new Mock<HttpRequest>();

        //    // Act
        //    var result = await _function.Run(req.Object);

        //    // Assert
        //    Assert.IsType<NoContentResult>(result);
        //}

        //[Fact]
        //public async Task Run_ReturnsOk_WithListOfCarts()
        //{
        //    // Arrange
        //    InitializeContext();
        //    var req = new Mock<HttpRequest>();

        //    var carts = new List<CartEntity>
        //    {
        //        new CartEntity { Id = Guid.NewGuid().ToString() },
        //        new CartEntity { Id = Guid.NewGuid().ToString() }
        //    };

        //    _context.CartEntity.AddRange(carts);
        //    await _context.SaveChangesAsync();

        //    // Act
        //    var result = await _function.Run(req.Object);

        //    // Assert
        //    var okResult = Assert.IsType<OkObjectResult>(result);
        //    var returnValue = Assert.IsType<List<CartEntity>>(okResult.Value);
        //    Assert.Equal(2, returnValue.Count);
        //}
    }
}

using ManeroProductsFunction.Data.Context;
using ManeroProductsFunction.Data.Entitys;
using ManeroProductsFunction.Functions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;


namespace Test.ProductUnitTest
{
    public class GetProductTests : IDisposable
    {
        private readonly Mock<ILogger<GetAllProducts>> _mockLogger;
        private readonly DataContext _context;
        private readonly GetAllProducts _function;
        private bool _disposed;

        public GetProductTests()
        {
            _mockLogger = new Mock<ILogger<GetAllProducts>>();

            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new DataContext(options);

            _function = new GetAllProducts(_mockLogger.Object, _context);
        }

        private void SeedDatabase()
        {
            var products = new List<ProductEntity>
            {
                new ProductEntity
                {
                    Id = "1",
                    Author = "Author1",
                    Title = "Title1",
                    Price = "100",
                    ImageUrl = "image1",
                    Category = "Category1",
                    SubCategory = "SubCategory1",
                    Format = "Format1",
                    CategoryName = "CategoryName1",
                    SubCategoryName = "SubCategoryName1",
                    FormatName = "FormatName1"
                },
                new ProductEntity
                {
                    Id = "2",
                    Author = "Author2",
                    Title = "Title2",
                    Price = "200",
                    ImageUrl = "image2",
                    Category = "Category2",
                    SubCategory = "SubCategory2",
                    Format = "Format2",
                    CategoryName = "CategoryName2",
                    SubCategoryName = "SubCategoryName2",
                    FormatName = "FormatName2"
                }
            };
            _context.Product.AddRange(products);
            _context.SaveChanges();

            // Verify that products are added
            var addedProducts = _context.Product.ToList();
            Console.WriteLine($"Products added to database: {addedProducts.Count}");
            foreach (var product in addedProducts)
            {
                Console.WriteLine($"Product ID: {product.Id}, Title: {product.Title}");
            }
        }

        [Fact]
        public async Task RunGetAll_ReturnsOkObjectResult_WhenProductsExist()
        {
            // Arrange
            SeedDatabase();
            var request = new Mock<HttpRequest>();

            // Log to verify that the database is seeded
            var productCount = await _context.Product.CountAsync();
            Console.WriteLine($"Product count in database after seeding: {productCount}");

            // Act
            var result = await _function.RunGetAll(request.Object);

            // Assert
            if (result is NoContentResult)
            {
                Console.WriteLine("Received NoContentResult, indicating no products found.");
            }

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult);
            var returnValue = Assert.IsType<List<ProductEntity>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task RunGetAll_ReturnsNoContentResult_WhenNoProductsExist()
        {
            // Arrange
            // Ensuring the database is clean before the test
            foreach (var product in _context.Product)
            {
                _context.Product.Remove(product);
            }
            _context.SaveChanges();

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

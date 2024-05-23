using ManeroProductsFunction.Data.Context;
using ManeroProductsFunction.Data.Entitys;
using ManeroProductsFunction.Functions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Globalization;

namespace Test.SortTest
{
    public class SortProductTests
    {
        private readonly Mock<ILogger<SortProduct>> _mockLogger;

        public SortProductTests()
        {
            _mockLogger = new Mock<ILogger<SortProduct>>();
        }

        private DataContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase" + System.Guid.NewGuid().ToString()) // Ensure a unique database for each test
                .Options;
            var context = new DataContext(options);

            SeedDatabase(context);

            return context;
        }

        private void SeedDatabase(DataContext context)
        {
            context.Product.AddRange(new List<ProductEntity>
    {
        new ProductEntity { CategoryName = "electronics", SubCategoryName = "phones", FormatName = "new", Price = "300", OnSale = true, BestSeller = true, FeaturedProduct = false, IsFavorite = true, Author = "Thomas Hallström", Title = "Barn av vår tid", Rating="4,5"  },
        new ProductEntity { CategoryName = "electronics", SubCategoryName = "laptops", FormatName = "new", Price = "1200", OnSale = false, BestSeller = false, FeaturedProduct = true, IsFavorite = false, Author = "Thomas Hallström", Title = "Kolla kolla", Rating="4,8" },
        new ProductEntity { CategoryName = "home", SubCategoryName = "furniture", FormatName = "used", Price = "200", OnSale = true, BestSeller = false, FeaturedProduct = false, IsFavorite = true, Author = "Thomas Hallström", Title = "Livet är en fest", Rating = null }
    });
            context.SaveChanges();
        }

        private HttpRequest CreateHttpRequest(Dictionary<string, string> query)
        {
            var context = new DefaultHttpContext();
            var request = context.Request;

            var queryString = "?" + string.Join('&', query.Select(q => $"{q.Key}={q.Value}"));
            request.QueryString = new QueryString(queryString);

            // Log query string for debugging purposes
            _mockLogger.Object.LogInformation("QueryString: {QueryString}", queryString);

            return request;
        }

        [Fact]
        public async Task Run_ReturnsNoContent_WhenNoProducts()
        {
            // Arrange
            var context = GetInMemoryContext();
            context.Product.RemoveRange(context.Product);
            context.SaveChanges();
            var function = new SortProduct(_mockLogger.Object, context);

            // Act
            var result = await function.Run(CreateHttpRequest(new Dictionary<string, string>()));

            // Log result for debugging purposes
            _mockLogger.Object.LogInformation("Result: {Result}", result);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Run_FiltersByCategory()
        {
            // Arrange
            var context = GetInMemoryContext();
            var function = new SortProduct(_mockLogger.Object, context);

            // Act
            var result = await function.Run(CreateHttpRequest(new Dictionary<string, string> { { "category", "electronics" } }));

            // Log result for debugging purposes
            _mockLogger.Object.LogInformation("Result: {Result}", result);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var products = Assert.IsType<List<ProductEntity>>(okResult.Value);

            // Log products for debugging purposes
            _mockLogger.Object.LogInformation("Filtered Products by Category: {Products}", products);

            // Check intermediate state
            Assert.NotEmpty(products);
            Assert.Equal(2, products.Count);

            // Verify individual product details
            foreach (var product in products)
            {
                _mockLogger.Object.LogInformation("Product: {Title}, {Price}", product.Title, product.Price);
            }
        }

        [Fact]
        public async Task Run_FiltersByPriceRange()
        {
            // Arrange
            var context = GetInMemoryContext();
            var function = new SortProduct(_mockLogger.Object, context);

            // Act
            var result = await function.Run(CreateHttpRequest(new Dictionary<string, string> { { "minPrice", "250" }, { "maxPrice", "500" } }));

            // Log result for debugging purposes
            _mockLogger.Object.LogInformation("Result: {Result}", result);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var products = Assert.IsType<List<ProductEntity>>(okResult.Value);

            // Log products for debugging purposes
            _mockLogger.Object.LogInformation("Filtered Products by Price Range: {Products}", products);

            // Check intermediate state
            Assert.NotEmpty(products);
            Assert.Single(products);

            // Verify individual product details
            foreach (var product in products)
            {
                _mockLogger.Object.LogInformation("Product: {Title}, {Price}", product.Title, product.Price);
            }
        }

        [Fact]
        public async Task Run_FiltersByOnSale()
        {
            // Arrange
            var context = GetInMemoryContext();
            var function = new SortProduct(_mockLogger.Object, context);

            // Act
            var result = await function.Run(CreateHttpRequest(new Dictionary<string, string> { { "onSale", "true" } }));

            // Log result for debugging purposes
            _mockLogger.Object.LogInformation("Result: {Result}", result);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var products = Assert.IsType<List<ProductEntity>>(okResult.Value);

            // Log products for debugging purposes
            _mockLogger.Object.LogInformation("Filtered Products by On Sale: {Products}", products);

            // Check intermediate state
            Assert.NotEmpty(products);
            Assert.Equal(2, products.Count); // Förväntar 2 produkter som är på rea

            // Verify individual product details
            foreach (var product in products)
            {
                _mockLogger.Object.LogInformation("Product: {Title}, {OnSale}", product.Title, product.OnSale);
            }
        }

        [Fact]
        public async Task Run_FiltersByIsFavorite()
        {
            // Arrange
            var context = GetInMemoryContext();
            var function = new SortProduct(_mockLogger.Object, context);

            // Act
            var result = await function.Run(CreateHttpRequest(new Dictionary<string, string> { { "isFavorite", "true" } }));

            // Log result for debugging purposes
            _mockLogger.Object.LogInformation("Result: {Result}", result);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var products = Assert.IsType<List<ProductEntity>>(okResult.Value);

            // Log products for debugging purposes
            _mockLogger.Object.LogInformation("Filtered Products by IsFavorite: {Products}", products);

            // Check intermediate state
            Assert.NotEmpty(products);

            // Verify individual product details
            foreach (var product in products)
            {
                Assert.True(product.IsFavorite);
                _mockLogger.Object.LogInformation("Product: {Title}, {IsFavorite}", product.Title, product.IsFavorite);
            }
        }

        [Fact]
        public async Task Run_FiltersByTitle()
        {
            // Arrange
            var context = GetInMemoryContext();
            var function = new SortProduct(_mockLogger.Object, context);

            // Act
            var result = await function.Run(CreateHttpRequest(new Dictionary<string, string> { { "title", "Barn av vår tid" } }));

            // Log result for debugging purposes
            _mockLogger.Object.LogInformation("Result: {Result}", result);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var products = Assert.IsType<List<ProductEntity>>(okResult.Value);

            // Log products for debugging purposes
            _mockLogger.Object.LogInformation("Filtered Products by Title: {Products}", products);

            // Check intermediate state
            Assert.NotEmpty(products);
            Assert.Single(products); // Förväntar 1 produkt med den titeln

            // Verify individual product details
            foreach (var product in products)
            {
                Assert.Equal("Barn av vår tid", product.Title);
                _mockLogger.Object.LogInformation("Product: {Title}, {Price}", product.Title, product.Price);
            }
        }
        [Fact]
        public async Task Run_FiltersByRatingNotNull()
        {
            // Arrange
            var context = GetInMemoryContext();
            var function = new SortProduct(_mockLogger.Object, context);

            // Act
            var result = await function.Run(CreateHttpRequest(new Dictionary<string, string> { { "ratingNotNull", "true" } }));

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var products = Assert.IsType<List<ProductEntity>>(okResult.Value);

            Assert.NotEmpty(products);
            Assert.All(products, p => Assert.NotNull(p.Rating));
        }

        [Fact]
        public async Task Run_FiltersByMinRating()
        {
            // Arrange
            var context = GetInMemoryContext();
            var function = new SortProduct(_mockLogger.Object, context);

            // Act
            var result = await function.Run(CreateHttpRequest(new Dictionary<string, string> { { "minRating", "4,0" } }));

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var products = Assert.IsType<List<ProductEntity>>(okResult.Value);

            Assert.NotEmpty(products);
            Assert.All(products, p => Assert.True(decimal.Parse(p.Rating.Replace(",", "."), CultureInfo.InvariantCulture) >= 4.0m));
        }

        [Fact]
        public async Task Run_FiltersByMaxRating()
        {
            // Arrange
            var context = GetInMemoryContext();
            var function = new SortProduct(_mockLogger.Object, context);

            // Act
            var result = await function.Run(CreateHttpRequest(new Dictionary<string, string> { { "maxRating", "4,5" } }));

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var products = Assert.IsType<List<ProductEntity>>(okResult.Value);

            Assert.NotEmpty(products);
            Assert.All(products, p => Assert.True(decimal.Parse(p.Rating.Replace(",", "."), CultureInfo.InvariantCulture) <= 4.5m));
        }

        [Fact]
        public async Task Run_FiltersByRatingRange()
        {
            // Arrange
            var context = GetInMemoryContext();
            var function = new SortProduct(_mockLogger.Object, context);

            // Act
            var result = await function.Run(CreateHttpRequest(new Dictionary<string, string> { { "minRating", "4,0" }, { "maxRating", "4,8" } }));

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var products = Assert.IsType<List<ProductEntity>>(okResult.Value);

            Assert.NotEmpty(products);
            Assert.All(products, p =>
            {
                var rating = decimal.Parse(p.Rating.Replace(",", "."), CultureInfo.InvariantCulture);
                Assert.True(rating >= 4.0m && rating <= 4.8m);
            });
        }


    }
}

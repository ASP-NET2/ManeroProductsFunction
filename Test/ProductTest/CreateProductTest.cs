using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ManeroProductsFunction.Data.Context;
using ManeroProductsFunction.Data.Entitys;
using ManeroProductsFunction.Functions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Xunit;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Net;

namespace Test.ProductTest
{
    public class CreateProductTests
    {
        private readonly Mock<ILogger<CreateProduct>> _mockLogger;
        private readonly DbContextOptions<DataContext> _dbContextOptions;

        public CreateProductTests()
        {
            _mockLogger = new Mock<ILogger<CreateProduct>>();
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

        private HttpRequestData CreateHttpRequestData(string body)
        {
            var context = new Mock<FunctionContext>();
            var request = new Mock<HttpRequestData>(context.Object);
            var headers = new HttpHeadersCollection();
            var query = new NameValueCollection();
            var uri = new Uri("http://localhost");

            request.Setup(r => r.Body).Returns(new MemoryStream(Encoding.UTF8.GetBytes(body)));
            request.Setup(r => r.Headers).Returns(headers);
            request.Setup(r => r.Method).Returns("POST");
            request.Setup(r => r.Url).Returns(uri);
            request.Setup(r => r.CreateResponse()).Returns(new Mock<HttpResponseData>(context.Object).Object);

            return request.Object;
        }

        [Fact]
        public async Task CreateProduct_Returns_OkObjectResult()
        {
            // Arrange
            var context = GetInMemoryContext();
            var function = new CreateProduct(_mockLogger.Object, context);

            var product = new ProductEntity
            {
                Author = "Test Author",
                Title = "Test Title",
                Price = "100",
                ShortDescription = "Short Description",
                LongDescription = "Long Description",
                Language = "English",
                Pages = "200",
                PublishDate = DateTime.Now.ToString("yyyy-MM-dd"),
                Publisher = "Test Publisher",
                ISBN = "1234567890",
                ImageUrl = "http://example.com/image.jpg",
                OnSale = true,
                BestSeller = false,
                IsFavorite = true,
                FeaturedProduct = false,
                CategoryName = "Category",
                SubCategoryName = "SubCategory",
                FormatName = "Format",
                Rating = "4.5"
            };

            var requestBody = JsonConvert.SerializeObject(product);
            var request = CreateHttpRequestData(requestBody);

            // Act
            var response = await function.Run(request);

            // Assert
            var result = Assert.IsType<OkObjectResult>(response);
            var createdProduct = Assert.IsType<ProductEntity>(result.Value);

            Assert.Equal("Test Author", createdProduct.Author);
            Assert.Equal("Test Title", createdProduct.Title);
            Assert.Equal("100", createdProduct.Price);
            Assert.Equal("Short Description", createdProduct.ShortDescription);
            Assert.Equal("Long Description", createdProduct.LongDescription);
            Assert.Equal("English", createdProduct.Language);
            Assert.Equal("200", createdProduct.Pages);
            Assert.Equal(DateTime.Now.ToString("yyyy-MM-dd"), createdProduct.PublishDate);
            Assert.Equal("Test Publisher", createdProduct.Publisher);
            Assert.Equal("1234567890", createdProduct.ISBN);
            Assert.Equal("http://example.com/image.jpg", createdProduct.ImageUrl);
            Assert.True(createdProduct.OnSale);
            Assert.False(createdProduct.BestSeller);
            Assert.True(createdProduct.IsFavorite);
            Assert.False(createdProduct.FeaturedProduct);
            Assert.Equal("Category", createdProduct.CategoryName);
            Assert.Equal("SubCategory", createdProduct.SubCategoryName);
            Assert.Equal("Format", createdProduct.FormatName);
            Assert.Equal("4.5", createdProduct.Rating);
        }

        [Fact]
        public async Task CreateProduct_Returns_InternalServerError_On_Exception()
        {
            // Arrange
            var context = GetInMemoryContext();
            var function = new CreateProduct(_mockLogger.Object, context);

            var invalidProductJson = "{ invalid json }";
            var request = CreateHttpRequestData(invalidProductJson);

            // Act
            var response = await function.Run(request);

            // Assert
            var result = Assert.IsType<ObjectResult>(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, result.StatusCode);

            // Serialize the ObjectResult value to a JSON string and then deserialize it
            var serializedResponse = JsonConvert.SerializeObject(result.Value);
            dynamic errorResponse = JsonConvert.DeserializeObject(serializedResponse);
            Assert.NotNull(errorResponse);

            // Verify that the error message contains expected keywords
            string actualErrorMessage = errorResponse.error.ToString();
            Assert.Contains("Invalid character", actualErrorMessage);
            Assert.Contains("Expected", actualErrorMessage);
        }
    }
}
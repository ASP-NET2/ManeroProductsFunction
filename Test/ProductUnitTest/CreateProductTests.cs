//using Azure.Core.Serialization;
//using ManeroProductsFunction.Data.Context;
//using ManeroProductsFunction.Data.Entitys;
//using ManeroProductsFunction.Functions;
//using Microsoft.Azure.Functions.Worker;
//using Microsoft.Azure.Functions.Worker.Http;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using Moq;
//using Newtonsoft.Json;
//using System;
//using System.IO;
//using System.Text;
//using System.Threading.Tasks;
//using Xunit;

//namespace Test.ProductUnitTest
//{
//    public class CreateProductTests : IDisposable
//    {
//        private readonly Mock<ILogger<CreateProduct>> _createMockLogger;
//        private readonly DataContext _createContext;
//        private readonly CreateProduct _createFunction;
//        private bool _disposed;

//        public CreateProductTests()
//        {
//            _createMockLogger = new Mock<ILogger<CreateProduct>>();

//            var options = new DbContextOptionsBuilder<DataContext>()
//                .UseInMemoryDatabase(databaseName: "CreateTestDatabase")
//                .Options;
//            _createContext = new DataContext(options);

//            _createFunction = new CreateProduct(_createMockLogger.Object, _createContext);
//        }

//        private HttpRequestData CreateHttpRequestData(string body, FunctionContext functionContext)
//        {
//            var request = new Mock<HttpRequestData>(functionContext);

//            var stream = new MemoryStream(Encoding.UTF8.GetBytes(body));
//            request.Setup(r => r.Body).Returns(stream);
//            request.Setup(r => r.Headers).Returns(new HttpHeadersCollection());
//            request.Setup(r => r.Method).Returns("POST");
//            request.Setup(r => r.Url).Returns(new Uri("http://localhost"));

//            request.Setup(r => r.CreateResponse()).Returns(() =>
//            {
//                var response = new Mock<HttpResponseData>(functionContext);
//                response.SetupProperty(r => r.StatusCode);
//                response.SetupProperty(r => r.Headers, new HttpHeadersCollection());
//                response.SetupProperty(r => r.Body, new MemoryStream());
//                return response.Object;
//            });

//            return request.Object;
//        }

//        [Fact]
//        public async Task Run_CreatesProduct_ReturnsOkObjectResult()
//        {
//            // Arrange
//            var product = new ProductEntity
//            {
//                Author = "Author1",
//                Title = "Title1",
//                Price = "100",
//                ImageUrl = "http://example.com/image.jpg",
//                Category = "Category1",
//                SubCategory = "SubCategory1",
//                Format = "Format1",
//                CategoryName = "CategoryName1",
//                SubCategoryName = "SubCategoryName1",
//                FormatName = "FormatName1"
//            };
//            var jsonProduct = JsonConvert.SerializeObject(product);

//            var functionContext = new Mock<FunctionContext>();
//            var services = new ServiceCollection();
//            services.AddSingleton(new JsonObjectSerializer());
//            var serviceProvider = services.BuildServiceProvider();
//            functionContext.Setup(c => c.InstanceServices).Returns(serviceProvider);

//            var request = CreateHttpRequestData(jsonProduct, functionContext.Object);

//            // Act
//            var result = await _createFunction.Run(request);

//            // Assert
//            var okResult = Assert.IsType<HttpResponseData>(result);
//            Assert.Equal(System.Net.HttpStatusCode.OK, okResult.StatusCode);

//            okResult.Body.Seek(0, SeekOrigin.Begin);
//            var responseBody = await new StreamReader(okResult.Body).ReadToEndAsync();
//            var returnValue = JsonConvert.DeserializeObject<ProductEntity>(responseBody);

//            Assert.Equal("Author1", returnValue.Author);
//            Assert.Equal("Title1", returnValue.Title);
//            Assert.Equal("100", returnValue.Price);
//            Assert.Equal("http://example.com/image.jpg", returnValue.ImageUrl);
//            Assert.Equal("Category1", returnValue.Category);
//            Assert.Equal("SubCategory1", returnValue.SubCategory);
//            Assert.Equal("Format1", returnValue.Format);
//            Assert.Equal("CategoryName1", returnValue.CategoryName);
//            Assert.Equal("SubCategoryName1", returnValue.SubCategoryName);
//            Assert.Equal("FormatName1", returnValue.FormatName);
//        }

//        protected virtual void Dispose(bool disposing)
//        {
//            if (!_disposed)
//            {
//                if (disposing)
//                {
//                    _createContext.Database.EnsureDeleted();
//                    _createContext.Dispose();
//                }
//                _disposed = true;
//            }
//        }

//        public void Dispose()
//        {
//            Dispose(true);
//            GC.SuppressFinalize(this);
//        }
//    }
//}

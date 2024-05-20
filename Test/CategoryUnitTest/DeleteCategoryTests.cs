//using Azure.Core.Serialization;
//using ManeroProductsFunction.Data.Context;
//using ManeroProductsFunction.Data.Entitys;
//using ManeroProductsFunction.Functions.Category;
//using Microsoft.Azure.Functions.Worker;
//using Microsoft.Azure.Functions.Worker.Http;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using Moq;

//namespace Test.CategoryUnitTest
//{
//    public class DeleteCategoryTests : IDisposable
//    {
//        private readonly Mock<ILogger<DeleteCategory>> _deleteMockLogger;
//        private readonly DataContext _deleteContext;
//        private readonly DeleteCategory _deleteFunction;
//        private bool _disposed;

//        public DeleteCategoryTests()
//        {
//            _deleteMockLogger = new Mock<ILogger<DeleteCategory>>();

//            var options = new DbContextOptionsBuilder<DataContext>()
//                .UseInMemoryDatabase(databaseName: "DeleteTestDatabase")
//                .Options;
//            _deleteContext = new DataContext(options);

//            _deleteFunction = new DeleteCategory(_deleteMockLogger.Object, _deleteContext);
//        }

//        private HttpRequestData CreateHttpRequestData(FunctionContext functionContext)
//        {
//            var request = new Mock<HttpRequestData>(functionContext);
//            request.Setup(r => r.Method).Returns("DELETE");
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

//        private void SeedDatabase()
//        {
//            var categories = new List<CategoryEntity>
//            {
//                new CategoryEntity { Id = "1", CategoryName = "Category1", ImageLink = "link1" },
//                new CategoryEntity { Id = "2", CategoryName = "Category2", ImageLink = "link2" }
//            };
//            _deleteContext.Category.AddRange(categories);
//            _deleteContext.SaveChanges();
//        }

//        [Fact]
//        public async Task Run_DeleteExistingCategory_ReturnsOkResult()
//        {
//            // Arrange
//            SeedDatabase();
//            var functionContext = new Mock<FunctionContext>();
//            var services = new ServiceCollection();
//            services.AddSingleton(new JsonObjectSerializer());
//            var serviceProvider = services.BuildServiceProvider();
//            functionContext.Setup(c => c.InstanceServices).Returns(serviceProvider);
//            var request = CreateHttpRequestData(functionContext.Object);

//            // Act
//            var result = await _deleteFunction.Run(request, "1");

//            // Assert
//            var okResult = Assert.IsType<HttpResponseData>(result);
//            Assert.Equal(System.Net.HttpStatusCode.OK, okResult.StatusCode);
//        }

//        [Fact]
//        public async Task Run_DeleteNonExistingCategory_ReturnsNotFoundResult()
//        {
//            // Arrange
//            SeedDatabase();
//            var functionContext = new Mock<FunctionContext>();
//            var services = new ServiceCollection();
//            services.AddSingleton(new JsonObjectSerializer());
//            var serviceProvider = services.BuildServiceProvider();
//            functionContext.Setup(c => c.InstanceServices).Returns(serviceProvider);
//            var request = CreateHttpRequestData(functionContext.Object);

//            // Act
//            var result = await _deleteFunction.Run(request, "3");

//            // Assert
//            var notFoundResult = Assert.IsType<HttpResponseData>(result);
//            Assert.Equal(System.Net.HttpStatusCode.NotFound, notFoundResult.StatusCode);
//        }

//        protected virtual void Dispose(bool disposing)
//        {
//            if (!_disposed)
//            {
//                if (disposing)
//                {
//                    _deleteContext.Database.EnsureDeleted();
//                    _deleteContext.Dispose();
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
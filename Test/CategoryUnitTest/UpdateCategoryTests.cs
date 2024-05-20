//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Net;
//using System.Text;
//using System.Threading.Tasks;
//using ManeroProductsFunction.Data.Context;
//using ManeroProductsFunction.Data.Entitys;
//using ManeroProductsFunction.Functions.Category;
//using Microsoft.Azure.Functions.Worker;
//using Microsoft.Azure.Functions.Worker.Http;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using Moq;
//using Newtonsoft.Json;
//using Xunit;

//namespace Test.CategoryUnitTest
//{
//    public class UpdateCategoryTests : IDisposable
//    {
//        private readonly Mock<ILogger<UpdateCategory>> _updateMockLogger;
//        private readonly DataContext _updateContext;
//        private readonly UpdateCategory _updateFunction;
//        private bool _disposed;

//        public UpdateCategoryTests()
//        {
//            _updateMockLogger = new Mock<ILogger<UpdateCategory>>();

//            var options = new DbContextOptionsBuilder<DataContext>()
//                .UseInMemoryDatabase(databaseName: "UpdateTestDatabase")
//                .Options;
//            _updateContext = new DataContext(options);

//            _updateFunction = new UpdateCategory(_updateMockLogger.Object, _updateContext);
//        }

//        private void SeedDatabase()
//        {
//            var categories = new List<CategoryEntity>
//            {
//                new CategoryEntity { Id = "1", CategoryName = "Category1", ImageLink = "link1" },
//                new CategoryEntity { Id = "2", CategoryName = "Category2", ImageLink = "link2" }
//            };
//            _updateContext.Category.AddRange(categories);
//            _updateContext.SaveChanges();
//        }

//        private HttpRequestData CreateHttpRequestData(string body)
//        {
//            var context = new Mock<FunctionContext>();
//            var request = new Mock<HttpRequestData>(context.Object);
//            var stream = new MemoryStream(Encoding.UTF8.GetBytes(body));
//            request.Setup(r => r.Body).Returns(stream);
//            request.Setup(r => r.Method).Returns("PUT");
//            request.Setup(r => r.Headers).Returns(new HttpHeadersCollection());

//            var response = new Mock<HttpResponseData>(context.Object);
//            response.SetupProperty(r => r.StatusCode, HttpStatusCode.OK);
//            response.Setup(r => r.Body).Returns(new MemoryStream());
//            request.Setup(r => r.CreateResponse()).Returns(response.Object);

//            return request.Object;
//        }

//        [Fact]
//        public async Task Run_UpdateCategory_ReturnsOkObjectResult_WhenCategoryIsUpdated()
//        {
//            // Arrange
//            SeedDatabase();
//            var updatedCategory = new CategoryEntity
//            {
//                Id = "1",
//                CategoryName = "Updated Category",
//                ImageLink = "http://example.com/updatedimage.jpg"
//            };
//            var jsonCategory = JsonConvert.SerializeObject(updatedCategory);
//            var request = CreateHttpRequestData(jsonCategory);

//            // Act
//            var result = await _updateFunction.Run(request);

//            // Assert
//            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
//            var returnValue = JsonConvert.DeserializeObject<CategoryEntity>(await new StreamReader(result.Body).ReadToEndAsync());
//            Assert.Equal("Updated Category", returnValue?.CategoryName);
//            Assert.Equal("http://example.com/updatedimage.jpg", returnValue?.ImageLink);
//        }

//        [Fact]
//        public async Task Run_UpdateCategory_ReturnsNotFoundResult_WhenCategoryDoesNotExist()
//        {
//            // Arrange
//            var updatedCategory = new CategoryEntity
//            {
//                Id = "3",
//                CategoryName = "Non Existing Category",
//                ImageLink = "http://example.com/nonexistingimage.jpg"
//            };
//            var jsonCategory = JsonConvert.SerializeObject(updatedCategory);
//            var request = CreateHttpRequestData(jsonCategory);

//            // Act
//            var result = await _updateFunction.Run(request);

//            // Assert
//            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
//        }

//        [Fact]
//        public async Task Run_UpdateCategory_ReturnsBadRequestResult_WhenCategoryIsInvalid()
//        {
//            // Arrange
//            var request = CreateHttpRequestData("invalid json");

//            // Act
//            var result = await _updateFunction.Run(request);

//            // Assert
//            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
//        }

//        protected virtual void Dispose(bool disposing)
//        {
//            if (!_disposed)
//            {
//                if (disposing)
//                {
//                    _updateContext.Database.EnsureDeleted();
//                    _updateContext.Dispose();
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
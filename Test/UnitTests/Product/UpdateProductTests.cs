using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ManeroProductsFunction.Data.Context;
using ManeroProductsFunction.Data.Entitys;
using ManeroProductsFunction.Functions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace ManeroProductsFunction.Tests.UnitTests;

public class MockHttpResponseData : HttpResponseData
{
    public MockHttpResponseData(FunctionContext functionContext) : base(functionContext)
    {
        Body = new MemoryStream();
        Headers = new HttpHeadersCollection();
    }

    public override Stream Body { get; set; }
    public override HttpHeadersCollection Headers { get; set; }
    public override HttpStatusCode StatusCode { get; set; }

    public override HttpCookies Cookies => throw new NotImplementedException();
}

public class UpdateProductTests
{
    private DataContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new DataContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    private HttpRequestData CreateHttpRequestData(FunctionContext context, string bodyContent)
    {
        var request = new Mock<HttpRequestData>(context);
        var response = new MockHttpResponseData(context);
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(bodyContent));

        request.Setup(r => r.Body).Returns(stream);
        request.Setup(r => r.Headers).Returns(new HttpHeadersCollection());
        request.Setup(r => r.Method).Returns("PUT");
        request.Setup(r => r.Url).Returns(new Uri("http://localhost"));
        request.Setup(r => r.CreateResponse()).Returns(response);

        return request.Object;
    }

    [Fact]
    public async Task UpdateProduct_ReturnsOkResult_WhenProductIsUpdated()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<UpdateProduct>>();
        var context = GetInMemoryContext();
        var functionContext = new Mock<FunctionContext>().Object;

        var productId = "99";
        var product = new ProductEntity
        {
            Id = productId,
            PartitionKey = "Products",
            Title = "Old Title",
            Author = "Old Author",
            Category = "Old Category",
            Format = "Old Format",
            ImageUrl = "Old ImageUrl",
            Price = "100",
            SubCategory = "Old SubCategory"
        };

        context.Product.Add(product);
        await context.SaveChangesAsync();

        var updateProductFunction = new UpdateProduct(mockLogger.Object, context);

        var updatedProduct = new ProductEntity
        {
            Id = productId,
            PartitionKey = "Products",
            Title = "New Title",
            Author = "New Author",
            Category = "New Category",
            Format = "New Format",
            ImageUrl = "New ImageUrl",
            Price = "200",
            SubCategory = "New SubCategory"
        };

        var requestBody = JsonConvert.SerializeObject(updatedProduct);
        var mockRequest = CreateHttpRequestData(functionContext, requestBody);

        // Act
        var result = await updateProductFunction.Run(mockRequest);

        // Assert
        var response = (MockHttpResponseData)result;
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProduct_ReturnsNotFoundResult_WhenProductNotFound()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<UpdateProduct>>();
        var context = GetInMemoryContext();
        var functionContext = new Mock<FunctionContext>().Object;

        var productId = "100"; // Säkerställa att produkten inte finns
        var updateProductFunction = new UpdateProduct(mockLogger.Object, context);

        var updatedProduct = new ProductEntity
        {
            Id = productId,
            PartitionKey = "Products",
            Title = "New Title",
            Author = "New Author",
            Category = "New Category",
            Format = "New Format",
            ImageUrl = "New ImageUrl",
            Price = "200",
            SubCategory = "New SubCategory"
        };

        var requestBody = JsonConvert.SerializeObject(updatedProduct);
        var mockRequest = CreateHttpRequestData(functionContext, requestBody);

        // Act
        var result = await updateProductFunction.Run(mockRequest);

        // Assert
        var response = (MockHttpResponseData)result;
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProduct_ReturnsBadRequestResult_WhenInvalidProductData()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<UpdateProduct>>();
        var context = GetInMemoryContext();
        var functionContext = new Mock<FunctionContext>().Object;

        var updateProductFunction = new UpdateProduct(mockLogger.Object, context);

        var invalidRequestBody = "invalid json";
        var mockRequest = CreateHttpRequestData(functionContext, invalidRequestBody);

        // Act
        var result = await updateProductFunction.Run(mockRequest);

        // Assert
        var response = (MockHttpResponseData)result;
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
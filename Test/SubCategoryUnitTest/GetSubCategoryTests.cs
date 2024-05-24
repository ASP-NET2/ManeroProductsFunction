using ManeroProductsFunction.Data.Context;
using ManeroProductsFunction.Data.Entitys;
using ManeroProductsFunction.Functions.SubCategory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Test.SubCategoryUnitTest;

public class GetSubCategoryTests : IDisposable
{
    private readonly Mock<ILogger<GetSubCategory>> _mockLogger;
    private readonly DataContext _context;
    private readonly GetSubCategory _function;
    private bool _disposed;

    public GetSubCategoryTests()
    {
        _mockLogger = new Mock<ILogger<GetSubCategory>>();

        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _context = new DataContext(options);

        _function = new GetSubCategory(_mockLogger.Object, _context);
    }

    private void SeedDatabase()
    {
        var subCategories = new List<SubCategoryEntity>
        {
            new SubCategoryEntity { Id = "1", SubCategoryName = "SubCategory1", ImageLink = "link1" },
            new SubCategoryEntity { Id = "2", SubCategoryName = "SubCategory2", ImageLink = "link2" }
        };
        _context.SubCategory.AddRange(subCategories);
        _context.SaveChanges();
    }

    //[Fact]
    //public async Task RunGetAll_ReturnsOkObjectResult_WhenSubCategoriesExist()
    //{
    //    // Arrange
    //    SeedDatabase();
    //    var request = new Mock<HttpRequest>();

    //    // Act
    //    var result = await _function.RunGetAll(request.Object);

    //    // Assert
    //    var okResult = Assert.IsType<OkObjectResult>(result);
    //    var returnValue = Assert.IsType<List<SubCategoryEntity>>(okResult.Value);
    //    Assert.Equal(2, returnValue.Count);
    //}

    [Fact]
    public async Task RunGetAll_ReturnsNoContentResult_WhenNoSubCategoriesExist()
    {
        // Arrange
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

using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ManeroProductsFunction.Functions
{
    public class ImageUploader
    {
        private readonly IConfiguration _configuration;
        private readonly BlobServiceClient _client;
        private readonly BlobContainerClient _container;
        private BlobClient? _blobClient;

        public ImageUploader(IConfiguration configuration)
        {
            _configuration = configuration;
            _client = new BlobServiceClient(_configuration.GetConnectionString("DefaultEndpointsProtocol=https;AccountName=productsblob;AccountKey=oZ6fd6P0fVvyt38Iyz9Jo6UzIefg5m0Q7U52567g2oQCcfWcCYtCAeeX2kLvwyiik2KIRJtU0HzQ+AStmYzPSQ==;EndpointSuffix=core.windows.net"));
            _container = _client.GetBlobContainerClient("images");
        }



        [Function("ImageUpload")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req, IFormFile file)
        {
            if (file != null || file!.Length != 0)
            {
                _blobClient = _container.GetBlobClient(file.FileName);
                await using var stream = file.OpenReadStream();
                await _blobClient.UploadAsync(stream, overwrite: true);
                return new OkObjectResult(_blobClient.Uri);
            }
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}

using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using System.Text;

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
            _client = new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=productsblob;AccountKey=oZ6fd6P0fVvyt38Iyz9Jo6UzIefg5m0Q7U52567g2oQCcfWcCYtCAeeX2kLvwyiik2KIRJtU0HzQ+AStmYzPSQ==;EndpointSuffix=core.windows.net");
            _container = _client.GetBlobContainerClient("images");
        }



        [Function("ImageUpload")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            // Läs innehållet i förfrågningskroppen
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            // Kontrollera om kroppen innehåller något innehåll
            if (!string.IsNullOrEmpty(requestBody))
            {
                // Anta att filen skickas som multipart/form-data och separera det
                var boundary = GetBoundary(req.ContentType);
                var sections = requestBody.Split(new[] { boundary }, StringSplitOptions.RemoveEmptyEntries);

                // Loopa igenom varje sektion för att hitta filen
                foreach (var section in sections)
                {
                    if (section.Contains("filename"))
                    {
                        // Filens headers kommer att innehålla "filename" för att indikera att det är en fil
                        var fileName = GetFileName(section);
                        var fileContent = GetFileContent(section);

                        // Ladda upp filen till blob-lagring
                        _blobClient = _container.GetBlobClient(fileName);
                        await using var stream = new MemoryStream(fileContent);
                        await _blobClient.UploadAsync(stream, overwrite: true);

                        // Returnera en respons med den uppladdade filens URI
                        return new OkObjectResult(_blobClient.Uri);
                    }
                }
            }

            // Om ingen fil hittades, returnera en felmeddelande
            return new BadRequestObjectResult("No file received");
        }

        // Hjälpmetoder för att bearbeta multipart/form-data

        private string GetBoundary(string contentType)
        {
            var elements = contentType.Split(' ');
            var element = elements.FirstOrDefault(e => e.StartsWith("boundary="));
            return "--" + element.Substring("boundary=".Length);
        }

        private string GetFileName(string section)
        {
            var fileNameRegex = new Regex(@"(?<=filename=)[^\s;]+");
            var match = fileNameRegex.Match(section);
            return match.Success ? match.Value : string.Empty;
        }



        private byte[] GetFileContent(string section)
        {
            string contentStart = "\r\n\r\n";
            var startIndex = section.IndexOf(contentStart) + contentStart.Length;
            return Encoding.ASCII.GetBytes(section.Substring(startIndex).Trim('\r', '\n', '-'));
        }



    }
}

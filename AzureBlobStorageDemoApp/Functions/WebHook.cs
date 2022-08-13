using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AzureBlobStorageDemoApp.Helpers;

namespace AzureBlobStorageDemoApp.Functions
{
    public class WebHook
    {
        private readonly ILogger<WebHook> logger;
        private readonly IBlobService blobService;

        public WebHook(ILogger<WebHook> logger,
            IBlobService blobService)
        {
            this.logger = logger;
            this.blobService = blobService;
        }

        [FunctionName(nameof(WebHook))]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] 
        HttpRequest req)
        {
            logger.LogInformation("{class} - {method} - Start", nameof(WebHook), nameof(WebHook.RunAsync));

            logger.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var result = blobService.UploadAsync("{container-name}","{file-name}", requestBody);

            logger.LogInformation($"file successfully upload into blob storage");

            logger.LogInformation("{class} - {method} - End", nameof(WebHook), nameof(WebHook.RunAsync));

            return new OkObjectResult("file successfully uploaded into blob storage");
        }
    }
}

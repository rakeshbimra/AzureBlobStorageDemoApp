using Azure.Storage.Blobs;
using AzureBlobStorageDemoApp.Helpers.Dtos;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureBlobStorageDemoApp.Helpers
{
    /// <summary>
    /// Blob service - Upload/Download files into Azure blob storage
    /// </summary>
    public class BlobService : IBlobService
    {
        private readonly ILogger<BlobService> logger;
        private readonly BlobServiceClient blobServiceClient;

        public BlobService(ILogger<BlobService> logger,
            BlobServiceClient blobServiceClient)
        {
            this.logger = logger;
            this.blobServiceClient = blobServiceClient;
        }

        /// <summary>
        /// Download file from azure blob storage with blob uri
        /// </summary>
        /// <param name="containerName">Blob storage container name</param>
        /// <param name="fileUri">Blob storage file uri</param>
        /// <returns>Byte[]</returns>
        public async Task<byte[]> DownloadAsync(string containerName, string fileUri)
        {
            logger.LogInformation("{class} - {method} - Start", nameof(BlobService), nameof(BlobService.DownloadAsync));

            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            var blobUri = new System.Uri(fileUri);

            var name = new CloudBlockBlob(blobUri).Name;

            var blobClient = containerClient.GetBlobClient(name);

            var response = await blobClient.DownloadStreamingAsync();

            using MemoryStream ms = new MemoryStream();

            await response.Value.Content.CopyToAsync(ms);

            ms.Position = 0;

            logger.LogInformation("{class} - {method} - End", nameof(BlobService), nameof(BlobService.DownloadAsync));

            return ms.ToArray();
        }

        /// <summary>
        /// Upload file into azure blob storage
        /// </summary>
        /// <param name="containerName">Azure blob storage container name</param>
        /// <param name="filename">Azure blob storage file name</param>
        /// <param name="content">file content</param>
        /// <returns>BlobDto</returns>
        public async Task<BlobDto> UploadAsync(string containerName, string filename, string content)
        {
            logger.LogInformation("{class} - {method} - Start", nameof(BlobService), nameof(BlobService.UploadAsync));

            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            var blobClient = containerClient.GetBlobClient(filename);

            var bytes = Encoding.UTF8.GetBytes(content);

            await using var memoryStream = new MemoryStream(bytes);

            await blobClient.UploadAsync(content);

            logger.LogInformation("{class} - {method} - End", nameof(BlobService), nameof(BlobService.UploadAsync));

            return new BlobDto
            {
                Uri = blobClient.Uri,
                Name = blobClient.Name
            };
        }
    }
}

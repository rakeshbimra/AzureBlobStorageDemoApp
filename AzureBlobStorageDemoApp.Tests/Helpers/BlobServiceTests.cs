using Azure;
using Azure.Storage.Blobs;
using AzureBlobStorageDemoApp.Helpers;
using AzureBlobStorageDemoApp.Helpers.Dtos;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;

namespace AzureBlobStorageDemoApp.Tests.Helpers
{
    [TestClass]
    public class BlobServiceTests
    {
        private BlobService blobService;
        private Mock<ILogger<BlobService>> loggerMock;
        private Mock<BlobServiceClient> blobServiceClientMock;

        [TestInitialize]
        public void Initialize()
        {
            loggerMock = new Mock<ILogger<BlobService>>();
            blobServiceClientMock = new Mock<BlobServiceClient>();
            blobService = new BlobService(loggerMock.Object, blobServiceClientMock.Object);
        }

        [TestMethod]
        public async Task DownloadAsync_ValidInputs_ReturnsByteArray()
        {
            // Arrange
            var containerName = "test-container";
            var fileUri = "https://test.blob.core.windows.net/test-container/test-blob.txt";
            var containerClientMock = new Mock<BlobContainerClient>();
            var blobClientMock = new Mock<BlobClient>();
            var downloadResponseMock = new Mock<Response>();
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes("test content"));
            var expectedBytes = memoryStream.ToArray();

            containerClientMock.Setup(c => c.GetBlobClient(It.IsAny<string>())).Returns(blobClientMock.Object);
            blobClientMock.Setup(b => b.DownloadStreamingAsync()).ReturnsAsync(Response.FromValue(new BlobDownloadStreamingResponse(memoryStream), null));

            blobServiceClientMock.Setup(b => b.GetBlobContainerClient(containerName)).Returns(containerClientMock.Object);

            // Act
            var result = await blobService.DownloadAsync(containerName, fileUri);

            // Assert
            CollectionAssert.AreEqual(expectedBytes, result);
            loggerMock.Verify(
                x => x.LogInformation(
                    It.Is<string>(s => s.Contains(nameof(BlobService.DownloadAsync))),
                    It.IsAny<object[]>()
                ), Times.Once);
        }

        [TestMethod]
        public async Task UploadAsync_ValidInputs_ReturnsBlobDto()
        {
            // Arrange
            var containerName = "test-container";
            var filename = "test-blob.txt";
            var content = "test content";
            var blobUri = new Uri($"https://test.blob.core.windows.net/{containerName}/{filename}");
            var blobClientMock = new Mock<BlobClient>();
            var expectedBlobDto = new BlobDto { Uri = blobUri, Name = filename };

            blobServiceClientMock.Setup(b => b.GetBlobContainerClient(containerName)).Returns(new Mock<BlobContainerClient>().Object);
            blobServiceClientMock.Setup(b => b.GetBlobContainerClient(containerName).GetBlobClient(filename)).Returns(blobClientMock.Object);
            blobClientMock.Setup(b => b.UploadAsync(content)).Returns(Task.CompletedTask);
            blobClientMock.SetupGet(b => b.Uri).Returns(blobUri);

            // Act
            var result = await blobService.UploadAsync(containerName, filename, content);

            // Assert
            Assert.AreEqual(expectedBlobDto.Name, result.Name);
            Assert.AreEqual(expectedBlobDto.Uri, result.Uri);
            loggerMock.Verify(
                x => x.LogInformation(
                    It.Is<string>(s => s.Contains(nameof(BlobService.UploadAsync))),
                    It.IsAny<object[]>()
                ), Times.Once);
        }
    }

}

using AzureBlobStorageDemoApp.Helpers.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureBlobStorageDemoApp.Helpers
{
    public interface IBlobService
    {
        /// <summary>
        /// Download file from azure blob storage with blob uri
        /// </summary>
        /// <param name="containerName">Blob storage container name</param>
        /// <param name="fileUri">Blob storage file uri</param>
        /// <returns>Byte[]</returns>
        Task<byte[]> DownloadAsync(string containerName, string fileUri);

        /// <summary>
        /// Upload file into azure blob storage
        /// </summary>
        /// <param name="containerName">Azure blob storage container name</param>
        /// <param name="filename">Azure blob storage file name</param>
        /// <param name="content">file content</param>
        /// <returns>BlobDto</returns>
        Task<BlobDto> UploadAsync(string containerName, string filename, string content);
    }
}

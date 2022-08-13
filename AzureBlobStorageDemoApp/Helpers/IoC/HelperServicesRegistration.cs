using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureBlobStorageDemoApp.Helpers.IoC
{
    public static class HelperServicesRegistration
    {
        public static void AddHelpers(this IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton(x =>
            new BlobServiceClient(connectionString: Environment.GetEnvironmentVariable("AzureWebJobsStorage")));

            builder.Services.AddSingleton<IBlobService, BlobService>();
        }
    }
}

using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ZipFunction.util
{
    /// <summary>
    /// Azure Blob Storage utilities
    /// </summary>
    internal static class BlobUtil
    {
        /// <summary>
        /// Returns a blob client 
        /// </summary>
        /// <param name="_containerpath">blob path</param>
        /// <returns></returns>
        internal static BlobContainerClient getBlobContainer(string _containerpath)
        {
            string connectionString = System.Environment.GetEnvironmentVariable("StorageAccountConnectionString");

            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_containerpath);

            return containerClient;
        }

        /// <summary>
        /// Creates a file in a blob storage container
        /// </summary>
        /// <param name="_name">File name</param>
        /// <param name="_data"></param>
        /// <param name="_containerpath">path to the container</param>
        /// <param name="log"></param>
        /// <returns></returns>
        internal static async Task CreateBlob(string _name, string _data, string _containerpath, ILogger log)
        {

            BlobContainerClient container;
            BlobClient blob;

            container = getBlobContainer(_containerpath);

            await container.CreateIfNotExistsAsync();

            blob = container.GetBlobClient(_name);

        }

        /// <summary>
        /// Get an specific Blob
        /// </summary>
        /// <param name="_name">blob name</param>
        /// <param name="_containerpath">blob path</param>
        /// <param name="log"></param>
        /// <returns></returns>
        internal static BlobClient getBlob(string _name, string _containerpath, ILogger log)
        {
            return getBlobContainer(_containerpath).GetBlobClient(_name);
        }

    }
}

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ICSharpCode.SharpZipLib.Zip;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Net;

namespace CompressAzureBlobStorageV2
{
    public static class CreateZipPackageV2
    {
        [FunctionName("CreateZipPackageV2")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            
            BlobClient zipblob;
            string containerpath;
            string zipfilename;

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            containerpath = data?.containerpath;
            zipfilename = data?.zipfilename;
            JArray filelist = JArray.Parse(@data["filelist"].ToString());

            try
            {
                await CreateBlob(zipfilename, "", containerpath, log);
                zipblob = getBlob(zipfilename, containerpath, log);
                await zipFiles(zipblob, containerpath, filelist);

                return new OkObjectResult("");

            }
            catch (Exception e)
            {
                return new BadRequestObjectResult("An unhandled exception has ocurred: " + e.Message);
            }
        }


        private static BlobContainerClient getBlobContainer(string _containerpath)
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
        /// <param name="log"file content></param>
        /// <returns></returns>
        private static async Task CreateBlob(string _name, string _data, string _containerpath, ILogger log)
        {

            BlobContainerClient container;
            BlobClient blob;

            container = getBlobContainer(_containerpath);

            await container.CreateIfNotExistsAsync();

            blob = container.GetBlobClient(_name);

        }

        private static BlobClient getBlob(string _name, string _containerpath, ILogger log)
        {
            return getBlobContainer(_containerpath).GetBlobClient(_name);
        }

        /// <summary>
        /// Create a .zip file containing all the files in blobFileNames
        /// </summary>
        /// <param name="zipblob">Instance to a BlobClient</param>
        /// <param name="containerpath">the path to the container where the files are</param>
        /// <param name="blobFileNames">A json array containing the list of files to add to the .zip file</param>
        /// <returns></returns>
        public static async Task zipFiles(BlobClient zipblob, string containerpath, JArray blobFileNames)
        {
            var container = getBlobContainer(containerpath);

            MemoryStream tmpzipstream = new MemoryStream();

            ZipOutputStream zipOutputStream = new ZipOutputStream(tmpzipstream);

            foreach (string blobFileName in blobFileNames)
            {
                string blobFileNameOrig, blobFileNameDest;
                
                blobFileNameOrig = blobFileName.Split(',')[0].Replace("/" + containerpath + "/", "");

                if (blobFileName.Contains(","))
                    blobFileNameDest = blobFileName.Split(',')[1];
                else
                    blobFileNameDest = blobFileNameOrig;
           
                zipOutputStream.SetLevel(0);

                var blob = container.GetBlobClient(blobFileNameOrig);
                
                // skip Lenght 0 files
                BlobProperties properties = await blob.GetPropertiesAsync();
                if (properties.ContentLength <= 0)
                {
                    continue;
                }

                var entry = new ZipEntry(blobFileNameDest)
                {
                    DateTime = DateTime.Now
                };

                zipOutputStream.PutNextEntry(entry);

                await blob.DownloadToAsync(zipOutputStream);

            }

            zipOutputStream.Finish();

            tmpzipstream.Seek(0, SeekOrigin.Begin);
            await zipblob.UploadAsync(tmpzipstream);

            
            zipOutputStream.Close();


        }


    }
}

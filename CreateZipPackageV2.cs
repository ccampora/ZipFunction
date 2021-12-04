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

            if (string.IsNullOrEmpty(requestBody))
            {
                return new BadRequestObjectResult("Body cannot be blank");
            }

            if(!ZipFunction.util.SchemaValidator.validateZipFileRequestBody(requestBody))
            {
                return new BadRequestObjectResult("Schema validation failed. A valid example is: \n" +
                    ZipFunction.util.SchemaValidator.zipFileRequestBodyExample());
            }

            dynamic data = JsonConvert.DeserializeObject(requestBody);

            containerpath = data?.containerpath;

            if (string.IsNullOrEmpty(containerpath))
            {
                return new BadRequestObjectResult("containerpath value cannot be blank");
            }

            zipfilename = data?.zipfilename;

            if (string.IsNullOrEmpty(zipfilename))
            {
                return new BadRequestObjectResult("zipfilename value cannot be blank");
            }

            JArray filelist = JArray.Parse(@data["filelist"].ToString());

            if(filelist.Count == 0)
            {
                return new BadRequestObjectResult("filelist count needs to be major than 0");
            }

            try
            {
                await ZipFunction.util.BlobUtil.CreateBlob(zipfilename, "", containerpath, log);
                zipblob = ZipFunction.util.BlobUtil.getBlob(zipfilename, containerpath, log);
                await zipFiles(zipblob, containerpath, filelist);

                return new OkObjectResult("");

            }
            catch (Exception e)
            {
                return new BadRequestObjectResult("An unhandled exception has ocurred: " + e.Message);
            }
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
            var container = ZipFunction.util.BlobUtil.getBlobContainer(containerpath);

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

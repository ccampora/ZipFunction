using System;
using System.Text;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using System.Threading.Tasks;

namespace ZipFunction.util
{
    /// <summary>
    /// This class checks a .zip file on a blob container 
    /// </summary>
    internal static class PackageChecker
    {
        // Define file extensions to search
        static readonly string[] fileExtensions = { ".csv", ".xsls"};

        /// <summary>
        /// When calling ImportToPackage API function it will accept a package regardless in the package contains valid entity data.
        /// If the package only contains the Metadata.xml and PackageHeader.xml files, the API will accept the request and return an executionId (IMO this case should trigget an API exception), 
        /// however the mapping will be done but won't show on the DMFExecution list, hence is not possible to capture this exception properly.
        /// This function purpose is to look at the .zip package content and look for at least one data file. 
        /// </summary>
        /// <param name="_containerpath">path of the file to validate</param>
        /// <param name="_filename">name of the .zip file</param>
        /// <returns>true if the package is valid, false if not</returns>
        public static async Task<bool> validatePackageAsync(string _containerpath, string _filename)
        {
            if (string.IsNullOrEmpty(_containerpath))
            { 
                return false;
            }

            var container = BlobUtil.getBlobContainer(_containerpath);
            var blobClient = container.GetBlobClient(_filename);
            
            if (!await blobClient.ExistsAsync())
            {
                return false;
            }
                        
            MemoryStream tmpzipstream = new MemoryStream();
            await blobClient.DownloadToAsync(tmpzipstream);  
            tmpzipstream.Position = 0;        

            using (ZipInputStream s = new ZipInputStream(tmpzipstream))
            {
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null) {
                    if(validateFile(theEntry.Name))
                    {
                        Console.WriteLine("Name : {0}", theEntry.Name);                    
                        return true;
                    }
                    
                }                
                s.Close();
            }            
            return false;
        }
        
        /// <summary>
        /// Validates filename contains at least one of the valid extensions
        /// </summary>
        /// <param name="_fileNamesArray">filename to validate</param>
        public static bool validateFile(string _filename)
        {
            foreach(string ext in fileExtensions)
            {
                if(_filename.Contains(ext))
                {
                    return true;
                }
            }
            return false;
        }
    }
}

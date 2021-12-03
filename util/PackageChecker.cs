using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZipFunction.util
{
    /// <summary>
    /// This class checks a .zip file on a blob container 
    /// </summary>
    internal class PackageChecker
    {
        /// <summary>
        /// When calling ImportToPackage API function it will accept a package regardless in the package contains valid entity data.
        /// If the package only contains the Metadata.xml and PackageHeader.xml files, the API will accept the request and return an executionId (IMO this case should trigget an API exception), 
        /// however the mapping will be done but won't show on the DMFExecution list, hence is not possible to capture this exception properly.
        /// This function purpose is to look at the .zip package content and look for at least one data file. 
        /// </summary>
        /// <param name="_containerpath">path of the file to validate</param>
        /// <param name="_fileExtensions">string array of extensions to search. Example ['.csv', '.xlsx']. If null will default to look for .csv extensions</param>
        /// <returns></returns>
        public static bool validatePackage(string _containerpath, string[] _fileExtensions)
        {
            if (string.IsNullOrEmpty(_containerpath))
            { 
                return false;
            }

            if(_fileExtensions.Length == 0)
            {
                Array.Resize(ref _fileExtensions, 1);
                _fileExtensions[0] = ".csv"; // default extension
            }
            return false;

        }
    }
}

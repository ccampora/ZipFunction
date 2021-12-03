using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System.Threading.Tasks;

namespace ZipFunction.util
{
    internal class SchemaValidator
    {
        /// <summary>
        /// Validates the against requestBodySchema schema
        /// </summary>
        /// <returns></returns>
        public static bool validateZipFileRequestBody(string message)
        {
            var requestBodySchema = @"
            {
	            ""type"": ""object"",
	            ""required"": [

                    ""containerpath"",
		            ""filelist"",
		            ""zipfilename""
	            ],
	            ""properties"": {
                    ""containerpath"": {
                    ""type"": ""string""
                },
		        ""filelist"": {
			        ""type"": ""array"",
			        ""items"":{                       
				        ""type"": ""string"",
				   }
                },
		        ""zipfilename"": {
			        ""type"": ""string"",
			    }
            }
            }";

            var schema = JSchema.Parse(requestBodySchema);

            if (schema == null)
            {
                return false;
            }

            JObject msg = JObject.Parse(message);

            bool valid = msg.IsValid(schema);

            return valid;
        }

        public static string zipFileRequestBodyExample()
        {
            return @"
                    {
                        ""containerpath"": ""container01"",
                        ""filelist"": [
                            ""in/2021071156857-08585755930712445680477052973CU30-SalesOrderHeaderV2Entity.csv,SalesOrderHeaderV2Entity.csv"",
                            ""in/2021071156857-08585755930712445680477052973CU30-SalesOrderLineV2Entity.csv,SalesOrderLineV2Entity.csv"",
                            ""config-files/salesorders/Manifest.xml,Manifest.xml"",
                            ""config-files/salesorders/PackageHeader.xml,PackageHeader.xml""
                        ],
                        ""zipfilename"": ""in/zipFile/2021071156857-08585755930712445680477052973CU30.zip""
                    }
                    ";
        }

    }
}

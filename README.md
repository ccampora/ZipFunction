# Deploy to Azure

<a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fccampora%2FZipFunction%2Fmain%2Fdeployment%2Fdeploytoazure.json" target="_blank"><img src="https://azuredeploy.net/deploybutton.png"/></a>

# ZipFunction
Function to zip files from a blob Storage. The initial purpose of this function is to use it as part of a Logic Apps flow. 

Create the parameter StorageAccountConnectionString for your own storage connection string from the Azure Blob Storage account

![image](https://user-images.githubusercontent.com/7789650/125275895-ddcac480-e30f-11eb-904d-b7d924b4139e.png)


Input message example: 

```
{
  "containerpath": "container01", 
  "filelist": [
    "in/2021071156857-08585755930712445680477052973CU30-SalesOrderHeaderV2Entity.csv,SalesOrderHeaderV2Entity.csv",
    "in/2021071156857-08585755930712445680477052973CU30-SalesOrderLineV2Entity.csv,SalesOrderLineV2Entity.csv",
    "config-files/salesorders/Manifest.xml,Manifest.xml",
    "config-files/salesorders/PackageHeader.xml,PackageHeader.xml"
  ],
  "zipfilename": "in/zipFile/2021071156857-08585755930712445680477052973CU30.zip"
}
```

the example above will take zip all the files listed in "filelist" array in a file name "zipfilename". It will also rename each of the files if specified. 

Also, read this post: https://ccampora.blogspot.com/2021/12/creating-dmf-data-package-from-files.html

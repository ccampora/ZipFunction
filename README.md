# ZipFunction
Function to zip files from a blob Storage. The initial purpose of this function is to use it as part of a Logic Apps flow. 

# Installation 

## Deploy to Azure

<a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fccampora%2FZipFunction%2Fmain%2Fdeployment%2Fdeploytoazure.json" target="_blank"><img src="https://azuredeploy.net/deploybutton.png"/></a>

![image](https://user-images.githubusercontent.com/7789650/144675752-9c75be14-82b3-4001-bc8f-8f3ece6a3fec.png)

After the deployment is sucessful 

![image](https://user-images.githubusercontent.com/7789650/144679310-e711b7e7-c75d-439e-a2cb-ed942f32fa31.png)


## Configure a new Blob Storage Account

- Create a new Azure Blob Storage Account 
-- Yes. Another account. Do not re-use the account created by the Azure functions deployment. 
- Create a new container.
- Go to Access Keys and obtain a Connection String
- Create the parameter StorageAccountConnectionString and input the value obtain in the previous step

![image](https://user-images.githubusercontent.com/7789650/125275895-ddcac480-e30f-11eb-904d-b7d924b4139e.png)

# Testing 


Drop some files to zip into the container. 

Go to the Azure function and select Code + Test 

![image](https://user-images.githubusercontent.com/7789650/144681245-90929faa-54bb-4e3a-8181-eb35ef2516f9.png)

Replace the body with a proper message. See an example below 

![image](https://user-images.githubusercontent.com/7789650/144681348-6d2728b7-8e4f-424b-8af2-fa1bad7ea189.png)

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

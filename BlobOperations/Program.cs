﻿using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobOperations
{
    class Program
    {
        static void Main(string[] args)
        {
            BasicBlobOperation().GetAwaiter().GetResult();

            Console.Read();
        }


        private static async Task BasicBlobOperation()
        {            
            const string ImageToUplaod = "Cloud.xml";
            const string connectionString = "DefaultEndpointsProtocol=https;AccountName=az203sa;AccountKey=WF9Ojv2ghJwFuQ9YNuKJPRBpPotvRbhN4ScJaGy7qLBuXgAJgQVej/AVfu3MWTGxzxBp1pk7jqT5yJ33BwWcDg==;EndpointSuffix=core.windows.net";

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference("newcontainer");
            await container.CreateIfNotExistsAsync().ConfigureAwait(false);

           CloudBlobDirectory dir =  container.GetDirectoryReference("vsdir");
           CloudBlockBlob blockBlob = dir.GetBlockBlobReference(ImageToUplaod);
            await blockBlob.UploadFromFileAsync(ImageToUplaod);

           var blobs = container.ListBlobs("vsdir",true);

            blockBlob.Metadata["Author"] = "Visual Studio";
            blockBlob.Metadata["Priority"] = "High";

            await blockBlob.SetMetadataAsync();

            //await HandleConCurrencyForUpdate(blockBlob, ConcurrencyType.Default);
            //await HandleConCurrencyForUpdate(blockBlob, ConcurrencyType.Optimistic);
            await HandleConCurrencyForUpdate(blockBlob, ConcurrencyType.Pessimistic);

            //await blockBlob.DownloadToFileAsync("copyof" + ImageToUplaod, System.IO.FileMode.Create);

            //await blockBlob.DeleteAsync();

        }

        private static async Task<bool> HandleConCurrencyForUpdate(CloudBlockBlob blockBlob, ConcurrencyType type)
        {
            bool result = false;
            switch(type)
            {
                case ConcurrencyType.Default:
                    blockBlob.Metadata["Author"] = "Default Update";
                    blockBlob.Metadata["Priority"] = "High";
                    await blockBlob.SetMetadataAsync();
                    break;
                case ConcurrencyType.Optimistic:
                    blockBlob.Metadata["Author"] = "Optimistic Update";
                    blockBlob.Metadata["Priority"] = "High";
                    var accessCondition = new AccessCondition
                    {
                        IfMatchETag = blockBlob.Properties.ETag
                    };
                    try
                    {
                        await blockBlob.SetMetadataAsync(accessCondition, null, null);
                    }
                    catch(Exception exe)
                    {
                        result = false;
                    }
                    
                    break;
                case ConcurrencyType.Pessimistic:
                    blockBlob.Metadata["Author"] = "Optimistic Update";
                    blockBlob.Metadata["Priority"] = "High";
                    string leaseId = await blockBlob.AcquireLeaseAsync(TimeSpan.FromMinutes(1));
                    var accessConditionPessimistic = new AccessCondition
                    {
                        LeaseId = leaseId
                    };
                    try
                    {
                        await blockBlob.SetMetadataAsync(accessConditionPessimistic, null, null);
                    }
                    catch(Exception exe)
                    {
                        result = false;
                    }

                    //await blockBlob.BreakLeaseAsync(TimeSpan.FromSeconds(0));

                    await blockBlob.ReleaseLeaseAsync(accessConditionPessimistic);
                    break;
            }

            return result;
        }
    }
}

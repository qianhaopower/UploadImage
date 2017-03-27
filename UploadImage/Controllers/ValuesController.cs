using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace UploadImage.Controllers
{
    [RoutePrefix("api/values")]
    public class ValuesController : ApiController
    {
        [HttpGet]
        [Route("est")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        private void UploadToBlobStorage(System.IO.FileStream stream)
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("StorageKey");

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference("mycontainer");

            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference("myblob");

            // Create or overwrite the "myblob" blob with contents from a local file.
            using (var fileStream = System.IO.File.OpenRead(@"path\myfile"))
            {
                blockBlob.UploadFromStream(stream);
            }
        }
        [HttpPost]
        [Route("postimage")]
        public async Task<HttpResponseMessage> PostFormData()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            try
            {
                MultipartStreamProvider provider = new BlobStorageMultipartStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    


   
        //public async Task<HttpResponseMessage> PostImage()
        //{
        //    Dictionary<string, object> dict = new Dictionary<string, object>();
        //    try
        //    {

        //        var httpRequest = HttpContext.Current.Request;

        //        foreach (string file in httpRequest.Files)
        //        {
        //            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);

        //            var postedFile = httpRequest.Files[file];
        //            if (postedFile != null && postedFile.ContentLength > 0)
        //            {

        //                int MaxContentLength = 1024 * 1024 * 1; //Size = 1 MB  

        //                IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
        //                var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
        //                var extension = ext.ToLower();
        //                if (!AllowedFileExtensions.Contains(extension))
        //                {

        //                    var message = string.Format("Please Upload image of type .jpg,.gif,.png.");

        //                    dict.Add("error", message);
        //                    return Request.CreateResponse(HttpStatusCode.BadRequest, dict);
        //                }
        //                else if (postedFile.ContentLength > MaxContentLength)
        //                {

        //                    var message = string.Format("Please Upload a file upto 1 mb.");

        //                    dict.Add("error", message);
        //                    return Request.CreateResponse(HttpStatusCode.BadRequest, dict);
        //                }
        //                else
        //                {
        //                   // this.UploadToBlobStorage(postedFile.InputStream);


        //                    //var filePath = HttpContext.Current.Server.MapPath("~/Userimage/" + postedFile.FileName + extension);

        //                    //postedFile.SaveAs(filePath);

        //                }
        //            }

        //            var message1 = string.Format("Image Updated Successfully.");
        //            return Request.CreateErrorResponse(HttpStatusCode.Created, message1); ;
        //        }
        //        var res = string.Format("Please Upload a image.");
        //        dict.Add("error", res);
        //        return Request.CreateResponse(HttpStatusCode.NotFound, dict);
        //    }
        //    catch (Exception ex)
        //    {
        //        var res = string.Format("some Message");
        //        dict.Add("error", res);
        //        return Request.CreateResponse(HttpStatusCode.NotFound, dict);
        //    }
        //}
    }

    public class BlobStorageMultipartStreamProvider : MultipartStreamProvider
    {
        public override Stream GetStream(HttpContent parent, HttpContentHeaders headers)
        {
            Stream stream = null;
            ContentDispositionHeaderValue contentDisposition = headers.ContentDisposition;
            if (contentDisposition != null)
            {
                if (!String.IsNullOrWhiteSpace(contentDisposition.FileName))
                {
                    string connectionString = ConfigurationManager.AppSettings[""];
                    string containerName = ConfigurationManager.AppSettings["image"];
                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
                    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                    CloudBlobContainer blobContainer = blobClient.GetContainerReference(containerName);

                    CloudBlockBlob blob = blobContainer.GetBlockBlobReference(contentDisposition.FileName);
                    stream = blob.OpenWrite();
                }
            }
            return stream;
        }
    }


    
}

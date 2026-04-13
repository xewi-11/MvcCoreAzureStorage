using Microsoft.AspNetCore.Mvc;
using MvcCoreAzureStorage.Models;
using MvcCoreAzureStorage.Services;

namespace MvcCoreAzureStorage.Controllers
{
    public class AzureBlobController : Controller
    {
        private ServiceStorageBlobs service;

        public AzureBlobController(ServiceStorageBlobs service)
        {
            this.service = service;
        }

        public async Task<IActionResult> Index()
        {
            List<string> containers = await this.service.GetContainerASync();
            return View(containers);
        }

        public IActionResult CreateContainer()
        {
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> CreateContainer(string containerName)
        {
            await this.service.CreateContainerAsync(containerName);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> DeleteContainer(string containerName)
        {
            await this.service.DeleteContainerAsync(containerName);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ListBlobs(string containerName)
        {
            List<BlobModel> blobs = await this.service.GetBlobsAsync(containerName);

            string rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagenes");
            if (Directory.Exists(rootPath))
            {
                Directory.Delete(rootPath, true);
            }
            Directory.CreateDirectory(rootPath);

            foreach (BlobModel blob in blobs)
            {
                var blobData = await this.service.GetBlobStreamAsync(containerName, blob.Nombre);
                string localFileName = blob.Nombre.Replace("/", "_").Replace("\\", "_");
                string localPath = Path.Combine(rootPath, localFileName);

                await using FileStream fileStream = new FileStream(localPath, FileMode.Create, FileAccess.Write);
                await blobData.Stream.CopyToAsync(fileStream);

                blob.Url = $"/imagenes/{localFileName}";
            }

            ViewData["CONTAINER"] = containerName;
            return View(blobs);
        }

        public IActionResult UploadBlob(string containerName)
        {
            ViewData["CONTAINER"] = containerName;
            return View();
        }
        [HttpPost]

        public async Task<IActionResult> UploadBlob(string containerName, IFormFile file)
        {
            string blobName = file.FileName;
            using (Stream stream = file.OpenReadStream())
            {
                await this.service.UploadloadAsync(containerName, blobName, stream);
            }
            return RedirectToAction("ListBlobs", new { containerName = containerName });
        }

        public async Task<IActionResult> DeleteBlob(string containerName, string blobName)
        {
            await this.service.DeleteBlobAsync(containerName, blobName);
            return RedirectToAction("ListBlobs", new { containerName = containerName });

        }

        public async Task<IActionResult> GetBlobFile(string containerName, string blobName)
        {
            var blobData = await this.service.GetBlobStreamAsync(containerName, blobName);
            return File(blobData.Stream, blobData.ContentType);
        }

        public async Task<IActionResult> DownloadBlobFile(string containerName, string blobName)
        {
            var blobData = await this.service.GetBlobStreamAsync(containerName, blobName);
            return File(blobData.Stream, blobData.ContentType, blobName);
        }

        [HttpPost]
        public IActionResult CleanupImages()
        {
            string rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagenes");
            if (Directory.Exists(rootPath))
            {
                Directory.Delete(rootPath, true);
            }
            return Ok();
        }
    }
}

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MvcCoreAzureStorage.Models;

namespace MvcCoreAzureStorage.Services
{
    public class ServiceStorageBlobs
    {
        private BlobServiceClient client;
        public ServiceStorageBlobs(BlobServiceClient client)
        {
            this.client = client;
        }

        //metodo para recuperar todos los contenedores
        public async Task<List<string>> GetContainerASync()
        {
            List<string> containers = new List<string>();
            await foreach (BlobContainerItem container in this.client.GetBlobContainersAsync())
            {
                containers.Add(container.Name);
            }
            return containers;
        }

        //metodo para crear un contenedor

        public async Task CreateContainerAsync(string containerName)
        {
            await this.client.CreateBlobContainerAsync(containerName.ToLower(), PublicAccessType.None);
        }
        //ELIMINAR CONTENEDOR
        public async Task DeleteContainerAsync(string containerName)
        {
            await this.client.DeleteBlobContainerAsync(containerName.ToLower());
        }

        //LISTADO FICHEROS DNTRO DE UN CONTAINER

        public async Task<List<BlobModel>> GetBlobsAsync(string containerName)
        {
            //necesitamos un client de blobs para el acceso a los ficheros
            BlobContainerClient containerClient = this.client.GetBlobContainerClient(containerName.ToLower());

            List<BlobModel> models = new List<BlobModel>();

            await foreach (BlobItem item in containerClient.GetBlobsAsync())
            {
                BlobClient blobClient = containerClient.GetBlobClient(item.Name);
                BlobModel blob = new BlobModel();
                blob.Nombre = item.Name;
                blob.Container = containerName;
                blob.Url = blobClient.Uri.AbsoluteUri;
                models.Add(blob);
            }
            return models;
        }
        //ELIMINAR UN  BLOB

        public async Task DeleteBlobAsync(string continerName, string blobName)
        {
            BlobContainerClient blobContainerClient = this.client.GetBlobContainerClient(continerName);
            await blobContainerClient.DeleteBlobAsync(blobName);
        }
        //subir un blob a un container

        public async Task UploadloadAsync(string continerName, string blobName, Stream stream)
        {
            BlobContainerClient containerClient = this.client.GetBlobContainerClient(continerName);
            await containerClient.UploadBlobAsync(blobName, stream);
        }

        public async Task<(Stream Stream, string ContentType)> GetBlobStreamAsync(string containerName, string blobName)
        {
            BlobContainerClient containerClient = this.client.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            BlobDownloadInfo download = await blobClient.DownloadAsync();
            string contentType = download.ContentType ?? "application/octet-stream";
            return (download.Content, contentType);
        }
    }
}

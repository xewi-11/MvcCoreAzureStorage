using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;

namespace MvcCoreAzureStorage.Services
{
    public class ServiceStorageFiles
    {
        //TODO SERVICIO STORAGR TARABAJA SIEMPRE CON UN CLIENT
        //DICHO CLIENTE PUEDE SER DIRECTAMENTE UN SHARED O
        //PODRIA SER EL SERVICIO DE FILES

        private ShareDirectoryClient root;

        public ServiceStorageFiles(IConfiguration configuration)
        {
            string azureKeys = configuration.GetValue<string>("AzureKeys:StorageAccount");

            //NUESTRO CLIENTE TRABAJA SOBRE EL SHARED QUE HEMOS CREADO PREVIAMNETE (EJEMPLO)

            ShareClient client = new ShareClient(azureKeys, "ejemplo");

            this.root = client.GetRootDirectoryClient();
        }

        //listar todos los ficheros
        public async Task<List<string>> GetFilesAsync()
        {
            List<string> files = new List<string>();

            await foreach (ShareFileItem item in this.root.GetFilesAndDirectoriesAsync())
            {
                files.Add(item.Name);
            }
            return files;
        }

        //leer contenido de file

        public async Task<string> ReadFileAsync(string fileName)
        {
            ShareFileClient fileClient = this.root.GetFileClient(fileName);

            ShareFileDownloadInfo data = await fileClient.DownloadAsync();
            Stream stream = data.Content;

            string contenido = "";
            using (StreamReader reader = new StreamReader(stream))
            {
                contenido = await reader.ReadToEndAsync();

            }
            return contenido;
        }

        public async Task UploadFileAsync(string fileName, Stream stream)
        {
            ShareFileClient client = this.root.GetFileClient(fileName);
            await client.CreateAsync(stream.Length);
            await client.UploadAsync(stream);
        }

        public async Task DeleteFileASync(string filename)
        {
            ShareFileClient client = this.root.GetFileClient(filename);
            await client.DeleteAsync();
        }
    }
}

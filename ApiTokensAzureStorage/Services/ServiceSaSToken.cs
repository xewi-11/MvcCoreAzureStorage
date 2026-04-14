using Azure.Data.Tables;
using Azure.Data.Tables.Sas;

namespace ApiTokensAzureStorage.Services
{
    public class ServiceSaSToken


    {
        private TableClient tableClient;

        public ServiceSaSToken(IConfiguration configuration)
        {
            string azureKey = configuration.GetValue<string>("AzureKeys:StorageAccount");
            TableServiceClient serviceClient = new TableServiceClient(azureKey);
            this.tableClient = serviceClient.GetTableClient("alumnos");
        }

        public string GenerateToken(string curso)
        {
            //NECESITAMOS LOS PERMISOS DE ACCESO PARA EL TOKEN, EN ESTE CASO SOLO LECTURA

            TableSasPermissions permisos = TableSasPermissions.Read;

            //EL ACCESO A TOKEN ESTA DELIMITADO POR UN TIEMPO DETERMINADO

            TableSasBuilder builder = this.tableClient.GetSasBuilder(permisos, DateTimeOffset.UtcNow.AddMinutes(30));

            //EL ACCESO A LOS DATOS ES MEDIANTE ROW KEY O PARTITION KEY, SON STRING Y VAN DE FORMA ALFABETICA

            builder.PartitionKeyStart = curso;
            builder.PartitionKeyEnd = curso;
            //YA TENEMOS EL TOKEN QUE ES UN ACCESO MEDIANTE URI

            Uri uriToken = this.tableClient.GenerateSasUri(builder);
            string token = uriToken.AbsoluteUri;
            return token;
        }
    }
}

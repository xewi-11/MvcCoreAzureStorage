using Azure;
using Azure.Data.Tables;

namespace MvcCoreAzureStorage.Models
{
    public class Cliente : ITableEntity
    {

        public string Nombre { get; set; }
        public int Edad { get; set; }
        public int Salario { get; set; }

        //Id CLiente: ROW KEY
        //CUANDO EL USUARIO ALMACENE EL ID CLIENTE NOSOSTROS ALMACENAMOS ROWKEY

        private int _IdCliente;
        public int IdCliente
        {
            get { return _IdCliente; }
            set
            {
                _IdCliente = value;
                RowKey = value.ToString();
            }
        }
        private string _Empresa;
        public string Empresa
        {
            get { return _Empresa; }
            set
            {
                _Empresa = value;
                PartitionKey = value;
            }
        }

        //EMPRESA: PARTITION KEY
        //CUANDO EL USUARIO ALMACENE LA EMPRESA NOSOTROS ALMACENAMOS PARTITION KEY


        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get => ; set; }
    }
}

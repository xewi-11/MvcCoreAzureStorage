using Azure.Data.Tables;
using MvcCoreAzureStorage.Models;

namespace MvcCoreAzureStorage.Services
{
    public class ServiceStorageTable
    {
        private TableClient tableClient;

        public ServiceStorageTable(TableServiceClient tableService)
        {
            this.tableClient = tableService.GetTableClient("clientes");

        }

        public async Task CreateClienteAsync(string empresa, int idCliente, string nombre, int edad, int salario)
        {
            Cliente client = new Cliente
            {
                Empresa = empresa,
                IdCliente = idCliente,
                Nombre = nombre,
                Edad = edad,
                Salario = salario
            };
            await this.tableClient.AddEntityAsync<Cliente>(client);
        }
        //LAS ENTIDADES TABLA SI DESEAMOS BUSCAR POR SU ID SOLAMENTE,NO PODEMOS,DEBEMOS BUSCAR POR SU PARTITION KEY Y SU ROW KEY, EN ESTE CASO LA EMPRESA Y EL IDCLIENTE

        public async Task<Cliente> FindClienteASync(string partitionKey, string rowKey)
        {
            Cliente Client = await this.tableClient.GetEntityAsync<Cliente>(partitionKey, rowKey);
            return Client;
        }

        public async Task DeleteCLient(string partitionKey, string rowKey)
        {
            await this.tableClient.DeleteEntityAsync(partitionKey, rowKey);

        }
        public async Task<List<Cliente>> GetClientesAsync()
        {
            List<Cliente> clientes = new List<Cliente>();
            //para la busquedase utilizan query y filter aunque no busquemos,si queremos todos le enviariamos un filter vacio

            var query = this.tableClient.QueryAsync<Cliente>(filter: "");
            //extraemos los datos de la consulta del query

            await foreach (Cliente client in query)
            {
                clientes.Add(client);
            }
            return clientes;
        }
        public async Task<List<Cliente>> GetClientesEmpresaAsync(string empresa)
        {
            //TENEMOS DOS TIPOS DE FILTER, LOS DOS CON query
            //1) SI UTILIZAMOS QUERYaSYNC DEBEMOS ESCRIBIR UNA SINTAXIS ESPECIAL DENTRO DEL filter 
            //STRING FILTRO="CAMPO EQ VALOR"; IGUAL
            //STRING FILTRO="CAMPO GT VALOR"; MAYOR
            //STRING FILTRO="CAMPO EQ VALOR AND CAMPO2 LT VALOR2"; iGUAL Y MENOR
            //  String FILTRO= $"Empresa eq '{empresa}'";

            // List<Cliente> clientes = new List<Cliente>();
            // var query= this.tableClient.QueryAsync<Cliente>(filter: FILTRO);


            //2) SI UTILIZAMOS QUERY PERMITE CONSULTAR CON lambda,  PERO SE PIERDE ASINCRONO Y NOS DEVUELVE TODO DIRECTAMENTE, NO DEBEMOS HACER UN BUCLE PARA EXTRAER LOS DATOS

            var query2 = this.tableClient.Query<Cliente>(C => C.Empresa == empresa);

            return query2.ToList();

        }
    }
}

using Microsoft.AspNetCore.Mvc;
using MvcCoreAzureStorage.Models;
using MvcCoreAzureStorage.Services;

namespace MvcCoreAzureStorage.Controllers
{
    public class AzureTableController : Controller
    {
        private ServiceStorageTable service;

        public AzureTableController(ServiceStorageTable service)
        {
            this.service = service;
        }


        public async Task<IActionResult> Index()
        {
            List<Cliente> clientes = await this.service.GetClientesAsync();
            return View(clientes);
        }
        [HttpPost]
        public async Task<IActionResult> Index(string empresa)
        {
            List<Cliente> clientes = await this.service.GetClientesEmpresaAsync(empresa);
            return View(clientes);
        }
        public async Task<IActionResult> Detalles(string partitionKey, string rowKey)
        {
            Cliente client = await this.service.FindClienteASync(partitionKey, rowKey);
            return View(client);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Cliente cliente)
        {
            await this.service.CreateClienteAsync(cliente.Empresa, cliente.IdCliente, cliente.Nombre, cliente.Edad, cliente.Salario);
            return RedirectToAction("Detalles", new { partitionKey = cliente.Empresa, rowKey = cliente.IdCliente });
        }

        public async Task<IActionResult> Delete(string partitionKey, string rowKey)
        {
            await this.service.DeleteCLient(partitionKey, rowKey);
            return RedirectToAction("Index");
        }
    }
}

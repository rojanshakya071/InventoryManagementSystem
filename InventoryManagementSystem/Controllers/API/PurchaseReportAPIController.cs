
using Inventory.Services.PurchasesReport;
using InventoryManagementSystem.Models.VM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Controllers.API
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    [Authorize]
    public class PurchaseReportAPIController : ControllerBase
    {
        private readonly IPurchasesReportServices _purchasesReportServices;
        public PurchaseReportAPIController(IPurchasesReportServices purchasesReportServices)
        {
            _purchasesReportServices = purchasesReportServices;
        }
        [HttpGet]
        public Task<List<PurchasesMasterVM>> GetAll()
        {
            return _purchasesReportServices.GetAll();
        }

        [HttpGet("{id}")]
        public Task<PurchasesMasterVM> GetById(int id)
        {
            return _purchasesReportServices.GetById(id);
        }
        [HttpDelete]
        public Task<bool> Delete(int id)
        {
            return _purchasesReportServices.Delete(id);
        }
        [HttpGet]
        public Task<IEnumerable<GetVendorsNameVM>> GetAllVendor()
        {
            return _purchasesReportServices.GetVendorsName();
        }

        [HttpGet]
        public Task<IEnumerable<GetItemsNameVM>> GetItemsName()
        {
            return _purchasesReportServices.GetItemsName();
        }
        [HttpPost]
        public Task<ActionResult> Create(PurchasesMasterVM purchases)
        {
            return _purchasesReportServices.Create(purchases);
        }
        [HttpPut]
        public Task<bool> Update(PurchasesMasterVM purchases)
        {
            return _purchasesReportServices.Update(purchases);
        }
    }
}

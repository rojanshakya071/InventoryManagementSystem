using InventoryManagementSystem.Models.VM;
using InventoryManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Controllers.API
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    [Authorize]
    public class SalesReportAPIController : ControllerBase
    {
        private readonly ISalesReportServices _salesReportServices;
        public SalesReportAPIController(ISalesReportServices salesReportServices)
        {
            _salesReportServices = salesReportServices;
        }

        [HttpGet]
        public Task<List<SalesMasterVM>> GetAll()
        {
            return _salesReportServices.GetAll();
        }

        [HttpGet("{id}")]
        public Task<SalesMasterVM> GetById(int id)
        {
            return _salesReportServices.GetById(id);
        }

        [HttpGet]
        public Task<IEnumerable<GetCustomersNameVM>> GetAllCustomers()
        {
            return _salesReportServices.GetCustomersName();
        }

        [HttpGet]
        public Task<IEnumerable<GetItemsNameVM>> GetItemsName()
        {
            return _salesReportServices.GetItemsName();
        }
        [HttpPost]
        public Task<ActionResult> Create(SalesMasterVM salesReport)
        {
            return _salesReportServices.Create(salesReport);
        }
        [HttpPut]
        public Task<ActionResult> Update(SalesMasterVM salesReport)
        {
            return _salesReportServices.Update(salesReport);
        }
        [HttpDelete]
        public Task<bool> Delete(int id)
        {
            return _salesReportServices.Delete(id);
        }
    }
}

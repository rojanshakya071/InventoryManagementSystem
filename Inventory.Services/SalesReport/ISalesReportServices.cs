using InventoryManagementSystem.Models;
using InventoryManagementSystem.Models.VM;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Services
{
    public interface ISalesReportServices 
    {
        Task<List<SalesMasterVM>> GetAll();
        Task<SalesMasterVM> GetById(int id);
        Task<ActionResult> Create(SalesMasterVM salesReport);
        Task<ActionResult> Update(SalesMasterVM salesReport);
        Task<bool> Delete(int id);
        Task<IEnumerable<GetCustomersNameVM>> GetCustomersName();
        Task<IEnumerable<GetItemsNameVM>> GetItemsName();
    }
}

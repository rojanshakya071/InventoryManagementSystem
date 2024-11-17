using InventoryManagementSystem.Models;
using InventoryManagementSystem.Models.VM;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Services.PurchasesReport
{
    public interface IPurchasesReportServices
    {
        Task<List<PurchasesMasterVM>> GetAll();
        Task<PurchasesMasterVM> GetById(int id);
        Task<ActionResult> Create(PurchasesMasterVM purchasesReport);
        Task<bool> Update(PurchasesMasterVM purchaseReport);
        Task<bool> Delete(int id);
        Task<IEnumerable<GetVendorsNameVM>> GetVendorsName();
        Task<IEnumerable<GetItemsNameVM>> GetItemsName();
    }
}

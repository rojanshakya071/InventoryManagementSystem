using Inventory.Entities;
using InventoryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Services
{
    public interface IVendorServices
    {
        Task<List<VendorModel>> GetAll();
        Task<VendorModel> GetById(int id);
        Task<ActionResult> Create (VendorModel vendor);
        Task<bool> Update (VendorModel vendor);
        Task<ActionResult> Delete (int id);
        Task<List<VendorModel>> SearchVendor(string searchTerm);
    }
}

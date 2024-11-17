using InventoryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Services
{
    public interface IItemsServices
    {
        Task<List<ItemModel>> GetAll();
        Task<ItemModel> GetById(int id);
        Task<IActionResult> Create(ItemModel item);
        Task<IActionResult> Update(ItemModel item);
        Task<bool> Delete(int id);
    }
}

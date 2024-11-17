using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Services
{  
    public class ItemsServices : IItemsServices
    {
        private readonly ApplicationDbContext _context;
        public ItemsServices(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<List<ItemModel>> GetAll()
        {
            return await _context.Items
                .Select(c => new ItemModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Unit = c.Unit,
                    Category = c.Category
                }).ToListAsync();
        }

        public async Task<ItemModel> GetById(int id)
        {
            var item = await _context.Items.FindAsync(id);

            if (item == null)
            {
                return null;
            }

            return new ItemModel
            {
                Id = item.Id,
                Name = item.Name,
                Unit = item.Unit,
                Category = item.Category
            };
        }

        public async Task<IActionResult> Create(ItemModel itemModel)
        {
            var existingData = await _context.Items.FirstOrDefaultAsync(x => x.Name.ToLower() == itemModel.Name.ToLower());
            if (existingData != null)
            {
                return new OkObjectResult(new { Success = false, Message = "Item with the same name already exists." });
            }

            var item = new ItemModel
            {
                Id = itemModel.Id,
                Name = itemModel.Name,
                Unit = itemModel.Unit,
                Category = itemModel.Category
            };

            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new { Success = true, item = item });
        }




        public async Task<bool> Delete(int id)
        {
            var item = _context.Items.Find(id);

            if (item == null)
            {
                return false;
            }

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IActionResult> Update(ItemModel itemModel)
        {
            var item = await _context.Items.FindAsync(itemModel.Id);
            var existingData = _context.Items.Any(x => x.Name.ToLower() == itemModel.Name.ToLower() && x.Id != itemModel.Id);
            if (existingData)
            {
                return new OkObjectResult(new {Success=false, Message = "Item with the same name already exists." });
            }

            item.Name = itemModel.Name;
            item.Unit = itemModel.Unit;
            item.Category = itemModel.Category;
            await _context.SaveChangesAsync();
            return new OkObjectResult(new {Success = true, Data = itemModel});
        }
    }
}

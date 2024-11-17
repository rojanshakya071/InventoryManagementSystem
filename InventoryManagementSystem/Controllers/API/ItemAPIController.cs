using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ItemAPIController : ControllerBase
    {
        private readonly IItemsServices _item;
        public ItemAPIController(IItemsServices item)
        {
            _item = item;
        }
        [HttpGet]
        public async Task<List<ItemModel>> GetAll()
        {
            return await _item.GetAll();
        }

        [HttpGet("{id}")]
        public async Task<ItemModel> GetById(int id)
        {
            return await _item.GetById(id);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ItemModel itemModel)
        {
            return await _item.Create(itemModel);
        }

        [HttpPut]
        public async Task<IActionResult> Update(ItemModel itemModel)
        {
            return await _item.Update(itemModel);
        }

        [HttpDelete]
        public async Task<bool> delete(int id)
        {
            return await _item.Delete(id);
        }
    }
}

using Inventory.Entities;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace InventoryManagementSystem.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CurrentItemsInfoAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public CurrentItemsInfoAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public List<ItemCurrentInfoVM> GetItems()
        {
            var existingItem = _context.Items.ToList();
            var existingCurrentInfo = _context.ItemsCurrentInfo.ToList();

            var requiredCurrentInfo = (from c in existingCurrentInfo
                                       join i in existingItem on c.ItemId equals i.Id
                                       select new ItemCurrentInfoVM
                                       {
                                           Id = c.Id,
                                           ItemId = c.ItemId,
                                           quantity = c.quantity,
                                           ItemName = i.Name
                                       }).ToList();

            return requiredCurrentInfo;
        }

        public class ItemCurrentInfoVM
        {
            public int Id { get; set; }
            public int ItemId { get; set; }
            public string ItemName { get; set; }
            public int quantity { get; set; }
        }
    }
}

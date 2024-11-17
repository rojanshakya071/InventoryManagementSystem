using Inventory.Entities;
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
    public class VendorAPIController : ControllerBase
    {
        private readonly IVendorServices _vendor;
        public VendorAPIController(IVendorServices vendor)
        {
            _vendor = vendor;
        }

        [HttpGet]
        public async Task<List<VendorModel>> GetAll()
        {
            return await _vendor.GetAll();
        }

        [HttpGet("{id}")]
        public async Task<VendorModel> GetById(int id)
        {
            return await _vendor.GetById(id);
        }

        [HttpPost]
        public async Task<ActionResult> Create(VendorModel vendorModel)
        {
            return await _vendor.Create(vendorModel);
        }

        [HttpPut]
        public async Task<bool> Update (VendorModel vendorModel)
        {
            return await _vendor.Update(vendorModel);
        }

        [HttpDelete]
        public async Task<ActionResult> delete (int id)
        {
            return await _vendor.Delete(id);
        }

        [HttpGet("Search")]
        public async Task<List<VendorModel>> SearchVendor(string searchTerm)
        {
            return await _vendor.SearchVendor(searchTerm);
        }
    }
}

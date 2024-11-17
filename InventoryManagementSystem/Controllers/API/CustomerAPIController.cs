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
    public class CustomerAPIController : ControllerBase
    {
        private readonly ICustomerServices _customer;
        public CustomerAPIController(ICustomerServices customer)
        {
            _customer = customer;
        }

        [HttpGet]
        public async Task<List<CustomerModel>> GetAll()
        {
            return await _customer.GetAll();
        }

        [HttpGet("{id}")]
        public async Task<CustomerModel> GetById(int id)
        {
            return await _customer.GetById(id);
        }

        [HttpPost]
        public async Task<int> Create(CustomerModel customerModel)
        {
            return await _customer.Create(customerModel);
        }

        [HttpPut]
        public async Task<bool> Update (CustomerModel customerModel)
        {
            return await _customer.Update(customerModel);
        }

        [HttpDelete]
        public async Task<bool> delete (int id)
        {
            return await _customer.Delete(id);
        }

        [HttpGet("Search")]
        public async Task<List<CustomerModel>> SearchCustomer(string searchTerm)
        {
            return await _customer.SearchCustomer(searchTerm);
        }
    }
}

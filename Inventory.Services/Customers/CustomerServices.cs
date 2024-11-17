using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Services
{
    public class CustomerServices : ICustomerServices
    {
        private readonly ApplicationDbContext _context;
        public CustomerServices(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<CustomerModel>> GetAll()
        {
            return await _context.Customers
                .Select(c => new CustomerModel
            {
                Id = c.Id,
                FullName = c.FullName,
                Address = c.Address,
                ContactNo = c.ContactNo
            }).ToListAsync();
        
        }

        public async Task<CustomerModel> GetById(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return null;
            }

            return new CustomerModel
            {
                Id = customer.Id,
                FullName = customer.FullName,
                Address = customer.Address,
                ContactNo = customer.ContactNo
            };

        }

        public async Task<int> Create(CustomerModel customerModel)
        {
           
            var Customer = new CustomerModel
            {
                FullName = customerModel.FullName,
                Address = customerModel.Address,
                ContactNo = customerModel.ContactNo
            };

            _context.Customers.Add(Customer);
            await _context.SaveChangesAsync();
            return Customer.Id;
        }

        public async Task<bool> Update(CustomerModel customerModel)
        {
            var customer = await _context.Customers.FindAsync(customerModel.Id);

            if(customer == null)
            {
                return false;
            }

            customer.FullName = customerModel.FullName;
            customer.Address = customerModel.Address;
            customer.ContactNo = customerModel.ContactNo;   

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(int id)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
            if (customer == null)
            {
                return false;
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<CustomerModel>> SearchCustomer(string searchTerm)
        {
            return await _context.Customers.Where( x=> x.FullName.ToLower().Contains(searchTerm.ToLower())).ToListAsync();
        }

    }
}

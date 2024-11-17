using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Services
{
    public interface ICustomerServices
    {
        Task<List<CustomerModel>> GetAll();
        Task<CustomerModel> GetById(int id);
        Task<int> Create (CustomerModel customer);
        Task<bool> Update (CustomerModel customer);
        Task<bool> Delete (int id);
        Task<List<CustomerModel>> SearchCustomer(string searchTerm);
    }
}

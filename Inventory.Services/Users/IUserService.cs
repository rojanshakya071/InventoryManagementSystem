using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Services
{
    public interface IUserService
    {
        bool RegisterUser(UserModel model);
        UserModel GetUserWithRole(string email, string password);
    }
}

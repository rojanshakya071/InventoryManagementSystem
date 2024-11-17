using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Services
{
    public class UserServices : IUserService
    {
        protected readonly ApplicationDbContext _context;
        public UserServices(ApplicationDbContext context)
        {
            _context = context;
        }
        public  bool RegisterUser(UserModel model)
        {
            bool alreadyExit = _context.User.Count(x => x.Email == model.Email) > 0;
            if(!alreadyExit) {
                var user = new UserModel
                {
                    Username = model.Username,
                    Email = model.Email,
                    Password = model.Password,
                    Roles = "Employee",
                    ConfirmPassword = model.ConfirmPassword,
                };
                _context.User.Add(user);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public UserModel GetUserWithRole(string email, string password)
        {
            return _context.User.FirstOrDefault(x => x.Email == email && x.Password == password);
        }
    }
}

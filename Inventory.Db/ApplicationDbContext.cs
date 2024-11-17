using Inventory.Entities;
using InventoryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InventoryManagementSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<ItemModel> Items { get; set; }
        public DbSet<CustomerModel> Customers { get; set; }
        public DbSet<SalesMasterModel> SalesMaster { get; set; }
        public DbSet<SalesDetailsModel> SalesDetails { get; set; }
        public DbSet<UserModel> User { get; set; }
        public DbSet<VendorModel> Vendors { get; set; }
        public DbSet<ItemCurrentInfo> ItemsCurrentInfo { get; set; }

        public DbSet<ItemCurrentInfoHistory> ItemsHistoryInfo { get; set; }

        public DbSet<PurchaseMasterModel> PurchasesMaster { get; set; }

        public DbSet<PurchaseDetailModel> PurchasesDetail { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserModel>().HasData(
                new UserModel { Id = 1, Username = "Admin", Password = "Admin@123", Roles = "Admin", Email = "Admin123@gmail.com" });
        }
        
    }

}

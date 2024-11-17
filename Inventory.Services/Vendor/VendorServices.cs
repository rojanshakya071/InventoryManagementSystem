using Inventory.Entities;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Services
{
    public class VendorServices : IVendorServices
    {
        private readonly ApplicationDbContext _context;
        public VendorServices(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<VendorModel>> GetAll()
        {
            return await _context.Vendors
                .Select(c => new VendorModel
            {
                Id = c.Id,
                Name = c.Name,
                Contact = c.Contact,
                Address = c.Address
            }).ToListAsync();
        
        }

        public async Task<VendorModel> GetById(int id)
        {
            var vendor = await _context.Vendors.FindAsync(id);

            if (vendor == null)
            {
                return null;
            }

            return new VendorModel
            {
                Id = vendor.Id,
                Name = vendor.Name,
                Contact = vendor.Contact,
                Address = vendor.Address
            };

        }

        public async Task<ActionResult> Create(VendorModel VendorModel)
        {
            try
            {
                var vendor = new VendorModel
                {
                    Name = VendorModel.Name,
                    Address = VendorModel.Address,
                    Contact = VendorModel.Contact
                };

                _context.Vendors.Add(vendor);
                await _context.SaveChangesAsync();
                return new OkObjectResult(new { Success = true, Message = "Vendor added successfully" });
            }
            catch (Exception ex)
            {
                return new OkObjectResult(new { Success = false, Message = "while adding vendor error occured" });
            }
           
        }

        public async Task<bool> Update(VendorModel VendorModel)
        {
            var vendor = await _context.Vendors.FindAsync(VendorModel.Id);

            if(vendor == null)
            {
                return false;
            }

            vendor.Name = VendorModel.Name;
            vendor.Address = VendorModel.Address;
            vendor.Contact = VendorModel.Contact;   

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ActionResult> Delete(int id)
        {
            var purchase = await _context.PurchasesMaster.Where(c => c.VendorId == id).ToListAsync();
            if (purchase.Any())
            {
                return new OkObjectResult(new { Success = false, Message = "Vendor is used in other table also" });
            }
            var vendor = await _context.Vendors.FirstOrDefaultAsync(c => c.Id == id);
            if (vendor == null)
            {
                return new OkObjectResult(new { Success = false, Message = "The Vendor table is empty" });

            }

            _context.Vendors.Remove(vendor);
            await _context.SaveChangesAsync();
            return new OkObjectResult(new { Success = true, Message = "Vendor delete successfully " });

        }
        public async Task<List<VendorModel>> SearchVendor(string searchTerm)
        {
            return await _context.Vendors.Where(x => x.Name.ToLower().Contains(searchTerm.ToLower())).ToListAsync();
        }
    }
}

using Inventory.Entities;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Models.VM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace Inventory.Services.PurchasesReport
{
    public class PurchasesReportServices : IPurchasesReportServices
    {
        private readonly ApplicationDbContext _context;
        public PurchasesReportServices(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<PurchasesMasterVM>> GetAll()
        {
            List<PurchasesMasterVM> result = new List<PurchasesMasterVM>();
            var masterData = _context.PurchasesMaster.ToList();

            if (masterData.Count > 0)
            {
                result = (from m in masterData
                          let details = _context.PurchasesDetail.Where(x => x.PurchaseMasterId == m.Id)
                          let vendor = _context.Vendors.FirstOrDefault(c => c.Id == m.VendorId)
                          select new PurchasesMasterVM
                          {
                              Id = m.Id,
                              VendorId = m.VendorId,
                              VendorName = vendor.Name,
                              InvoiceNumber = m.InvoiceNumber,
                              BillAmount = m.BillAmount,
                              Discount = m.Discount,
                              NetAmount = m.NetAmount,
                              Purchases = (from d in details
                                           select new PurchasesDetailsVM
                                           {
                                               Id = d.Id,
                                               ItemId = d.ItemId,
                                               Unit = d.Unit,
                                               Quantity = d.Quantity,
                                               Amount = d.Amount
                                           }).ToList()
                          }).ToList();

            }
            return result;

        }

        public async Task<PurchasesMasterVM> GetById(int id)
        {
            var masterData = _context.PurchasesMaster.Find(id);
            if (masterData == null)
            {
                return new PurchasesMasterVM();
            }
            else
            {
                var details = _context.PurchasesDetail.Where(x => x.PurchaseMasterId == masterData.Id).ToList();
                var data = new PurchasesMasterVM
                {
                    Id = masterData.Id,
                    VendorId = masterData.VendorId,
                    InvoiceNumber = masterData.InvoiceNumber,
                    BillAmount = masterData.BillAmount,
                    Discount = masterData.Discount,
                    NetAmount = masterData.NetAmount,
                };
                data.Purchases = (from d in details
                                  select new PurchasesDetailsVM
                                  {
                                      Id = d.Id,
                                      ItemId = d.ItemId,
                                      Unit = d.Unit,
                                      Quantity = d.Quantity,
                                  }).ToList();
                return data;
            }

        }

        public async Task<IEnumerable<GetVendorsNameVM>> GetVendorsName()
        {
            var vendorData = _context.Vendors.Select(vendor => new GetVendorsNameVM
            {
                Id = vendor.Id,
                VendorName = vendor.Name,
            }).ToList();
            return vendorData;
        }

        public async Task<IEnumerable<GetItemsNameVM>> GetItemsName()
        {
            var ItemsData = _context.Items.Select(item => new GetItemsNameVM
            {
                Id = item.Id,
                ItemName = item.Name,
                Unit = item.Unit,
            }).ToList();
            return ItemsData;
        }

        public async Task<ActionResult> Create(PurchasesMasterVM purchaseReport)
        {

            using (_context.Database.BeginTransaction())
            {
                if (purchaseReport.Purchases.Count == 0)
                {
                    return new OkObjectResult(new { Success = false, Message = "There must be one item in purchase." });
                }
                
                else
                {
                    var masterData = new PurchaseMasterModel()
                    {
                        Id = purchaseReport.Id,
                        VendorId = purchaseReport.VendorId,
                        InvoiceNumber = purchaseReport.InvoiceNumber,
                        BillAmount = purchaseReport.BillAmount,
                        Discount = purchaseReport.Discount,
                        NetAmount = purchaseReport.NetAmount,
                    };

                    var masterAdd = _context.PurchasesMaster.Add(masterData);
                    _context.SaveChanges();
                    _context.Database.CommitTransaction();
                    if (masterData != null)
                    {
                        var details = from d in purchaseReport.Purchases
                                      select new PurchaseDetailModel
                                      {
                                          Id = 0,
                                          ItemId = d.ItemId,
                                          Amount = d.Amount,
                                          Quantity = d.Quantity,
                                          PurchaseMasterId = masterAdd.Entity.Id,
                                          Unit = d.Unit,
                                      };

                        _context.PurchasesDetail.AddRange(details);
                        _context.SaveChanges();
                        foreach (var item in details)
                        {
                            var existingData = _context.ItemsCurrentInfo.FirstOrDefault(x => x.ItemId == item.ItemId);
                            if (existingData != null)
                            {

                                existingData.quantity = existingData.quantity + item.Quantity;

                                _context.ItemsCurrentInfo.Update(existingData);
                                _context.SaveChanges();
                            }
                            else
                            {
                                var itemCurrentInfo = new ItemCurrentInfo
                                {
                                    Id = 0,
                                    ItemId = item.ItemId,
                                    quantity = item.Quantity
                                };
                                _context.ItemsCurrentInfo.Add(itemCurrentInfo);
                                _context.SaveChanges();
                            }

                            var itemCurrentInfoHistory = new ItemCurrentInfoHistory()
                            {
                                Id = 0,
                                ItemId = item.ItemId,
                                Quantity = item.Quantity,
                                TransDate = DateTime.Now,
                                StockCheckOut = StockCheckOut.In,
                                TransactionType = TransactionType.Purchase
                            };
                            _context.ItemsHistoryInfo.Add(itemCurrentInfoHistory);
                            _context.SaveChanges();

                        }
                    }

                    // _context.Database.RollbackTransaction();
                    return new OkObjectResult(new { Success = true, Message = "Purchase Successfully" });
                }

            }

        }

        public async Task<bool> Update(PurchasesMasterVM purchaseReport)
        {
            var masterData = _context.PurchasesMaster.Find(purchaseReport.Id);
            if (masterData == null)
            {
                return false;
            }
            masterData.Id = purchaseReport.Id;
            masterData.VendorId = purchaseReport.VendorId;
            masterData.InvoiceNumber = purchaseReport.InvoiceNumber;
            masterData.Discount = purchaseReport.Discount;
            masterData.BillAmount = purchaseReport.BillAmount;
            masterData.NetAmount = purchaseReport.NetAmount;
            var masterAdd = _context.PurchasesMaster.Update(masterData);
            _context.SaveChanges();

            var existingDetailsData = _context.PurchasesDetail.Where(x => x.PurchaseMasterId == purchaseReport.Id).ToList();

            foreach (var item in existingDetailsData)
            {
                var itemCurrentInfo = _context.ItemsCurrentInfo.FirstOrDefault(x => x.ItemId == item.ItemId);
                if (itemCurrentInfo != null)
                {
                    itemCurrentInfo.quantity -= item.Quantity;
                    _context.ItemsCurrentInfo.Update(itemCurrentInfo);
                    _context.SaveChanges();
                }
                var itemHistoryInfo = new ItemCurrentInfoHistory
                {
                    Id = 0,
                    ItemId = item.ItemId,
                    Quantity = item.Quantity,
                    TransDate = DateTime.Now,
                    StockCheckOut = StockCheckOut.Out,
                    TransactionType = TransactionType.Sales
                };
                _context.ItemsHistoryInfo.Add(itemHistoryInfo);
                _context.SaveChanges();
            }
            _context.PurchasesDetail.RemoveRange(existingDetailsData);

            var detailsData = from c in purchaseReport.Purchases
                              select new PurchaseDetailModel
                              {
                                  Id = 0,
                                  Quantity = c.Quantity,
                                  Unit = c.Unit,
                                  Amount = c.Amount,
                                  ItemId = c.ItemId,
                                  PurchaseMasterId = masterAdd.Entity.Id
                              };

            _context.PurchasesDetail.AddRange(detailsData);
            _context.SaveChanges();
            foreach (var item in detailsData)
            {
                var itemCurrentinfo = _context.ItemsCurrentInfo.FirstOrDefault(x => x.ItemId == item.ItemId);
                if (itemCurrentinfo != null)
                {
                    itemCurrentinfo.quantity += item.Quantity;
                    _context.ItemsCurrentInfo.Update(itemCurrentinfo);
                    _context.SaveChanges();
                }
                else
                {
                    var currentInfo = new ItemCurrentInfo
                    {
                        Id = 0,
                        ItemId = item.ItemId,
                        quantity = item.Quantity,

                    };
                    _context.ItemsCurrentInfo.Add(currentInfo);
                    _context.SaveChanges();
                }

                var historyInfo = new ItemCurrentInfoHistory
                {
                    Id = 0,
                    ItemId = item.ItemId,
                    Quantity = item.Quantity,
                    TransDate = DateTime.Now,
                    TransactionType = TransactionType.Purchase,
                    StockCheckOut = StockCheckOut.In
                };
                _context.ItemsHistoryInfo.Add(historyInfo);
                _context.SaveChanges();
            }
            return true;
        }

        public async Task<bool> Delete(int id)
        {
            var masterData = _context.PurchasesMaster.Find(id);
            if (masterData == null)
            {
                return false;
            }
            else
            {
                var details = _context.PurchasesDetail.Where(x => x.PurchaseMasterId == masterData.Id).ToList();
                if (details.Count > 0)
                {
                    foreach (var item in details)
                    {
                        var currentInfo = _context.ItemsCurrentInfo.FirstOrDefault(x => x.ItemId == item.ItemId);


                        currentInfo.quantity -= item.Quantity;
                        _context.ItemsCurrentInfo.Update(currentInfo);
                        _context.SaveChanges();

                        var itemCurrentInfoHistory = new ItemCurrentInfoHistory
                        {
                            Id = 0,
                            ItemId = item.ItemId,
                            Quantity = item.Quantity,
                            TransactionType = TransactionType.Sales,
                            StockCheckOut = StockCheckOut.Out,
                            TransDate = DateTime.Now,
                        };
                        _context.ItemsHistoryInfo.Add(itemCurrentInfoHistory);
                        _context.SaveChanges();
                    }
                    _context.PurchasesDetail.RemoveRange(details);
                    _context.SaveChanges();
                }
                _context.PurchasesMaster.Remove(masterData);
                _context.SaveChanges();
            }
            return true;
        }


    }
}


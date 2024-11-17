using Inventory.Entities;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Models.VM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Services
{
    public class SalesReportServices : ISalesReportServices
    {
        private readonly ApplicationDbContext _context;


        public SalesReportServices(ApplicationDbContext context)
        {
            _context = context;
        }
        private string ItemName(int id)
        {
            return _context.Items.FirstOrDefault(x => x.Id == id).Name;
        }
        private bool IsItemAvaliable(int itemId, int quantity)
        {
            var itemCurrentInfo = _context.ItemsCurrentInfo.FirstOrDefault(x => x.ItemId == itemId);
            return itemCurrentInfo != null && itemCurrentInfo.quantity >= quantity;
        }
        public async Task<List<SalesMasterVM>> GetAll()
        {
            List<SalesMasterVM> result = new List<SalesMasterVM>();
            var masterData = await _context.SalesMaster.ToListAsync();
            if (masterData.Count > 0)
            {
                result = (from m in masterData
                          let customer = _context.Customers.FirstOrDefault(c => c.Id == m.CustomerId)
                          let details = _context.SalesDetails.Where(x => x.SalesMasterId == m.Id)
                          select new SalesMasterVM
                          {
                              Id = m.Id,
                              SalesDate = m.SalesDate,
                              CustomerId = m.CustomerId,
                              CustomerName = customer.FullName,
                              InvoiceNumber = m.InvoiceNumber,
                              BillAmount = m.BillAmount,
                              Discount = m.Discount,
                              NetAmount = m.NetAmount,
                              Sales = (from d in details
                                       select new SalesDetailsVM
                                       {
                                           Id = d.Id,
                                           ItemId = d.ItemId,
                                           Unit = d.Unit,
                                           Quantity = d.Quantity,
                                           Amount = d.Amount,
                                           Price = d.Price
                                       }
                              ).ToList()
                          }).ToList();
            }
            return result;
        }

        public async Task<SalesMasterVM> GetById(int id)
        {
            var masterData = _context.SalesMaster.Find(id);

            if (masterData == null)
            {
                return new SalesMasterVM();
            }
            else
            {
                var details = _context.SalesDetails.Where(x => x.SalesMasterId == masterData.Id).ToList();
                var data = new SalesMasterVM()
                {
                    Id = masterData.Id,
                    SalesDate = masterData.SalesDate,
                    CustomerId = masterData.CustomerId,
                    InvoiceNumber = masterData.InvoiceNumber,
                    BillAmount = masterData.BillAmount,
                    Discount = masterData.Discount,
                    NetAmount = masterData.NetAmount,
                };
                data.Sales = (from d in details
                              select new SalesDetailsVM()
                              {
                                  Id = d.Id,
                                  ItemId = d.ItemId,
                                  Unit = d.Unit,
                                  Quantity = d.Quantity,
                                  Amount = d.Amount,
                                  Price = d.Price
                              }).ToList();
                return data;
            }

        }

        public async Task<IEnumerable<GetCustomersNameVM>> GetCustomersName()
        {
            var customerData = _context.Customers.Select(customer => new GetCustomersNameVM
            {
                Id = customer.Id,
                CustomerName = customer.FullName
            }).ToList();

            return customerData;

        }

        public async Task<IEnumerable<GetItemsNameVM>> GetItemsName()
        {
            var result = from item in _context.Items
                         join info in _context.ItemsCurrentInfo
                         on item.Id equals info.ItemId
                         select new GetItemsNameVM
                         {
                             Id = item.Id,
                             ItemName = item.Name,
                             Unit = item.Unit,
                             Quantity = info.quantity
                         };
            return await result.ToListAsync();
        }

        public async Task<ActionResult> Create(SalesMasterVM salesReport)
        {
            if (salesReport.CustomerId == 0)
            {
                return new OkObjectResult(new { Success = false, Message = "sale form is not completly filled. Please verify Again!" });
            }
            else
            {
                foreach (var item in salesReport.Sales)
                {
                    if (!IsItemAvaliable(item.ItemId, item.Quantity))
                    {
                        var name = ItemName(item.ItemId);
                        return new OkObjectResult(new { Success = false, Message = $"Item {name} quentity is less that you wanna buy." });
                    }
                }


                var masterData = new SalesMasterModel()
                {
                    Id = 0,
                    SalesDate = salesReport.SalesDate,
                    CustomerId = salesReport.CustomerId,
                    InvoiceNumber = salesReport.InvoiceNumber,
                    BillAmount = salesReport.BillAmount,
                    Discount = salesReport.Discount,
                    NetAmount = salesReport.NetAmount,
                };
                var masterAdd = _context.SalesMaster.Add(masterData);
                _context.SaveChanges();
                if (masterData != null)
                {
                    var details = from d in salesReport.Sales
                                  select new SalesDetailsModel
                                  {
                                      Id = 0,
                                      ItemId = d.ItemId,
                                      Unit = d.Unit,
                                      Quantity = d.Quantity,
                                      Amount = d.Amount,
                                      Price = d.Price,
                                      SalesMasterId = masterData.Id,
                                  };

                    foreach (var item in details)
                    {
                        var exitingData = _context.ItemsCurrentInfo.FirstOrDefault(x => x.ItemId == item.ItemId);
                        if (exitingData != null && exitingData.quantity > 0)
                        {
                            exitingData.quantity -= item.Quantity;
                            _context.ItemsCurrentInfo.Update(exitingData);
                            _context.SaveChanges();
                        }
                        else
                        {
                            throw new Exception("quantity is zero");
                        }

                        var itemCurrentInfo = new ItemCurrentInfoHistory()
                        {
                            Id = 0,
                            ItemId = item.ItemId,
                            Quantity = item.Quantity,
                            TransDate = DateTime.Now,
                            StockCheckOut = StockCheckOut.Out,
                            TransactionType = TransactionType.Sales
                        };
                        _context.ItemsHistoryInfo.Add(itemCurrentInfo);
                        _context.SaveChanges();

                    }
                    _context.SalesDetails.AddRange(details);
                    _context.SaveChanges();
                }
                return new OkObjectResult(new { Success = true, Message = "Sales add successfully" });

            }
        }

        public async Task<ActionResult> Update(SalesMasterVM salesReport)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    
                    var masterData = await _context.SalesMaster.FindAsync(salesReport.Id);
                    if (masterData == null)
                    {
                        return new OkObjectResult(new { Success = false, Message = $"sales data is null" });
                    }

                    masterData.SalesDate = salesReport.SalesDate;
                    masterData.CustomerId = salesReport.CustomerId;
                    masterData.InvoiceNumber = salesReport.InvoiceNumber;
                    masterData.Discount = salesReport.Discount;
                    masterData.BillAmount = salesReport.BillAmount;
                    masterData.NetAmount = salesReport.NetAmount;

                    var existingDetail = _context.SalesDetails.Where(x => x.SalesMasterId == masterData.Id).ToList();
                    if (existingDetail != null)
                    {
                        foreach (var item in existingDetail)
                        {
                            var itemCurrent = await _context.ItemsCurrentInfo.FirstOrDefaultAsync(x => x.ItemId == item.ItemId);
                            if (itemCurrent != null)
                            {
                                itemCurrent.quantity += item.Quantity;
                                _context.ItemsCurrentInfo.Update(itemCurrent);
                            }
                            else
                            {
                                return new OkObjectResult(new { Success = false, Message = "There stock of such item" });
                            }

                            var currentHistory = new ItemCurrentInfoHistory()
                            {
                                ItemId = item.ItemId,
                                Quantity = item.Quantity,
                                TransDate = DateTime.Now,
                                StockCheckOut = StockCheckOut.In,
                                TransactionType = TransactionType.Purchase
                            };
                            await _context.ItemsHistoryInfo.AddAsync(currentHistory);
                        }

                        _context.SalesDetails.RemoveRange(existingDetail);
                    }
                   

                    var details = salesReport.Sales.Select(d => new SalesDetailsModel
                    {
                        ItemId = d.ItemId,
                        Unit = d.Unit,
                        Quantity = d.Quantity,
                        Amount = d.Amount,
                        Price = d.Price,
                        SalesMasterId = masterData.Id,
                    }).ToList();

                    await _context.SalesDetails.AddRangeAsync(details);

                    foreach (var item in salesReport.Sales)
                    {
                        if (!IsItemAvaliable(item.ItemId, item.Quantity))
                        {
                            var name = ItemName(item.ItemId);
                            return new OkObjectResult(new { Success = false, Message = $"Item {name} quentity is less that you wanna buy." });
                        }
                    }

                    foreach (var item in details)
                    {
                        var currentInfo = await _context.ItemsCurrentInfo.FirstOrDefaultAsync(x => x.ItemId == item.ItemId);
                        if (currentInfo != null)
                        {
                            currentInfo.quantity -= item.Quantity;
                            _context.ItemsCurrentInfo.Update(currentInfo);

                            var historyinfo = new ItemCurrentInfoHistory()
                            {
                                ItemId = item.ItemId,
                                Quantity = item.Quantity,
                                TransDate = DateTime.Now,
                                TransactionType = TransactionType.Sales,
                                StockCheckOut = StockCheckOut.Out
                            };
                            await _context.ItemsHistoryInfo.AddAsync(historyinfo);
                        }
                        else
                        {
                            return new OkObjectResult(new { Success = false, Message = "There must be one item" });
                        }
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                     return new OkObjectResult(new { Success = false, Message = "sales Add Successfully" });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    // Log the exception (ex) here if needed
                     return new OkObjectResult(new { Success = false, Message = "There is incomplete data " });
                }
            }
        }


        public async Task<bool> Delete(int id)
        {
            var masterDate = _context.SalesMaster.Find(id);
            if (masterDate == null)
            {
                return false;
            }
            else
            {
                var details = _context.SalesDetails.Where(x => x.SalesMasterId == masterDate.Id).ToList();


                foreach (var item in details)
                {
                    var itemCurrentInfo = _context.ItemsCurrentInfo.FirstOrDefault(x => x.ItemId == item.ItemId);
                    if (itemCurrentInfo.quantity > 0)
                    {
                        itemCurrentInfo.quantity += item.Quantity;
                        _context.ItemsCurrentInfo.Update(itemCurrentInfo);
                        _context.SaveChanges();
                    }
                    else
                    {
                        throw new Exception("item quntity is zero");
                    }

                    var currentHistory = new ItemCurrentInfoHistory()
                    {
                        Id = 0,
                        ItemId = item.ItemId,
                        Quantity = item.Quantity,
                        TransDate = DateTime.Now,
                        StockCheckOut = StockCheckOut.In,
                        TransactionType = TransactionType.Purchase,
                    };
                    _context.ItemsHistoryInfo.Add(currentHistory);
                    _context.SaveChanges();
                }
                if (details.Count > 0)
                {
                    _context.SalesDetails.RemoveRange(details);
                    _context.SaveChanges();
                }
                _context.SalesMaster.Remove(masterDate);
                _context.SaveChanges();

            }
            return true;
        }


    }
}

using ClosedXML.Excel;
using Inventory.Entities;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ItemsInfoHistoryAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ItemsInfoHistoryAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<List<ItemsHistoryVM>> GetItemsHistory(string search = "", TransactionType? transactionType = null)
        {
            var items = await _context.Items.ToListAsync();
            var itemsHistory = await _context.ItemsHistoryInfo.ToListAsync();

            var result = (from i in items
                          join h in itemsHistory on i.Id equals h.ItemId
                          where (string.IsNullOrEmpty(search) || i.Name.Contains(search)) &&
                                (!transactionType.HasValue || h.TransactionType == transactionType)
                          select new ItemsHistoryVM
                          {
                              Id = h.Id,
                              ItemId = h.ItemId,
                              ItemName = i.Name,
                              Quantity = h.Quantity,
                              StockCheckOut = h.StockCheckOut,
                              TransactionType = h.TransactionType,
                              TransDate = h.TransDate
                          }).ToList();

            return result;
        }

        [HttpGet("GenerateReport")]
        public IActionResult GenerateReport(string search = "", TransactionType? transactionType = null)
        {
            var items = _context.Items.ToList();
            var itemsHistory = _context.ItemsHistoryInfo.ToList();

            var result = (from i in items
                          join h in itemsHistory on i.Id equals h.ItemId
                          where (string.IsNullOrEmpty(search) || i.Name.Contains(search)) &&
                                (!transactionType.HasValue || h.TransactionType == transactionType)
                          select new ItemsHistoryVM
                          {
                              Id = h.Id,
                              ItemId = h.ItemId,
                              ItemName = i.Name,
                              Quantity = h.Quantity,
                              StockCheckOut = h.StockCheckOut,
                              TransactionType = h.TransactionType,
                              TransDate = h.TransDate
                          }).ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Items History");

                // Add header row
                worksheet.Cell(1, 1).Value = "Item Name";
                worksheet.Cell(1, 2).Value = "Quantity";
                worksheet.Cell(1, 3).Value = "Transaction Type";
                worksheet.Cell(1, 4).Value = "Stock Check Out";
                worksheet.Cell(1, 5).Value = "Date";

                // Add data rows
                for (int i = 0; i < result.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = result[i].ItemName;
                    worksheet.Cell(i + 2, 2).Value = result[i].Quantity;
                    worksheet.Cell(i + 2, 3).Value = result[i].TransactionTypeText;
                    worksheet.Cell(i + 2, 4).Value = result[i].StockCheckOutText;
                    worksheet.Cell(i + 2, 5).Value = result[i].TransDateFormatted;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ItemsHistoryReport.xlsx");
                }
            }
        }
        public class ItemsHistoryVM
        {
            public int Id { get; set; }
            public int ItemId { get; set; }
            public string ItemName { get; set; }
            public int Quantity { get; set; }
            public DateTime TransDate { get; set; }
            public StockCheckOut StockCheckOut { get; set; }
            public TransactionType TransactionType { get; set; }

            [NotMapped]
            public string TransDateFormatted => TransDate.ToString("yyyy-MM-dd");

            [NotMapped]
            public string StockCheckOutText => StockCheckOut.ToString();

            [NotMapped]
            public string TransactionTypeText => TransactionType.ToString();
        }
    }
}

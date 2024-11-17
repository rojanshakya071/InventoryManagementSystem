using InventoryManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Inventory.Entities
{
    public class ItemCurrentInfoHistory
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ItemId { get; set; }
        [ForeignKey("ItemId")]
        [JsonIgnore]
        public ItemModel Item { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be non-negative value")]
        public int Quantity { get; set; }
        [Required]
        public DateTime TransDate { get; set; }
        [Required]
        public StockCheckOut StockCheckOut { get; set; }
        [Required]
        public TransactionType TransactionType { get; set; }
    }

    public enum StockCheckOut
    {
        In,
        Out
    }

    public enum TransactionType
    {
        Purchase,
        Sales
    }
}


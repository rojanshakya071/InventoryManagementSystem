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
    public class PurchaseDetailModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ItemId {  get; set; }
        [ForeignKey("ItemId")]
        [JsonIgnore]
        public virtual ItemModel Item { get; set; }
        [Required]
        public string Unit { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage ="Quantity cannot be negative value")]
        public int Quantity { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage ="Price must be positive value")]
        public decimal Amount { get; set; }
        [Required]
        public int PurchaseMasterId { get; set; }
        [ForeignKey("PurchaseMasterId")]
        [JsonIgnore]
        public virtual PurchaseMasterModel PurchaseMaster { get; set; }

    }
}

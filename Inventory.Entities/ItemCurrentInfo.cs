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
    public class ItemCurrentInfo
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ItemId { get; set; }
        [ForeignKey("ItemId")]
        [JsonIgnore]
        public virtual ItemModel Item { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage ="Quantity must not be negative")]
        public int quantity { get; set; }
    }
}

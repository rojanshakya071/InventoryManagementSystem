using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace InventoryManagementSystem.Models
{
    public class SalesDetailsModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ItemId { get; set; }

        [ForeignKey("ItemId")]
        [JsonIgnore]
        public ItemModel Item { get; set; }

        [Required]
        [Display(Name = "Unit")]
        public string Unit { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be a negative value.")]
        public int Quantity { get; set; }

        [Required]
        [Range(0.00, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        public decimal Price { get; set; }

        [Required]
        [Range(0.00, double.MaxValue, ErrorMessage = "Amount must be a positive value.")]
        public decimal Amount { get; set; }

        [Required]
        public int SalesMasterId { get; set; }

        [ForeignKey("SalesMasterId")]
        [JsonIgnore]
        public SalesMasterModel SalesMaster { get; set; }
    }
}

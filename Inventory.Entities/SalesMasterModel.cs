

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace InventoryManagementSystem.Models
{
    public class SalesMasterModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime SalesDate { get; set; } = DateTime.Now;

        [Required]
        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        [JsonIgnore]
        public virtual CustomerModel Customer { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Invoice Number must be a positive integer.")]
        public int InvoiceNumber { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Bill Amount must be a positive value.")]
        public decimal BillAmount { get; set; }

        [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100.")]
        public decimal Discount { get; set; } = 0;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Net Amount must be a positive value.")]
        public decimal NetAmount { get; set; }
    }
}
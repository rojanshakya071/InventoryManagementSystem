using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Inventory.Entities
{
    public class PurchaseMasterModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int VendorId { get; set; }
        [ForeignKey("VendorId")]
        [JsonIgnore]
        public virtual VendorModel Vendor { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Invoice Number must be positive integer")]
        public int InvoiceNumber { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Bill Amount must be positve value.")]
        public decimal BillAmount { get; set; }
        
        [Range(0.00, double.MaxValue, ErrorMessage = "Bill Amount must be a positive value.")]
        public decimal Discount  { get; set;}
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Net Amount must be a positive value.")]
        public decimal NetAmount { get; set; }
    }

}

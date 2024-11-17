using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Models
{
    public class ItemModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(25, ErrorMessage ="Name can't be greater than 25 character")]
        [Display(Name = "Item Name")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Unit")]
        public string Unit { get; set; }
        [Required]
        [Display(Name = "Category")]
        public string Category { get; set; }
    }
}

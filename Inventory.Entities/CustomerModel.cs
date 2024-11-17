using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Models
{
    public class CustomerModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(25, ErrorMessage = "Full Name can't be longer than 25 character.")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }
        [Required]
        [StringLength(10, ErrorMessage = "Contace number cannot be longer than 10 digit")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "The contact number must have 10 digit")]
        public string ContactNo { get; set; }
        [Required]
        [StringLength(100, ErrorMessage ="Address cannot be longer than 100 characters")]
        public string Address { get; set; }
    }
}

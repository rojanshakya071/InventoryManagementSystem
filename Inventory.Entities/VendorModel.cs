using System.ComponentModel.DataAnnotations;

namespace Inventory.Entities
{
    public class VendorModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
       [StringLength(50, ErrorMessage = "Name can't be longer than 50 characters.")]
        [Display(Name = "Vendor Name")]
        public string Name { get; set; }

        [Required]
        [StringLength(10, ErrorMessage = "Contact can't be longer than 10 characters.")]
        //[RegularExpression(@"^\d{10}$", ErrorMessage = "The contact number must have 10 digit")]
        public string Contact { get; set; }

        [Required]
       [StringLength(100, ErrorMessage = "Address can't be longer than 100 characters.")]
        public string Address { get; set; }
    }
}

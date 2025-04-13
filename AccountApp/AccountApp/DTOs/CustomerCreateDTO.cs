using System.ComponentModel.DataAnnotations;

namespace AccountApp.DTOs
{
    public class CustomerCreateDTO
    {
        [Required]
        [StringLength(50)]
        [RegularExpression(@"/^[a-z ,.'-]+$/i")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [RegularExpression(@"/^[a-z ,.'-]+$/i")]
        public string LastName { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
    }
}

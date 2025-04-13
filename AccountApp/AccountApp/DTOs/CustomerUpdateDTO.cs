using System.ComponentModel.DataAnnotations;

namespace AccountApp.DTOs
{
    public class CustomerUpdateDTO
    {
        [Required]
        public int CustomerId { get; set; }

        [StringLength(50)]
        [RegularExpression(@"/^[a-z ,.'-]+$/i")]
        public string? FirstName {  get; set; }

        [StringLength(50)]
        [RegularExpression(@"/^[a-z ,.'-]+$/i")]
        public string? LastName { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }
    }
}

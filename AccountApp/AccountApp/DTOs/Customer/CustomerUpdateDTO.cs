using System.ComponentModel.DataAnnotations;

namespace AccountApp.DTOs.Customer
{
    public class CustomerUpdateDTO
    {
        [Required]
        public int CustomerId { get; set; }

        [StringLength(50)]
        [RegularExpression(@"^[a-zA-Z0-9][a-zA-Z0-9.,'\-_ ]*[a-zA-Z0-9]$")]
        public string? FirstName {  get; set; }

        [StringLength(50)]
        [RegularExpression(@"^[a-zA-Z0-9][a-zA-Z0-9.,'\-_ ]*[a-zA-Z0-9]$")]
        public string? LastName { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }
    }
}

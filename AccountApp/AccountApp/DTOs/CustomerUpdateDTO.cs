using System.ComponentModel.DataAnnotations;

namespace AccountApp.DTOs
{
    public class CustomerUpdateDTO
    {
        [Required]
        public int CustomerId { get; set; }
        public string? FirstName {  get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
    }
}

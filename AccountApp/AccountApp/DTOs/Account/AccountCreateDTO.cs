using AccountApp.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace AccountApp.DTOs.Account
{
    public class AccountCreateDTO
    {
        [Required]
        public int CustomerId { get; set; }    }
}

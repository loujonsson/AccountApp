using AccountApp.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace AccountApp.DTOs.Account
{
    public class AccountUpdateDTO
    {
        [Required]
        public int AccountId { get; set; }

        [AllowedEnumValues(AccountStatus.Active, AccountStatus.Frozen, AccountStatus.Closed)]
        public AccountStatus? Status { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Balance cannot be negative")]
        public decimal? Balance { get; set; }
    }
}

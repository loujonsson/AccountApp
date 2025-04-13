using AccountApp.Models.Enums;

namespace AccountApp.DTOs.Account
{
    public class AccountReadDTO
    {
        public int AccountId { get; set; }
        public int CustomerId { get; set; }
        public DateTime CreationTimestamp { get; set; }
        public DateTime? UpdatedTimestamp { get; set; }
        public AccountStatus Status { get; set; }
        public decimal Balance { get; set; }

    }
}

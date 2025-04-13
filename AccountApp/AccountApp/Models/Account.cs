using AccountApp.Models.Enums;
using System;

namespace AccountApp.Models
{
    public class Account
    {
        public int AccountId { get; set; }
        public int CustomerId { get; set; }
        public DateTime CreationTimestamp { get; set; }
        public DateTime? UpdatedTimestamp { get; set; }
        public AccountStatus Status { get; set; }
        public decimal Balance { get; set; }

        public Customer Customer { get; set; }
    }
}

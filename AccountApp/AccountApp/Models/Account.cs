using AccountApp.Models.Enums;
using System;

namespace AccountApp.Models
{
    public class Account
    {
        public string AccountId { get; set; }
        public int CustomerId { get; set; }
        public DateTime CreationTimestamp { get; set; }
        public DateTime UpdatedTimestamp { get; set; }
        public Status Status { get; set; }
        public decimal Balance { get; set; }
    }
}

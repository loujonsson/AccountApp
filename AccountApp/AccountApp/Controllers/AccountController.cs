using AccountApp.Data;
using AccountApp.DTOs.Account;
using AccountApp.DTOs.Customer;
using AccountApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AccountAppDbContext _context;

        public AccountController(AccountAppDbContext context) 
        {
            _context = context;
        }

        [HttpGet]
        [Route("GetAccounts")]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
        {
            var accounts = await _context.Accounts.ToListAsync();
            return Ok(accounts);
        }

        [HttpGet]
        [Route("GetAccount/{id}")]
        public async Task<ActionResult<Customer>> GetAccount(int accountId)
        {
            var account = await _context.Accounts.FindAsync(accountId);

            if (account == null)
            {
                return NotFound();
            }

            var result = new AccountReadDTO
            {
                AccountId = account.AccountId,
                CustomerId = account.CustomerId,
                CreationTimestamp = account.CreationTimestamp,
                UpdatedTimestamp = account.UpdatedTimestamp,
                Status = account.Status,
                Balance = account.Balance
            };

            return Ok(result);
        }
    }
}

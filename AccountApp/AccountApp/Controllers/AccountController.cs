﻿using AccountApp.Data;
using AccountApp.DTOs.Account;
using AccountApp.DTOs.Customer;
using AccountApp.Models;
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
        public async Task<ActionResult<IEnumerable<AccountReadDTO>>> GetAccounts()
        {
            var accounts = await _context.Accounts.ToListAsync();
            return Ok(accounts);
        }

        [HttpGet]
        [Route("GetAccount/{accountId}")]
        public async Task<ActionResult<AccountReadDTO>> GetAccount(int accountId)
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

        [HttpPost]
        [Route("CreateAccount")]
        public async Task<ActionResult> CreateAccount(AccountCreateDTO accountDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = await _context.Customers.FindAsync(accountDTO.CustomerId);
            if (customer == null)
            {
                return BadRequest("Customer does not exist");
            }

            var account = new Account
            {
                CustomerId = accountDTO.CustomerId,
                Status = Models.Enums.AccountStatus.Active,
                CreationTimestamp = DateTime.Now,
                Balance = accountDTO.Balance != null ? (decimal)accountDTO.Balance : 0
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return Created();
        }

        [HttpDelete]
        [Route("DeleteAccount/{accountId}")]
        public async Task<IActionResult> DeleteAccount(int accountId)
        {
            var account = await _context.Accounts.FindAsync(accountId);
            if (account == null)
            {
                return NotFound();
            }

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut]
        [Route("UpdateAccount/{accountId}")]
        public async Task<IActionResult> UpdateAccount(int accountId, AccountUpdateDTO accountDTO)
        {
            if (accountId != accountDTO.AccountId)
            {
                return BadRequest("Account ID mismatch");
            }

            var existingAccount = await _context.Accounts.FindAsync(accountId);
            if (existingAccount == null)
            {
                return NotFound();
            }

            if (accountDTO.Status != null)
            {
                existingAccount.Status = (Models.Enums.AccountStatus)accountDTO.Status;
            }

            if (accountDTO.Balance != null)
            {
                existingAccount.Balance = (decimal)accountDTO.Balance;
            }

            existingAccount.UpdatedTimestamp = DateTime.Now;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet]
        [Route("GetAccountsForCustomer/{customerId}")]
        public async Task<ActionResult<List<AccountReadDTO>>> GetAccountsForCustomer(int customerId)
        {
            var customer = await _context.Customers
                .Include(c => c.Accounts)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null)
            {
                return NotFound();
            }

            var customerAccounts = customer.Accounts.Select(a => new AccountReadDTO { 
                    AccountId = a.AccountId, 
                    CustomerId = a.CustomerId,
                    Balance = a.Balance,
                    Status = a.Status,
                    CreationTimestamp = a.CreationTimestamp,
                    UpdatedTimestamp = a.UpdatedTimestamp
                }).ToList();

            return Ok(customerAccounts);
        }

        [HttpGet]
        [Route("GetTotalBalanceForCustomer/{customerId}")]
        public async Task<ActionResult<decimal>> GetTotalBalanceForCustomer(int customerId)
        {
            var customer = await _context.Customers
                .Include(c => c.Accounts)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null)
            {
                return NotFound();
            }

            var customerBalance = customer.Accounts.Sum(a => a.Balance);
        
            return Ok(customerBalance);
        }
    }
}

﻿using AccountApp.Data;
using AccountApp.DTOs.Customer;
using AccountApp.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly AccountAppDbContext _context;

        public CustomerController(AccountAppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("GetCustomers")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            var customers = await _context.Customers.ToListAsync();
            return Ok(customers);
        }

        [HttpGet]
        [Route("GetCustomer/{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            var result = new CustomerReadDTO
            {
                CustomerId = customer.CustomerId,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                PhoneNumber = customer.PhoneNumber
            };

            return Ok(result);
        }

        [HttpPost]
        [Route("CreateCustomer")]
        public async Task<ActionResult<Customer>> CreateCustomer(CustomerCreateDTO customerDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = new Customer
            {
                FirstName = customerDTO.FirstName,
                LastName = customerDTO.LastName,
                PhoneNumber = customerDTO.PhoneNumber
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCustomer), new { id = customer.CustomerId }, customer);
        }

        [HttpPut]
        [Route("UpdateCustomer/{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, CustomerUpdateDTO customerDTO)
        {
            if (id != customerDTO.CustomerId)
            {
                return BadRequest("Customer IDs don't match");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingCustomer = await _context.Customers.FindAsync(id);
            if (existingCustomer == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(customerDTO.FirstName))
            {
                existingCustomer.FirstName = customerDTO.FirstName;
            }

            if (!string.IsNullOrWhiteSpace(customerDTO.LastName))
            {
                existingCustomer.LastName = customerDTO.LastName;
            }

            if (!string.IsNullOrWhiteSpace(customerDTO.PhoneNumber))
            {
                existingCustomer.PhoneNumber = customerDTO.PhoneNumber;
            }

            await _context.SaveChangesAsync();
            
            return NoContent();
        }

        [HttpDelete]
        [Route("DeleteCustomer/{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

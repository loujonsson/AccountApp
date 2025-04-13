using AccountApp.Controllers;
using AccountApp.DTOs.Customer;
using AccountApp.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace AccountApp.Tests.Controller
{
    public class CustomerControllerTests : InMemoryDbTestBase
    {
        private readonly CustomerController _sut;

        public CustomerControllerTests()
        {
            _sut = new CustomerController(_context);
        }

        [Fact]
        public async Task GetCustomers_ReturnsOkResult_WithListOfCustomers()
        {
            // Arrange
            var customers = new List<Customer>
            {
                new Customer { CustomerId = 1, FirstName = "Test", LastName = "Testsson", PhoneNumber = "0712344566" },
                new Customer { CustomerId = 2, FirstName = "Johan", LastName = "Johansson", PhoneNumber = "0739481739" }
            };

            _context.Customers.AddRange(customers);
            await _context.SaveChangesAsync();

            // Act
            var result = await _sut.GetCustomers();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(200); 

            var resultCustomers = okResult.Value.Should().BeAssignableTo<IEnumerable<Customer>>().Subject;
            resultCustomers.Should().HaveCount(2);
            resultCustomers.Should().Contain(c => c.FirstName == "Test" && c.LastName == "Testsson");
            resultCustomers.Should().Contain(c => c.FirstName == "Johan" && c.LastName == "Johansson");
        }

        [Fact]
        public async Task GetCustomers_ReturnsOkResult_WithEmptyList_WhenNoCustomers()
        {
            // Arrange            
            // Act
            var result = await _sut.GetCustomers();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(200);
            
            var returnCustomers = okResult.Value.Should().BeAssignableTo<IEnumerable<Customer>>().Subject;
            returnCustomers.Should().BeEmpty();
        }

        [Fact]
        public async Task GetCustomer_ReturnsCustomer_WhenCustomerExists()
        {
            // Arrange
            var customer = new Customer { CustomerId = 1, FirstName = "Test", LastName = "Testsson", PhoneNumber = "0712344566" };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // Act
            var result = await _sut.GetCustomer(1);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(200);

            var returnedCustomer = okResult.Value.Should().BeOfType<CustomerReadDTO>().Subject;
            returnedCustomer.FirstName.Should().Be("Test");
            returnedCustomer.LastName.Should().Be("Testsson");
        }

        [Fact]
        public async Task GetCustomer_ReturnsNotFound_WhenCustomerDoesNotExist()
        {
            // Arrange
            // Act
            var result = await _sut.GetCustomer(404);

            // Assert
            var notFoundResult = result.Result.Should().BeOfType<NotFoundResult>().Subject;
            notFoundResult.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task CreateCustomer_ReturnsCreatedResult_WhenCustomerIsValid()
        {
            // Arrange
            var newCustomer = new CustomerCreateDTO
            {
                FirstName = "Anna",
                LastName = "Eriksson",
                PhoneNumber = "0749274942"
            };

            // Act
            var result = await _sut.CreateCustomer(newCustomer);

            // Assert
            var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.StatusCode.Should().Be(201);

            var returnedCustomer = createdResult.Value.Should().BeOfType<Customer>().Subject;
            returnedCustomer.CustomerId.Should().Be(1);
            returnedCustomer.FirstName.Should().Be("Anna");
            returnedCustomer.LastName.Should().Be("Eriksson");

            var savedCustomer = await _context.Customers.FindAsync(1);
            savedCustomer.Should().NotBeNull();
            savedCustomer.FirstName.Should().Be("Anna");
        }

        [Fact]
        public async Task CreateCustomer_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var invalidCustomer = new CustomerCreateDTO
            {
                FirstName = null,
                LastName = "Eriksson",
                PhoneNumber = "0701234567",
            };

            _sut.ModelState.AddModelError("FirstName", "FirstName is required");

            // Act
            var result = await _sut.CreateCustomer(invalidCustomer);

            // Assert
            result.Should().NotBeNull();
            var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task UpdateCustomer_ReturnsNoContent_WhenCustomerIsUpdatedCompletely()
        {
            // Arrange
            var customer = new Customer
            {
                CustomerId = 1,
                FirstName = "Test",
                LastName = "Testsson",
                PhoneNumber = "0712344566",
            };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            var updatedCustomer = new CustomerUpdateDTO
            {
                CustomerId = 1,
                FirstName = "Updated",
                LastName = "New last name",
                PhoneNumber = "0739481739",
            };

            // Act
            var result = await _sut.UpdateCustomer(1, updatedCustomer);

            // Assert
            var noContentResult = result.Should().BeOfType<NoContentResult>().Subject;
            noContentResult.StatusCode.Should().Be(204);

            var savedCustomer = await _context.Customers.FindAsync(1);
            savedCustomer.FirstName.Should().Be("Updated");
            savedCustomer.PhoneNumber.Should().Be("0739481739");
            savedCustomer.LastName.Should().Be("New last name");
        }

        [Fact]
        public async Task UpdateCustomer_ReturnsNoContent_WhenCustomerIsUpdatedPartly()
        {
            // Arrange
            var customer = new Customer
            {
                CustomerId = 1,
                FirstName = "Test",
                LastName = "Testsson",
                PhoneNumber = "0712344566",
            };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            var updatedCustomer = new CustomerUpdateDTO
            {
                CustomerId = 1,
                FirstName = "Updated",
                PhoneNumber = "0739481739",
            };

            // Act
            var result = await _sut.UpdateCustomer(1, updatedCustomer);

            // Assert
            var noContentResult = result.Should().BeOfType<NoContentResult>().Subject;
            noContentResult.StatusCode.Should().Be(204);

            var savedCustomer = await _context.Customers.FindAsync(1);
            savedCustomer.FirstName.Should().Be("Updated");
            savedCustomer.PhoneNumber.Should().Be("0739481739");
            savedCustomer.LastName.Should().Be("Testsson");
        }

        [Fact]
        public async Task UpdateCustomer_ReturnsNotFound_WhenCustomerDoesNotExist()
        {
            // Arrange
            var updatedCustomer = new CustomerUpdateDTO
            {
                CustomerId = 404,
                FirstName = "Updated",
                LastName = "Testsson",
                PhoneNumber = "0739481739"
            };

            // Act
            var result = await _sut.UpdateCustomer(404, updatedCustomer);

            // Assert
            var notFoundResult = result.Should().BeOfType<NotFoundResult>().Subject;
            notFoundResult.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task UpdateCustomer_ReturnsBadRequest_WhenIdsDoNotMatch()
        {
            // Arrange
            var customer = new Customer
            {
                CustomerId = 1,
                FirstName = "Test",
                LastName = "Testsson",
                PhoneNumber = "0712344566"
            };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            var updatedCustomer = new CustomerUpdateDTO
            {
                CustomerId = 2, // Fel ID
                FirstName = "Updated",
                LastName = "Testsson",
                PhoneNumber = "0739481739"
            };

            // Act
            var result = await _sut.UpdateCustomer(1, updatedCustomer);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task DeleteCustomer_ReturnsNoContent_WhenCustomerIsDeleted()
        {
            // Arrange
            var customer = new Customer
            {
                CustomerId = 1,
                FirstName = "Test",
                LastName = "Testsson",
                PhoneNumber = "0712344566"
            };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // Act
            var result = await _sut.DeleteCustomer(1);

            // Assert
            var noContentResult = result.Should().BeOfType<NoContentResult>().Subject;
            noContentResult.StatusCode.Should().Be(204);

            // Verifiera att kunden togs bort
            var deletedCustomer = await _context.Customers.FindAsync(1);
            deletedCustomer.Should().BeNull();
        }

        [Fact]
        public async Task DeleteCustomer_ReturnsNotFound_WhenCustomerDoesNotExist()
        {
            // Arrange
            // Act
            var result = await _sut.DeleteCustomer(404);

            // Assert
            var notFoundResult = result.Should().BeOfType<NotFoundResult>().Subject;
            notFoundResult.StatusCode.Should().Be(404);
        }
    }
}
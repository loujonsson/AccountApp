using AccountApp.Controllers;
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

            var returnedCustomer = okResult.Value.Should().BeOfType<Customer>().Subject;
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
            var newCustomer = new DTOs.CustomerCreateDTO
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
    }
}
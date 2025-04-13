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
    }
}
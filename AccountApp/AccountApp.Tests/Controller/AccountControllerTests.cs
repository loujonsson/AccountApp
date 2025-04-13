using AccountApp.Controllers;
using AccountApp.DTOs.Account;
using AccountApp.DTOs.Customer;
using AccountApp.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountApp.Tests.Controller
{
    public class AccountControllerTests : InMemoryDbTestBase
    {
        private readonly AccountController _sut;

        public AccountControllerTests()
        {
            _sut = new AccountController(_context);
        }

        public async void SetupAccounts()
        {
            var accounts = new List<Account>
            {
                new Account { AccountId = 1, CustomerId = 1, CreationTimestamp = new DateTime(2025, 02, 18), UpdatedTimestamp = null, Status = Models.Enums.AccountStatus.Active, Balance = 1000.57m },
                new Account { AccountId = 2, CustomerId = 2, CreationTimestamp = new DateTime(2024, 02, 18), UpdatedTimestamp = null, Status = Models.Enums.AccountStatus.Frozen, Balance = 16.45m },
            };

            _context.Accounts.AddRange(accounts);
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task GetAccounts_ReturnsOkResult_WithListOfAcounts()
        {
            // Arrange
            SetupAccounts();

            // Act
            var result = await _sut.GetAccounts();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(200);

            var resultAccounts = okResult.Value.Should().BeAssignableTo<IEnumerable<Account>>().Subject;
            resultAccounts.Should().HaveCount(2);
            resultAccounts.Should().Contain(a => a.Status == Models.Enums.AccountStatus.Active && a.Balance == 1000.57m);
            resultAccounts.Should().Contain(a => a.Status == Models.Enums.AccountStatus.Frozen && a.Balance == 16.45m);
        }

        [Fact]
        public async Task GetAccounts_ReturnsOkResult_WithEmptyList_WhenNoAccounts()
        {
            // Arrange            
            // Act
            var result = await _sut.GetAccounts();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(200);

            var returnAccounts = okResult.Value.Should().BeAssignableTo<IEnumerable<Account>>().Subject;
            returnAccounts.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAccount_ReturnsAccount_WhenAccountExists()
        {
            // Arrange
            SetupAccounts();

            // Act
            var result = await _sut.GetAccount(1);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(200);

            var returnAccounts = okResult.Value.Should().BeOfType<AccountReadDTO>().Subject;
            returnAccounts.Balance.Should().Be(1000.57m);
            returnAccounts.CustomerId.Should().Be(1);
        }

        [Fact]
        public async Task GetAccount_ReturnsNotFound_WhenAccountDoesNotExist()
        {
            // Arrange
            // Act
            var result = await _sut.GetAccount(404);

            // Assert
            var notFoundResult = result.Result.Should().BeOfType<NotFoundResult>().Subject;
            notFoundResult.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task CreateAccount_ReturnsCreatedResult_WhenCustomerExists()
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

            var newAccount = new AccountCreateDTO
            {
                CustomerId = 1,
            };

            // Act
            var result = await _sut.CreateAccount(newAccount);

            // Assert
            var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.StatusCode.Should().Be(201);

            var returnedAccount = createdResult.Value.Should().BeOfType<Account>().Subject;
            returnedAccount.AccountId.Should().Be(1);
            returnedAccount.CustomerId.Should().Be(1);
            returnedAccount.Status.Should().Be(Models.Enums.AccountStatus.Active);
            returnedAccount.Balance.Should().Be(0);

            var savedAccount = await _context.Accounts.FindAsync(1);
            savedAccount.Should().NotBeNull();
            savedAccount.Status.Should().Be(Models.Enums.AccountStatus.Active);
        }

        [Fact]
        public async Task CreateAccount_ReturnsBadRequest_WhenCustomerDoesNotExist()
        {
            // Arrange
            var newAccount = new AccountCreateDTO
            {
                CustomerId = 404, 
                Balance = 1000.00m
            };

            // Act
            var result = await _sut.CreateAccount(newAccount);

            // Assert
            result.Should().NotBeNull();
            var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("Customer does not exist");
        }

        [Fact]
        public async Task DeleteAccount_ReturnsNoContent_WhenAccountIsDeleted()
        {
            // Arrange
            var customer = new Customer
            {
                CustomerId = 1,
                FirstName = "Test",
                LastName = "Testsson",
                PhoneNumber = "0712344566"
            };
            var account = new Account
            {
                AccountId = 1,
                CustomerId = 1,
                Status = Models.Enums.AccountStatus.Active,
                Balance = 1000.00m
            };
            _context.Accounts.Add(account);
            _context.Customers.Add(customer);

            await _context.SaveChangesAsync();

            // Act
            var result = await _sut.DeleteAccount(1);

            // Assert
            var noContentResult = result.Should().BeOfType<NoContentResult>().Subject;
            noContentResult.StatusCode.Should().Be(204);

            var deletedAccount = await _context.Accounts.FindAsync(1);
            deletedAccount.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAccount_ReturnsNotFound_WhenAccountDoesNotExist()
        {
            // Arrange
            // Act
            var result = await _sut.DeleteAccount(404);

            // Assert
            var notFoundResult = result.Should().BeOfType<NotFoundResult>().Subject;
            notFoundResult.StatusCode.Should().Be(404);
        }
    }
}

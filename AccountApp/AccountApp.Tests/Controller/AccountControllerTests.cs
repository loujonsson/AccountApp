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
    }
}

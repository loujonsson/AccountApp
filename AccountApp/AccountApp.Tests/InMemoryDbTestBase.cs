using AccountApp.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountApp.Tests
{
    public class InMemoryDbTestBase : IDisposable
    {
        private readonly DbContextOptions<AccountAppDbContext> _options;
        protected readonly AccountAppDbContext _context;

        public InMemoryDbTestBase()
        {
            _options = new DbContextOptionsBuilder<AccountAppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AccountAppDbContext(_options);
            _context.Database.EnsureDeleted();
        }

        public AccountAppDbContext CreateNewContext()
        {
            var context = new AccountAppDbContext(_options);
            context.Database.EnsureDeleted();
            return context;
        }

        public void Dispose() 
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}

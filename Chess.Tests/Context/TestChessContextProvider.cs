using Microservice.Framework.Persistence.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Tests.Context
{
    public class TestChessContextProvider : IDbContextProvider<ChessInMemoryContext>, IDisposable
    {
        private readonly DbContextOptions<ChessInMemoryContext> _options;

        public TestChessContextProvider()
        {
            _options = new DbContextOptionsBuilder<ChessInMemoryContext>()
                .UseInMemoryDatabase(databaseName: "Chess")
                .Options;
        }

        public ChessInMemoryContext CreateContext()
        {
            return new ChessInMemoryContext(_options);
        }

        public void Dispose()
        {
        }
    }
}

using Chess.Persistence.Mappings;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Tests.Context
{
    public class ChessInMemoryContext : DbContext
    {
        public ChessInMemoryContext(DbContextOptions<ChessInMemoryContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ChessModelMap();
        }
    }
}

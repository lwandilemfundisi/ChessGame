using Chess.Persistence.Mappings;
using Microsoft.EntityFrameworkCore;

namespace Chess.Persistence
{
    public class ChessContext : DbContext
    {
        public ChessContext(DbContextOptions<ChessContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ChessModelMap();
        }
    }
}

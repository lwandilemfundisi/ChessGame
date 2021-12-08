using Chess.Domain.DomianModel.ChessModel;
using Chess.Domain.DomianModel.ChessModel.Entities;
using Chess.Domain.DomianModel.ChessModel.ValueObjects.LookupValueObjects;
using Chess.Persistence.ValueObjectConverters;
using Microsoft.EntityFrameworkCore;

namespace Chess.Persistence.Mappings
{
    public static class ChessModelMapping
    {
        public static ModelBuilder ChessModelMap(this ModelBuilder modelBuilder)
        {
            modelBuilder
            .Entity<Board>()
            .Property(o => o.Id)
            .HasConversion(new SingleValueObjectIdentityValueConverter<BoardId>());

            modelBuilder
            .Entity<Block>()
            .Property(o => o.Id)
            .HasConversion(new SingleValueObjectIdentityValueConverter<BlockId>());

            modelBuilder
            .Entity<Block>()
            .Property(o => o.BlockColor)
            .HasConversion(new ValueObjectValueConverter<Color, Colors>());

            modelBuilder
            .Entity<ChessPiece>()
            .Property(o => o.Id)
            .HasConversion(new SingleValueObjectIdentityValueConverter<ChessPieceId>());

            modelBuilder
            .Entity<ChessPiece>()
            .Property(o => o.PieceColor)
            .HasConversion(new ValueObjectValueConverter<Color, Colors>());

            modelBuilder
            .Entity<ChessPiece>()
            .Property(o => o.PieceName)
            .HasConversion(new ValueObjectValueConverter<PieceName, PieceNames>());

            modelBuilder
                .Entity<Block>()
                .HasOne<Board>()
                .WithMany(c => c.Blocks);

            modelBuilder
                .Entity<ChessPiece>()
                .HasOne<Block>();

            return modelBuilder;
        }
    }
}

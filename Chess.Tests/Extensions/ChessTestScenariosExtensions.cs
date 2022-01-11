using Chess.Domain.DomianModel.ChessModel.Entities;
using Chess.Domain.DomianModel.ChessModel.ValueObjects.LookupValueObjects;
using Chess.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Tests.Extensions
{
    public static class ChessTestScenariosExtensions
    {
        public static IReadOnlyList<Block> PlacePawnValidCaptureScenario(this IReadOnlyList<Block> blocks)
        {
            blocks.PlacePiece(1, 2, new ChessPiece 
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().White,
                PieceName = PieceNames.Of().Pawn,
                XCoordinate = 1,
                YCoordinate = 2
            });

            blocks.PlacePiece(2, 3, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().Black,
                PieceName = PieceNames.Of().Pawn,
                XCoordinate = 2,
                YCoordinate = 3
            });

            return blocks;
        }

        public static IReadOnlyList<Block> PlacePawnInvalidCaptureScenario(this IReadOnlyList<Block> blocks)
        {
            blocks.PlacePiece(1, 2, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().White,
                PieceName = PieceNames.Of().Pawn,
                XCoordinate = 1,
                YCoordinate = 2
            });

            blocks.PlacePiece(2, 3, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().White,
                PieceName = PieceNames.Of().Pawn,
                XCoordinate = 2,
                YCoordinate = 3
            });

            return blocks;
        }
    }
}

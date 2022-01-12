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
        public static IReadOnlyList<Block> PlaceBlackPawnValidCaptureScenario(this IReadOnlyList<Block> blocks)
        {
            blocks.PlacePiece(1, 7, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().Black,
                PieceName = PieceNames.Of().Pawn,
                XCoordinate = 1,
                YCoordinate = 7
            });

            blocks.PlacePiece(2, 6, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().White,
                PieceName = PieceNames.Of().Pawn,
                XCoordinate = 2,
                YCoordinate = 6
            });

            return blocks;
        }

        public static IReadOnlyList<Block> PlaceWhitePawnValidCaptureScenario(this IReadOnlyList<Block> blocks)
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

        public static IReadOnlyList<Block> PlaceBlackPawnInvalidCaptureScenario(this IReadOnlyList<Block> blocks)
        {
            blocks.PlacePiece(1, 7, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().Black,
                PieceName = PieceNames.Of().Pawn,
                XCoordinate = 1,
                YCoordinate = 7
            });

            blocks.PlacePiece(2, 6, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().Black,
                PieceName = PieceNames.Of().Pawn,
                XCoordinate = 2,
                YCoordinate = 6
            });

            return blocks;
        }

        public static IReadOnlyList<Block> PlaceWhitePawnInvalidCaptureScenario(this IReadOnlyList<Block> blocks)
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

        public static IReadOnlyList<Block> PlaceCannotLeapOverPieceBlackScenario(this IReadOnlyList<Block> blocks)
        {
            blocks.PlacePiece(1, 7, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().Black,
                PieceName = PieceNames.Of().Pawn,
                XCoordinate = 1,
                YCoordinate = 7
            });

            blocks.PlacePiece(1, 6, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().Black,
                PieceName = PieceNames.Of().Pawn,
                XCoordinate = 1,
                YCoordinate = 6
            });

            return blocks;
        }

        public static IReadOnlyList<Block> PlaceCannotLeapOverPieceWhiteScenario(this IReadOnlyList<Block> blocks)
        {
            blocks.PlacePiece(1, 2, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().White,
                PieceName = PieceNames.Of().Pawn,
                XCoordinate = 1,
                YCoordinate = 2
            });

            blocks.PlacePiece(1, 3, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().White,
                PieceName = PieceNames.Of().Pawn,
                XCoordinate = 1,
                YCoordinate = 3
            });

            return blocks;
        }
    }
}

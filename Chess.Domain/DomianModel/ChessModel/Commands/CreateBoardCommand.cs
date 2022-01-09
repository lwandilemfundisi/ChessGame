using Chess.Domain.DomianModel.ChessModel.Entities;
using Chess.Domain.DomianModel.ChessModel.ValueObjects.LookupValueObjects;
using Microservice.Framework.Domain.Commands;
using Microservice.Framework.Domain.ExecutionResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chess.Domain.DomianModel.ChessModel.Commands
{
    public class CreateBoardCommand
        : Command<Board, BoardId>
    {
        #region Constructors

        public CreateBoardCommand(BoardId boardId)
            : base(boardId)
        {
        }

        #endregion
    }

    public class CreateBoardCommandHandler
        : CommandHandler<Board, BoardId, CreateBoardCommand>
    {
        #region Virtual Methods

        public override Task<IExecutionResult> ExecuteAsync(
            Board aggregate, 
            CreateBoardCommand command, 
            CancellationToken cancellationToken)
        {
            aggregate
                .CreateBoard(
                PopulateBoardWithPieces(
                    BuildBoard()));

            return Task.FromResult(ExecutionResult.Success());
        }

        #endregion

        #region Private Methods

        private IReadOnlyList<Block> BuildBoard()
        {
            var blocks = new List<Block>();

            Color colorToAssignBlock = null;
            for (int x = 1; x <= 8; x++)
            {
                if (x == 1)
                    colorToAssignBlock = Colors.Of().White;

                for (int y = 1; y <= 8; y++)
                {
                    blocks.Add(new Block
                    {
                        Id = BlockId.New,
                        BlockColor = colorToAssignBlock,
                        XCoordinate = (uint)x,
                        YCoordinate = (uint)y,
                    });

                    if (y == 8)
                        break;

                    if (colorToAssignBlock.IsIn(Colors.Of().White))
                        colorToAssignBlock = Colors.Of().Black;
                    else
                        colorToAssignBlock = Colors.Of().White;
                }
            }

            return blocks;
        }

        private IReadOnlyList<Block> PopulateBoardWithPieces(IReadOnlyList<Block> blocks)
        {
            PlacePiece(blocks, 1, 8, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().Black,
                PieceName = PieceNames.Of().Rook,
                XCoordinate = 1,
                YCoordinate = 8
            });
            PlacePiece(blocks, 8, 8, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().Black,
                PieceName = PieceNames.Of().Rook,
                XCoordinate = 8,
                YCoordinate = 8
            });
            PlacePiece(blocks, 2, 8, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().Black,
                PieceName = PieceNames.Of().Night,
                XCoordinate = 2,
                YCoordinate = 8
            });
            PlacePiece(blocks, 7, 8, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().Black,
                PieceName = PieceNames.Of().Night,
                XCoordinate = 7,
                YCoordinate = 8
            });
            PlacePiece(blocks, 3, 8, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().Black,
                PieceName = PieceNames.Of().Bishop,
                XCoordinate = 3,
                YCoordinate = 8
            });
            PlacePiece(blocks, 6, 8, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().Black,
                PieceName = PieceNames.Of().Bishop,
                XCoordinate = 6,
                YCoordinate = 8
            });
            PlacePiece(blocks, 4, 8, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().Black,
                PieceName = PieceNames.Of().King,
                XCoordinate = 4,
                YCoordinate = 8
            });
            PlacePiece(blocks, 5, 8, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().Black,
                PieceName = PieceNames.Of().Queen,
                XCoordinate = 5,
                YCoordinate = 8
            });
            PlacePawns(blocks, 7, Colors.Of().Black);
            PlacePawns(blocks, 2, Colors.Of().White);
            PlacePiece(blocks, 1, 1, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().White,
                PieceName = PieceNames.Of().Rook,
                XCoordinate = 1,
                YCoordinate = 1
            });
            PlacePiece(blocks, 8, 1, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().White,
                PieceName = PieceNames.Of().Rook,
                XCoordinate = 8,
                YCoordinate = 1
            });
            PlacePiece(blocks, 2, 1, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().White,
                PieceName = PieceNames.Of().Night,
                XCoordinate = 2,
                YCoordinate = 1
            });
            PlacePiece(blocks, 7, 1, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().White,
                PieceName = PieceNames.Of().Night,
                XCoordinate = 7,
                YCoordinate = 1
            });
            PlacePiece(blocks, 3, 1, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().White,
                PieceName = PieceNames.Of().Bishop,
                XCoordinate = 3,
                YCoordinate = 1
            });
            PlacePiece(blocks, 6, 1, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().White,
                PieceName = PieceNames.Of().Bishop,
                XCoordinate = 6,
                YCoordinate = 1
            });
            PlacePiece(blocks, 4, 1, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().White,
                PieceName = PieceNames.Of().King,
                XCoordinate = 4,
                YCoordinate = 1
            });
            PlacePiece(blocks, 5, 1, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().White,
                PieceName = PieceNames.Of().Queen,
                XCoordinate = 5,
                YCoordinate = 1
            });

            return blocks;
        }

        private void PlacePiece(
            IReadOnlyList<Block> blocks, 
            uint x, 
            uint y, 
            ChessPiece piece)
        {
            blocks.First(b => b.XCoordinate == x
            && b.YCoordinate == y).ChessPiece = piece;
        }

        private void PlacePawns(IReadOnlyList<Block> blocks, uint startYCoordinate, Color pawnColor)
        {
            for (int x = 1; x <= 8; x++)
            {
                PlacePiece(blocks, (uint)x, startYCoordinate, new ChessPiece
                {
                    Id = ChessPieceId.New,
                    PieceColor = pawnColor,
                    PieceName = PieceNames.Of().Pawn,
                    XCoordinate = (uint)x,
                    YCoordinate = startYCoordinate
                });
            }
        }

        #endregion
    }
}

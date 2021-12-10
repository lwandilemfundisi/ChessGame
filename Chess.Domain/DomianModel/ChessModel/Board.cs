using Chess.Domain.DomianModel.ChessModel.Entities;
using Chess.Domain.DomianModel.ChessModel.Events;
using Chess.Domain.DomianModel.ChessModel.ValueObjects;
using Chess.Domain.DomianModel.ChessModel.ValueObjects.LookupValueObjects;
using Chess.Domain.Extensions;
using Microservice.Framework.Domain.Aggregates;
using Microservice.Framework.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Domain.DomianModel.ChessModel
{
    public class Board : AggregateRoot<Board, BoardId>
    {
        private IList<Block> _blocks;

        #region Constructors

        public Board()
            : base(null)
        {

        }

        public Board(BoardId boardId)
            : base(boardId)
        {

        }

        private Board(Action<object, string> lazyLoader)
            : base(null)
        {
            LazyLoader = lazyLoader;
        }

        #endregion

        #region Properties

        private Action<object, string> LazyLoader { get; set; }

        public IList<Block> Blocks 
        {
            get => LazyLoader.Load(this, ref _blocks);
            set => _blocks = value;
        }

        #endregion

        #region Methods

        public void CreateBoard()
        {
            Specs.AggregateIsNew.ThrowDomainErrorIfNotSatisfied(this);
            BuildBoard();
            PopulateBoardWithPieces();
            Emit(new CreatedBoardEvent());
        }

        public void MovePiece(Move move)
        {
            Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);

            move.Specification(new ReadOnlyCollection<Block>(Blocks))
                .ThrowDomainErrorIfNotSatisfied(GetPiece(move.PieceId));

            //If we got at this point, the move was valid
            Move(move);

            Emit(new MovedPieceEvent());
        }

        public void ResetBoard()
        {
            Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
            Emit(new ResetBoardEvent());
        }

        #endregion

        #region Private Methods

        private ChessPiece GetPiece(ChessPieceId pieceId)
        {
            //To Review [Can be done better]
            return Blocks
                .First(s => s.ChessPiece?.Id == pieceId)
                .ChessPiece;
        }

        private void Move(Move move)
        {
            //Where we are moving from block
            var occupiedBlock = Blocks.First(b =>
                                 b.ChessPiece?.Id == move.PieceId);

            //Where we are moving from index
            var occupiedBlockIdx = Blocks
                .IndexOf(occupiedBlock);

            //Where we are moving to index
            var newBlockIdx = Blocks
                .IndexOf(Blocks.First(b => 
                                b.XCoordinate == move.NewXCoordinate 
                                    && b.YCoordinate == move.NewYCoordinate));

            //Keep a reference of the piece we want to move
            var pieceToMove = GetPiece(move.PieceId);

            //Now move the piece to the new block
            Blocks[newBlockIdx].ChessPiece = new ChessPiece
            {
                Id = pieceToMove.Id,
                PieceColor = pieceToMove.PieceColor,
                PieceName = pieceToMove.PieceName,
                XCoordinate = move.NewXCoordinate,
                YCoordinate = move.NewYCoordinate
            };

            //Clean the last block that was occupied
            Blocks[occupiedBlockIdx].ChessPiece = null;
        }

        private void BuildBoard()
        {
            Blocks = new List<Block>();

            Color colorToAssignBlock = null;
            for (int x = 1; x <= 8; x++)
            {
                if(x == 1)
                    colorToAssignBlock = Colors.Of().White;

                for (int y = 1; y <= 8; y++)
                {
                    Blocks.Add(new Block
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
        }

        private void PopulateBoardWithPieces()
        {
            PlacePiece(1, 1, new ChessPiece 
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().Black,
                PieceName = PieceNames.Of().Rook,
                XCoordinate = 1,
                YCoordinate = 1
            });

            PlacePiece(1, 8, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().Black,
                PieceName = PieceNames.Of().Rook,
                XCoordinate = 1,
                YCoordinate = 8
            });

            PlacePiece(1, 2, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().Black,
                PieceName = PieceNames.Of().Night,
                XCoordinate = 1,
                YCoordinate = 2
            });

            PlacePiece(1, 7, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().Black,
                PieceName = PieceNames.Of().Night,
                XCoordinate = 1,
                YCoordinate = 7
            });

            PlacePiece(1, 3, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().Black,
                PieceName = PieceNames.Of().Bishop,
                XCoordinate = 1,
                YCoordinate = 3
            });

            PlacePiece(1, 6, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().Black,
                PieceName = PieceNames.Of().Bishop,
                XCoordinate = 1,
                YCoordinate = 6
            });

            PlacePiece(1, 4, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().Black,
                PieceName = PieceNames.Of().King,
                XCoordinate = 1,
                YCoordinate = 4
            });

            PlacePiece(1, 5, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().Black,
                PieceName = PieceNames.Of().Queen,
                XCoordinate = 1,
                YCoordinate = 5
            });

            PlacePawns(2, Colors.Of().Black);

            PlacePawns(7, Colors.Of().White);

            PlacePiece(8, 1, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().White,
                PieceName = PieceNames.Of().Rook,
                XCoordinate = 8,
                YCoordinate = 1
            });

            PlacePiece(8, 8, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().White,
                PieceName = PieceNames.Of().Rook,
                XCoordinate = 8,
                YCoordinate = 8
            });

            PlacePiece(8, 2, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().White,
                PieceName = PieceNames.Of().Night,
                XCoordinate = 8,
                YCoordinate = 2
            });

            PlacePiece(8, 7, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().White,
                PieceName = PieceNames.Of().Night,
                XCoordinate = 8,
                YCoordinate = 7
            });

            PlacePiece(8, 3, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().White,
                PieceName = PieceNames.Of().Bishop,
                XCoordinate = 8,
                YCoordinate = 3
            });

            PlacePiece(8, 6, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().White,
                PieceName = PieceNames.Of().Bishop,
                XCoordinate = 8,
                YCoordinate = 6
            });

            PlacePiece(8, 4, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().White,
                PieceName = PieceNames.Of().King,
                XCoordinate = 8,
                YCoordinate = 4
            });

            PlacePiece(8, 5, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().White,
                PieceName = PieceNames.Of().Queen,
                XCoordinate = 8,
                YCoordinate = 5
            });
        }

        private void PlacePiece(uint x, uint y, ChessPiece piece)
        {
            Blocks.First(b => b.XCoordinate == x 
            && b.YCoordinate == y).ChessPiece = piece;
        }

        private void PlacePawns(uint startXCoordinate, Color pawnColor)
        {
            for (int y = 1; y <= 8; y++)
            {
                PlacePiece(startXCoordinate, (uint)y, new ChessPiece
                {
                    Id = ChessPieceId.New,
                    PieceColor = pawnColor,
                    PieceName = PieceNames.Of().Pawn,
                    XCoordinate = startXCoordinate,
                    YCoordinate = (uint)y
                });
            }
        }

        #endregion
    }
}

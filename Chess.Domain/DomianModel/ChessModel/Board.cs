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
                .ThrowDomainErrorIfNotSatisfied(move);

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
            ////Where we are moving from block
            //var occupiedBlock = Blocks.First(b =>
            //                     b.ChessPiece?.Id == move.PieceId);

            ////Where we are moving from index
            //var occupiedBlockIdx = Blocks
            //    .IndexOf(occupiedBlock);

            ////Where we are moving to index
            //var newBlockIdx = Blocks
            //    .IndexOf(Blocks.First(b => 
            //                    b.XCoordinate == move.NewXCoordinate 
            //                        && b.YCoordinate == move.NewYCoordinate));

            ////Keep a reference of the piece we want to move
            //var pieceToMove = GetPiece(move.PieceId);

            ////Now move the piece to the new block
            //Blocks[newBlockIdx].ChessPiece = new ChessPiece
            //{
            //    Id = pieceToMove.Id,
            //    PieceColor = pieceToMove.PieceColor,
            //    PieceName = pieceToMove.PieceName,
            //    XCoordinate = move.NewXCoordinate,
            //    YCoordinate = move.NewYCoordinate
            //};

            ////Clean the last block that was occupied
            //Blocks[occupiedBlockIdx].ChessPiece = null;
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
            PlacePiece(1, 8, new ChessPiece 
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().Black,
                PieceName = PieceNames.Of().Rook,
                XCoordinate = 1,
                YCoordinate = 8
            });
            PlacePiece(8, 8, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().Black,
                PieceName = PieceNames.Of().Rook,
                XCoordinate = 8,
                YCoordinate = 8
            });
            PlacePiece(2, 8, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().Black,
                PieceName = PieceNames.Of().Night,
                XCoordinate = 2,
                YCoordinate = 8
            });
            PlacePiece(7, 8, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().Black,
                PieceName = PieceNames.Of().Night,
                XCoordinate = 7,
                YCoordinate = 8
            });
            PlacePiece(3, 8, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().Black,
                PieceName = PieceNames.Of().Bishop,
                XCoordinate = 3,
                YCoordinate = 8
            });
            PlacePiece(6, 8, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().Black,
                PieceName = PieceNames.Of().Bishop,
                XCoordinate = 6,
                YCoordinate = 8
            });
            PlacePiece(4, 8, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().Black,
                PieceName = PieceNames.Of().King,
                XCoordinate = 4,
                YCoordinate = 8
            });
            PlacePiece(5, 8, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().Black,
                PieceName = PieceNames.Of().Queen,
                XCoordinate = 5,
                YCoordinate = 8
            });
            PlacePawns(7, Colors.Of().Black);
            PlacePawns(2, Colors.Of().White);
            PlacePiece(1, 1, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().White,
                PieceName = PieceNames.Of().Rook,
                XCoordinate = 1,
                YCoordinate = 1
            });
            PlacePiece(8, 1, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().White,
                PieceName = PieceNames.Of().Rook,
                XCoordinate = 8,
                YCoordinate = 1
            });
            PlacePiece(2, 1, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().White,
                PieceName = PieceNames.Of().Night,
                XCoordinate = 2,
                YCoordinate = 1
            });
            PlacePiece(7, 1, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().White,
                PieceName = PieceNames.Of().Night,
                XCoordinate = 7,
                YCoordinate = 1
            });
            PlacePiece(3, 1, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().White,
                PieceName = PieceNames.Of().Bishop,
                XCoordinate = 3,
                YCoordinate = 1
            });
            PlacePiece(6, 1, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().White,
                PieceName = PieceNames.Of().Bishop,
                XCoordinate = 6,
                YCoordinate = 1
            });
            PlacePiece(4, 1, new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().White,
                PieceName = PieceNames.Of().King,
                XCoordinate = 4,
                YCoordinate = 1
            });
            PlacePiece(5, 1,  new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceColor = Colors.Of().White,
                PieceName = PieceNames.Of().Queen,
                XCoordinate = 5,
                YCoordinate = 1
            });
        }

        private void PlacePiece(uint x, uint y, ChessPiece piece)
        {
            Blocks.First(b => b.XCoordinate == x 
            && b.YCoordinate == y).ChessPiece = piece;
        }

        private void PlacePawns(uint startYCoordinate, Color pawnColor)
        {
            for (int x = 1; x <= 8; x++)
            {
                PlacePiece((uint)x, startYCoordinate, new ChessPiece
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

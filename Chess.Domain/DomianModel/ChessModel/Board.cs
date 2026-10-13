using Chess.Domain.DomianModel.ChessModel.Entities;
using Chess.Domain.DomianModel.ChessModel.Events;
using Chess.Domain.DomianModel.ChessModel.ValueObjects;
using Chess.Domain.DomianModel.ChessModel.ValueObjects.LookupValueObjects;
using Chess.Domain.DomianModel.ChessModel.Specifications;
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

        public bool WhiteToMove { get; private set; } = true;
        public uint EnPassantTargetX { get; private set; }
        public uint EnPassantTargetY { get; private set; }
        public string GameStatus { get; private set; } = Rules.ChessRules.InProgress;
        public int HalfMoveClock { get; private set; }
        public string PositionHistory { get; private set; } = string.Empty;

        #endregion

        #region Methods

        public void CreateBoard(IReadOnlyList<Block> blocks, bool requireKings = false)
        {
            Specs.AggregateIsNew.ThrowDomainErrorIfNotSatisfied(this);
            Blocks = blocks.ToList();
            new BoardSpecification(requireKings).ThrowDomainErrorIfNotSatisfied(this);
            WhiteToMove = true;
            EnPassantTargetX = 0;
            EnPassantTargetY = 0;
            GameStatus = Rules.ChessRules.InProgress;
            HalfMoveClock = 0;
            PositionHistory = HasBothKings() ? Rules.ChessRules.PositionKey(Blocks.ToList(), WhiteToMove, EnPassantTargetX, EnPassantTargetY) : string.Empty;
            Emit(new CreatedBoardEvent());
        }

        public void MovePiece(Move move)
        {
            Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);

            new GameTurnSpecification(this).ThrowDomainErrorIfNotSatisfied(move);
            move.Specification(new ReadOnlyCollection<Block>(Blocks), EnPassantTargetX, EnPassantTargetY)
                .ThrowDomainErrorIfNotSatisfied(move);

            var movingPieceBeforeMove = Blocks.First(b => b.ChessPiece?.Id == move.PieceId).ChessPiece;
            var capturedPieceBeforeMove = Blocks.First(b => b.XCoordinate == move.NewXCoordinate && b.YCoordinate == move.NewYCoordinate).ChessPiece;
            var isPawnMove = movingPieceBeforeMove.PieceName.Text == "Pawn";

            //If we got at this point, the move was valid
            Move(move);

            if (HasBothKings())
            {
                WhiteToMove = !WhiteToMove;
                HalfMoveClock = isPawnMove || capturedPieceBeforeMove != null ? 0 : HalfMoveClock + 1;
                var positionKey = Rules.ChessRules.PositionKey(Blocks.ToList(), WhiteToMove, EnPassantTargetX, EnPassantTargetY);
                PositionHistory = string.IsNullOrEmpty(PositionHistory) ? positionKey : PositionHistory + "\n" + positionKey;
                GameStatus = Rules.ChessRules.GetPositionStatus(Blocks.ToList(), WhiteToMove, EnPassantTargetX, EnPassantTargetY);
                if (GameStatus != Rules.ChessRules.Checkmate && GameStatus != Rules.ChessRules.Stalemate)
                {
                    if (Rules.ChessRules.IsInsufficientMaterial(Blocks.ToList())) GameStatus = Rules.ChessRules.DrawInsufficientMaterial;
                    else if (Rules.ChessRules.IsFiftyMoveDraw(HalfMoveClock)) GameStatus = Rules.ChessRules.DrawFiftyMove;
                    else if (PositionHistory.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).Count(k => k == positionKey) >= 3)
                        GameStatus = Rules.ChessRules.DrawThreefoldRepetition;
                }
            }

            Emit(new MovedPieceEvent(move));
        }

        public void ResetBoard()
        {
            Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
            Blocks = ChessExtensions.BuildBoard().PlaceAllPieces().ToList();
            WhiteToMove = true;
            EnPassantTargetX = 0;
            EnPassantTargetY = 0;
            GameStatus = Rules.ChessRules.InProgress;
            HalfMoveClock = 0;
            PositionHistory = Rules.ChessRules.PositionKey(Blocks.ToList(), WhiteToMove, EnPassantTargetX, EnPassantTargetY);
            Emit(new ResetBoardEvent());
        }

        #endregion

        #region Private Methods

        private void Move(Move move)
        {
            var origin = Blocks.First(b => b.ChessPiece?.Id == move.PieceId);
            var destination = Blocks.First(b => b.XCoordinate == move.NewXCoordinate && b.YCoordinate == move.NewYCoordinate);
            var piece = origin.ChessPiece;
            var enPassant = piece.PieceName.Text == "Pawn" && destination.ChessPiece == null &&
                origin.XCoordinate != move.NewXCoordinate && move.NewXCoordinate == EnPassantTargetX && move.NewYCoordinate == EnPassantTargetY;
            var isCastle = piece.PieceName.Text == "King" && Math.Abs((int)move.NewXCoordinate - (int)origin.XCoordinate) == 2;
            var doublePawnMove = piece.PieceName.Text == "Pawn" && Math.Abs((int)move.NewYCoordinate - (int)origin.YCoordinate) == 2;

            if (enPassant)
                Blocks.First(b => b.XCoordinate == move.NewXCoordinate && b.YCoordinate == origin.YCoordinate).ChessPiece = null;

            destination.ChessPiece = piece;
            origin.ChessPiece = null;
            piece.XCoordinate = move.NewXCoordinate;
            piece.YCoordinate = move.NewYCoordinate;
            piece.HasMovedSinceStart = true;

            if (isCastle)
            {
                var rookFromX = move.NewXCoordinate > origin.XCoordinate ? 8u : 1u;
                var rookToX = move.NewXCoordinate > origin.XCoordinate ? move.NewXCoordinate - 1 : move.NewXCoordinate + 1;
                var rookFrom = Blocks.First(b => b.XCoordinate == rookFromX && b.YCoordinate == origin.YCoordinate);
                var rookTo = Blocks.First(b => b.XCoordinate == rookToX && b.YCoordinate == origin.YCoordinate);
                rookTo.ChessPiece = rookFrom.ChessPiece;
                rookFrom.ChessPiece = null;
                rookTo.ChessPiece.XCoordinate = rookToX;
                rookTo.ChessPiece.YCoordinate = origin.YCoordinate;
                rookTo.ChessPiece.HasMovedSinceStart = true;
            }

            if (piece.PieceName.IsIn(PieceNames.Of().Pawn) && (move.NewYCoordinate == 8 || move.NewYCoordinate == 1))
                piece.PieceName = PromotionName(move.PromotionPieceName);

            EnPassantTargetX = doublePawnMove ? origin.XCoordinate : 0;
            EnPassantTargetY = doublePawnMove ? (origin.YCoordinate + move.NewYCoordinate) / 2 : 0;
        }

        private bool HasBothKings() => Blocks.Count(b => b.ChessPiece != null && b.ChessPiece.PieceName.Text == "King" && b.ChessPiece.PieceColor.YAxisDirectionIsUp) > 0 &&
            Blocks.Count(b => b.ChessPiece != null && b.ChessPiece.PieceName.Text == "King" && !b.ChessPiece.PieceColor.YAxisDirectionIsUp) > 0;

        private static PieceName PromotionName(string name)
        {
            switch (name?.ToLowerInvariant())
            {
                case "rook": return PieceNames.Of().Rook;
                case "bishop": return PieceNames.Of().Bishop;
                case "night":
                case "knight": return PieceNames.Of().Night;
                default: return PieceNames.Of().Queen;
            }
        }

        #endregion
    }
}

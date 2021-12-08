using Chess.Domain.DomianModel.ChessModel.Entities;
using Chess.Domain.DomianModel.ChessModel.Specifications;
using Chess.Domain.DomianModel.ChessModel.ValueObjects.LookupValueObjects;
using Microservice.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess.Domain.DomianModel.ChessModel.ValueObjects
{
    public class Move : ValueObject
    {
        #region Constructors

        public Move(
            ChessPieceId pieceId,
            uint newXCoordinate,
            uint newYCoordinate)
        {
            PieceId = pieceId;
            NewXCoordinate = newXCoordinate;
            NewYCoordinate = newYCoordinate;
        }

        #endregion

        #region Properties

        public ChessPieceId PieceId { get; }
        public uint NewXCoordinate { get; }
        public uint NewYCoordinate { get; }

        #endregion

        #region Methods

        public PieceSpecification Specification(IReadOnlyCollection<Block> board)
        {
            var piece = board
                .First(s => s.ChessPiece.Id == PieceId)
                .ChessPiece;

            if(piece.PieceName.IsIn(PieceNames.Of().Pawn))
                return new PawnSpecification(this, board);
            if (piece.PieceName.IsIn(PieceNames.Of().Night))
                return new NightSpecification(this, board);
            if (piece.PieceName.IsIn(PieceNames.Of().Bishop))
                return new BishopSpecification(this, board);
            if (piece.PieceName.IsIn(PieceNames.Of().Rook))
                return new RookSpecification(this, board);
            if (piece.PieceName.IsIn(PieceNames.Of().Queen))
                return new QueenSpecification(this, board);
            if (piece.PieceName.IsIn(PieceNames.Of().King))
                return new KingSpecification(this, board);

            throw new ArgumentException("Could not find a matching piece specification");
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return PieceId;
            yield return NewXCoordinate;
            yield return NewYCoordinate;
        }

        #endregion

    }
}

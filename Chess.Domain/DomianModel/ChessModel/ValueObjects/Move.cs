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
            uint newYCoordinate,
            string promotionPieceName = null)
        {
            PieceId = pieceId;
            NewXCoordinate = newXCoordinate;
            NewYCoordinate = newYCoordinate;
            PromotionPieceName = promotionPieceName;
        }

        #endregion

        #region Properties

        public ChessPieceId PieceId { get; }
        public uint NewXCoordinate { get; }
        public uint NewYCoordinate { get; }
        public string PromotionPieceName { get; }

        #endregion

        #region Methods

        public PieceSpecification Specification(IReadOnlyCollection<Block> board, uint enPassantX = 0, uint enPassantY = 0)
        {
            var piece = board
                .First(s => s.ChessPiece?.Id == PieceId)
                .ChessPiece;

            if(piece.PieceName.IsIn(PieceNames.Of().Pawn))
                return new PawnSpecification(board, enPassantX, enPassantY);
            if (piece.PieceName.IsIn(PieceNames.Of().Night))
                return new NightSpecification(board, enPassantX, enPassantY);
            if (piece.PieceName.IsIn(PieceNames.Of().Bishop))
                return new BishopSpecification(board, enPassantX, enPassantY);
            if (piece.PieceName.IsIn(PieceNames.Of().Rook))
                return new RookSpecification(board, enPassantX, enPassantY);
            if (piece.PieceName.IsIn(PieceNames.Of().Queen))
                return new QueenSpecification(board, enPassantX, enPassantY);
            if (piece.PieceName.IsIn(PieceNames.Of().King))
                return new KingSpecification(board, enPassantX, enPassantY);

            throw new ArgumentException("Could not find a matching piece specification");
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return PieceId;
            yield return NewXCoordinate;
            yield return NewYCoordinate;
            yield return PromotionPieceName;
        }

        #endregion

    }
}

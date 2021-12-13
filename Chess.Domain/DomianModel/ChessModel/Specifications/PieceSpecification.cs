using Chess.Arithmetic;
using Chess.Domain.DomianModel.ChessModel.Entities;
using Chess.Domain.DomianModel.ChessModel.ValueObjects;
using Microservice.Framework.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Domain.DomianModel.ChessModel.Specifications
{
    public abstract class PieceSpecification : Specification<Move>
    {
        #region Constructors

        protected PieceSpecification(
            IReadOnlyCollection<Block> board)
        {
            Board = board;
        }

        #endregion

        #region Properties

        protected Move Move { get; set; }

        protected ChessPiece Piece => Board
                .First(s => s.ChessPiece?.Id == Move.PieceId)
                .ChessPiece;

        protected double Distance => ChessMath.Distance(
                    Move.NewXCoordinate,
                    Piece.XCoordinate,
                    Move.NewYCoordinate,
                    Piece.YCoordinate);

        protected bool IsValidDiagonal => Math.Abs(Slope) > SlopeThreshold;

        protected double Slope => ChessMath.Slope(
                    Move.NewYCoordinate,
                    Piece.YCoordinate,
                    Move.NewXCoordinate,
                    Piece.XCoordinate);

        protected bool IsMovingLOrR => Math.Abs(DirectionX_Axis) > 0;

        protected int DirectionX_Axis => ChessMath.DirectionX_Axis(
                    (int)Move.NewXCoordinate,
                    (int)Piece.XCoordinate);

        protected int DirectionY_Axis => ChessMath.DirectionY_Axis(
                    (int)Move.NewYCoordinate,
                    (int)Piece.YCoordinate);

        protected virtual double SlopeThreshold { get { return 1; } }

        public IReadOnlyCollection<Block> Board { get; }

        #endregion
    }
}

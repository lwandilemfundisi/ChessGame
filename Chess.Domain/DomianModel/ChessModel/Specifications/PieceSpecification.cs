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

        protected ChessPiece Piece
        {
            get
            {
                return Board
                .First(s => s.ChessPiece?.Id == Move.PieceId)
                .ChessPiece;
            }
        }

        protected double Distance
        {
            get
            {
                return ChessMath.Distance(
                    Move.NewXCoordinate,
                    Piece.XCoordinate,
                    Move.NewYCoordinate,
                    Piece.YCoordinate);
            }
        }

        protected bool IsDiagonal
        {
            get 
            {
                return Math.Abs(Slope) > 0;
            }
        }

        protected double Slope
        {
            get
            {
                return ChessMath.Slope(
                    Move.NewYCoordinate,
                    Piece.YCoordinate,
                    Move.NewXCoordinate,
                    Piece.XCoordinate);
            }
        }

        protected bool IsMovingLOrR
        {
            get
            {
                return Math.Abs(DirectionX_Axis) > 0;
            }
        }

        protected int DirectionX_Axis
        {
            get
            {
                return ChessMath.DirectionX_Axis(
                    (int)Move.NewXCoordinate,
                    (int)Piece.XCoordinate);
            }
        }

        protected int DirectionY_Axis
        {
            get
            {
                return ChessMath.DirectionY_Axis(
                    (int)Move.NewYCoordinate,
                    (int)Piece.YCoordinate);
            }
        }

        public IReadOnlyCollection<Block> Board { get; }

        #endregion
    }
}

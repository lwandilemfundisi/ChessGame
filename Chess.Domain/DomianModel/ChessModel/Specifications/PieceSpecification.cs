using Chess.Arithmetic;
using Chess.Domain.DomianModel.ChessModel.Entities;
using Chess.Domain.DomianModel.ChessModel.ValueObjects;
using Chess.Domain.DomianModel.ChessModel.ValueObjects.LookupValueObjects;
using Microservice.Framework.Common;
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

        protected bool IsValidUpOrDown => Math.Abs(DirectionY_Axis) 
            <= NumberOfBlocksAllowedToMoveVertical;

        protected bool IsValidDiagonal => Math.Abs(Slope) == SlopeThreshold
            && Math.Floor(Distance) == NumberOfBlocksAllowedToMoveDiagonal
            && IsCorrectDirection;

        protected bool IsDiagonalDown => IsMovingDown && IsMovingLOrR;

        protected bool IsDiagonalUp => IsMovingUp && IsMovingLOrR;

        protected double Slope => ChessMath.Slope(
                    Move.NewYCoordinate,
                    Piece.YCoordinate,
                    Move.NewXCoordinate,
                    Piece.XCoordinate);

        protected bool IsMovingLOrR => IsMovingLeft || IsMovingRight;

        protected bool IsMovingUp => DirectionY_Axis > 0;

        protected bool IsMovingDown => DirectionY_Axis < 0;

        protected bool IsMovingRight => DirectionX_Axis > 0;

        protected bool IsMovingLeft => DirectionX_Axis < 0;

        protected bool IsDiagonalUpLeft => IsMovingUp && IsMovingLeft;

        protected bool IsDiagonalUpRight => IsMovingUp && IsMovingRight;

        protected bool IsDiagonalDownLeft => IsMovingDown && IsMovingLeft;

        protected bool IsDiagonalDownRight => IsMovingDown && IsMovingRight;

        protected int DirectionX_Axis => ChessMath.DirectionX_Axis(
                    (int)Move.NewXCoordinate,
                    (int)Piece.XCoordinate);

        protected int DirectionY_Axis => ChessMath.DirectionY_Axis(
                    (int)Move.NewYCoordinate,
                    (int)Piece.YCoordinate);

        protected Block DestinationBlock => Board
                .First(b => b.XCoordinate == Move.NewXCoordinate 
                && b.YCoordinate == Move.NewYCoordinate);

        protected bool DestinationIsOccupied => 
            DestinationBlock.ChessPiece.IsNotNull();

        protected ChessPiece PieceAtDestination =>
            DestinationBlock.ChessPiece;

        protected bool PieceAtDestinationIsOpponent => 
            !PieceAtDestination.PieceColor.IsIn(Piece.PieceColor);

        protected virtual double SlopeThreshold => 1;

        protected virtual int NumberOfBlocksAllowedToMoveDiagonal => 0;

        protected virtual bool IsCorrectDirection => true;

        protected virtual bool CanLeapOverPieces => false;

        protected virtual int NumberOfBlocksAllowedToMoveVertical => 7;

        protected bool IsLeaping => CheckLeaping();

        public IReadOnlyCollection<Block> Board { get; }

        #endregion

        #region Methods

        private bool CheckLeaping()
        {
            if (CanLeapOverPieces)
                return true;

            var x_origin = (int)Piece.XCoordinate;
            var y_origin = (int)Piece.YCoordinate;
            var x_des = (int)Move.NewXCoordinate;
            var y_des = (int)Move.NewYCoordinate;

            var x_decider = (x_origin - x_des) == 0 ? 0 : (x_origin - x_des) / (x_origin - x_des);
            var y_decider = (y_des - y_origin) == 0 ? 0 : (y_des - y_origin) / (Piece.PieceColor.IsIn(Colors.Of().Black) ? -(y_des - y_origin) : (y_des - y_origin));

            var x_control = x_origin + x_decider;
            var y_control = y_origin + y_decider;
            do
            {
                if (x_control < 1 || y_control < 1 || x_control > 8 || y_control > 8)
                    return false;

                if (x_control == x_des && y_control == y_des)
                    return false;

                if (Board.First(b => b.XCoordinate == x_control && b.YCoordinate == y_control).ChessPiece.IsNotNull())
                    return true;

                x_control = x_control + x_decider;
                y_control = y_control + y_decider;

            }
            while (true);
        }

        #endregion
    }
}

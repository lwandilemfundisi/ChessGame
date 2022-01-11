using Chess.Arithmetic;
using Chess.Domain.DomianModel.ChessModel.Entities;
using Chess.Domain.DomianModel.ChessModel.ValueObjects;
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

            if (IsValidDiagonal)
            {
                var x_origin = Piece.XCoordinate;
                var y_origin = Piece.YCoordinate;
                var x_des = Move.NewXCoordinate;
                var y_des = Move.NewYCoordinate;

                if (IsDiagonalUpLeft)
                {
                    do
                    {
                        x_origin--;
                        y_origin++;

                        return Board
                            .First(b =>
                                b.XCoordinate == x_origin
                                && b.YCoordinate == y_origin)
                            .ChessPiece.IsNull();
                    }
                    while (x_origin >= x_des && y_origin <= y_des);
                }
                else if (IsDiagonalUpRight)
                {
                    do
                    {
                        x_origin++;
                        y_origin++;

                        return Board
                            .First(b =>
                                b.XCoordinate == x_origin
                                && b.YCoordinate == y_origin)
                            .ChessPiece.IsNull();
                    }
                    while (x_origin <= x_des && y_origin <= y_des);
                }
                else if (IsDiagonalDownLeft)
                {
                    do
                    {
                        x_origin--;
                        y_origin--;

                        return Board
                            .First(b =>
                                b.XCoordinate == x_origin
                                && b.YCoordinate == y_origin)
                            .ChessPiece.IsNull();
                    }
                    while (x_origin >= x_des && y_origin >= y_des);
                }
                else if (IsDiagonalDownRight)
                {
                    do
                    {
                        x_origin++;
                        y_origin--;

                        return Board
                            .First(b =>
                                b.XCoordinate == x_origin
                                && b.YCoordinate == y_origin)
                            .ChessPiece.IsNull();
                    }
                    while (x_origin <= x_des && y_origin >= y_des);
                }
            }
            else
            {
                if (IsMovingUp)
                {
                    for(int y_origin = (int)Piece.YCoordinate + 1; y_origin <= Move.NewYCoordinate; y_origin++)
                    {
                        if (Board.First(b => b.XCoordinate == Piece.XCoordinate && b.YCoordinate == y_origin).ChessPiece.IsNotNull())
                            return true;
                    }
                }
                else if (IsMovingDown)
                {
                    for (int y_origin = (int)Piece.YCoordinate - 1; y_origin >= Move.NewYCoordinate; y_origin--)
                    {
                        if (Board.First(b => b.XCoordinate == Piece.XCoordinate && b.YCoordinate == y_origin).ChessPiece.IsNotNull())
                            return true;
                    }
                }
                else if (IsMovingLeft)
                {
                    for (int x_origin = (int)Piece.XCoordinate - 1; x_origin >= Move.NewXCoordinate; x_origin--)
                    {
                        if (Board.First(b => b.YCoordinate == Piece.YCoordinate && b.XCoordinate == x_origin).ChessPiece.IsNotNull())
                            return true;
                    }
                }
                else if(IsMovingRight)
                {
                    for (int x_origin = (int)Piece.XCoordinate + 1; x_origin <= Move.NewXCoordinate; x_origin++)
                    {
                        if (Board.First(b => b.YCoordinate == Piece.YCoordinate && b.XCoordinate == x_origin).ChessPiece.IsNotNull())
                            return true;
                    }
                }
            }

            return false;
        }

        #endregion
    }
}

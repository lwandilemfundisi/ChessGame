using Chess.Domain.DomianModel.ChessModel.Entities;
using Chess.Domain.DomianModel.ChessModel.ValueObjects;
using Microservice.Framework.Domain.Rules.Notifications;
using System.Collections.Generic;

namespace Chess.Domain.DomianModel.ChessModel.Specifications
{
    public class PawnSpecification : PieceSpecification
    {
        #region Constructors

        public PawnSpecification(
            IReadOnlyCollection<Block> board)
            : base(board)
        {

        }

        #endregion

        #region Virtual Methods

        protected override int NumberOfBlocksAllowedToMoveVertical => Piece.HasMovedSinceStart ? 1 : 2;

        protected override int NumberOfBlocksAllowedToMoveDiagonal => 1;

        protected override bool IsCorrectDirection => (Piece.PieceColor.YAxisDirectionIsUp && (IsDiagonalUp || IsMovingUp))
            || (!Piece.PieceColor.YAxisDirectionIsUp && (IsDiagonalDown || IsMovingDown));

        protected override Notification IsNotSatisfiedBecause(Move obj)
        {
            var notification = base
                .IsNotSatisfiedBecause(obj);

            if (notification.HasErrors)
                return notification;

            if (IsMovingLOrR)
            {
                if (!IsValidDiagonal)
                    notification.AddError(new Message($"move was invalid for a {Piece.PieceName.Text}."));
                else if (!DestinationIsOccupied)
                    notification.AddError(new Message($"move was invalid for a {Piece.PieceName.Text}. " +
                        $"You can only move diagonal when capturing opponent's piece or perfoming enpassant!"));
                else if (!PieceAtDestinationIsOpponent)
                    notification.AddError(new Message($"move was invalid for a {Piece.PieceName.Text}. " +
                        "You cannot attempt to capture your own piece!"));
            }
            else
            {
                if (!IsCorrectDirection)
                    notification.AddError(new Message($"move was invalid for a {Piece.PieceName.Text}. " +
                        $"You can only move {(Piece.PieceColor.YAxisDirectionIsUp ? "up" : "down")}!"));
                else if (DestinationIsOccupied)
                    notification.AddError(new Message($"move was invalid for a {Piece.PieceName.Text}. " +
                        $"You cannot occupy a block that already has a piece on!"));
                else if (!IsValidUpOrDown)
                    notification.AddError(new Message($"move was invalid for a {Piece.PieceName.Text}. " +
                        $"You cannot move {(Piece.PieceColor.YAxisDirectionIsUp ? "up" : "down")} more than {NumberOfBlocksAllowedToMoveVertical}"));
            }

            return notification;
        }

        #endregion

        #region Private Classes

        private class EnPassentSpecification : PieceSpecification
        {
            #region Constructors

            public EnPassentSpecification(
                IReadOnlyCollection<Block> board)
                : base(board)
            {

            }

            #endregion

            #region Virtual Methods

            protected override Notification IsNotSatisfiedBecause(Move obj)
            {
                var notification = base
                    .IsNotSatisfiedBecause(obj);

                if (notification.HasErrors)
                    return notification;

                if (!IsValidDiagonal)
                    notification.AddError(new Message($"move was invalid for a {Piece.PieceName.Text}." +
                        $"You cannot perform enpassant! Invalid diagonal move!"));
                else if(DestinationIsOccupied)
                    notification.AddError(new Message($"move was invalid for a {Piece.PieceName.Text}. " +
                        $"You cannot perform enpassant! Destination block is occupied!"));




                return notification;
            }

            #endregion
        }

        #endregion
    }
}

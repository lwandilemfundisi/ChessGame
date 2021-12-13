﻿using Chess.Arithmetic;
using Chess.Domain.DomianModel.ChessModel.Entities;
using Chess.Domain.DomianModel.ChessModel.ValueObjects;
using Microservice.Framework.Common;
using Microservice.Framework.Domain;
using Microservice.Framework.Domain.Rules.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        protected override int NumberOfBlocksAllowedToMoveDiagonal => 1;

        protected override bool IsCorrectDirection => (Piece.PieceColor.YAxisDirectionIsUp && IsDiagonalUp) 
            || (!Piece.PieceColor.YAxisDirectionIsUp && IsDiagonalDown);

        protected override Notification IsNotSatisfiedBecause(Move obj)
        {
            Move = obj;
            var notification = Notification.CreateEmpty();

            if(IsMovingLOrR)
            {
                if(!IsValidDiagonal)
                    notification.AddError(new Message($"move was invalid for a {Piece.PieceName.Text}. " +
                        $"You cannot move left or right"));
                else
                {
                    if(!DestinationIsOccupied)
                        notification.AddError(new Message($"move was invalid for a {Piece.PieceName.Text}. " +
                            $"You can only move diagonal when capturing opponent's piece or perfoming enpassant"));
                    else
                    {

                    }
                }
            }
            else
            {
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
                throw new NotImplementedException();
            }

            #endregion
        }

        #endregion
    }
}

using Chess.Arithmetic;
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
        protected override Notification IsNotSatisfiedBecause(Move obj)
        {
            Move = obj;
            var notification = Notification.CreateEmpty();

            if(Piece.IsNull())
                notification.AddError(new Message("move was invalid. You need to select a block with a piece to move!"));
            else
            {
                if(IsMovingLOrR)
                {
                    if(!IsValidDiagonal)
                        notification.AddError(new Message($"move was invalid. You cannot move left or right using a {Piece.PieceName.Text}"));
                    else
                    {
                        //Are we capturing a piece or perfoming enpassant?
                    }
                }
                else
                {
                }
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

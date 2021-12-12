using Chess.Domain.DomianModel.ChessModel.Entities;
using Chess.Domain.DomianModel.ChessModel.ValueObjects;
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
            Move move,
            IReadOnlyCollection<Block> board)
            : base(move, board)
        {

        }

        #endregion

        #region Virtual Methods
        protected override Notification IsNotSatisfiedBecause(ChessPiece obj)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private Classes

        private class EnPassentSpecification : PieceSpecification
        {
            #region Constructors

            public EnPassentSpecification(
                Move move,
                IReadOnlyCollection<Block> board)
                : base(move, board)
            {

            }

            #endregion

            #region Virtual Methods

            protected override Notification IsNotSatisfiedBecause(ChessPiece obj)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        #endregion
    }
}

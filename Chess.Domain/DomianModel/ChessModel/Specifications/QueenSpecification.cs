﻿using Chess.Domain.DomianModel.ChessModel.Entities;
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
    public class QueenSpecification : PieceSpecification
    {
        #region Constructors

        public QueenSpecification(
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
}

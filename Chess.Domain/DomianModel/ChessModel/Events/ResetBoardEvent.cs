using Microservice.Framework.Domain.Events;
using Microservice.Framework.Domain.Events.AggregateEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Domain.DomianModel.ChessModel.Events
{
    [EventVersion("MovedPiece", 1)]
    public class ResetBoardEvent : AggregateEvent<Board, BoardId>
    {
        #region Constructors

        public ResetBoardEvent()
        {
        }

        #endregion
    }

}

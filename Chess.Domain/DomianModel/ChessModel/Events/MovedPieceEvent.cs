using Chess.Domain.DomianModel.ChessModel.ValueObjects;
using Microservice.Framework.Domain.Events;
using Microservice.Framework.Domain.Events.AggregateEvents;

namespace Chess.Domain.DomianModel.ChessModel.Events
{
    [EventVersion("MovedPiece", 1)]
    public class MovedPieceEvent : AggregateEvent<Board, BoardId>
    {
        #region Constructors

        public MovedPieceEvent(Move move)
        {
            Move = move;
        }

        #endregion

        #region Properties

        public Move Move { get; }

        #endregion
    }
}

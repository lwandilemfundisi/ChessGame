using Chess.Domain.DomianModel.ChessModel.Entities;
using Chess.Domain.DomianModel.ChessModel.Rules;
using Chess.Domain.DomianModel.ChessModel.ValueObjects;
using Microservice.Framework.Domain;
using Microservice.Framework.Domain.Rules.Notifications;
using System.Collections.Generic;

namespace Chess.Domain.DomianModel.ChessModel.Specifications
{
    public abstract class PieceSpecification : Specification<Move>
    {
        protected PieceSpecification(IReadOnlyCollection<Block> board, uint enPassantX = 0, uint enPassantY = 0)
        {
            Board = board;
            EnPassantX = enPassantX;
            EnPassantY = enPassantY;
        }

        protected Move Move { get; set; }
        public IReadOnlyCollection<Block> Board { get; }
        protected uint EnPassantX { get; }
        protected uint EnPassantY { get; }

        protected override Notification IsNotSatisfiedBecause(Move obj)
        {
            Move = obj;
            var notification = Notification.CreateEmpty();
            var error = ChessRules.ValidateMove(Board, obj, EnPassantX, EnPassantY);
            if (error != null) notification.AddError(new Message(error));
            return notification;
        }
    }
}

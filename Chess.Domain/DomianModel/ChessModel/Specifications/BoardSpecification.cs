using Microservice.Framework.Domain;
using Microservice.Framework.Domain.Rules.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Domain.DomianModel.ChessModel.Specifications
{
    public class BoardSpecification : Specification<Board>
    {
        private readonly bool _requireKings;

        public BoardSpecification(bool requireKings = false) => _requireKings = requireKings;
        #region Virtual Methods

        protected override Notification IsNotSatisfiedBecause(Board obj)
        {
            var notification = Notification.CreateEmpty();
            var blocks = obj.Blocks;
            if (blocks == null || blocks.Count != 64)
            {
                notification.AddError(new Message("A chess board must contain exactly 64 squares."));
                return notification;
            }

            if (blocks.Any(b => b.XCoordinate < 1 || b.XCoordinate > 8 || b.YCoordinate < 1 || b.YCoordinate > 8) ||
                blocks.Select(b => new { b.XCoordinate, b.YCoordinate }).Distinct().Count() != 64)
                notification.AddError(new Message("Board squares must have unique coordinates from 1 to 8."));

            if (blocks.Any(b => b.ChessPiece != null &&
                (b.ChessPiece.XCoordinate != b.XCoordinate || b.ChessPiece.YCoordinate != b.YCoordinate)))
                notification.AddError(new Message("Each piece must have the same coordinates as its square."));

            var whiteKings = blocks.Count(b => b.ChessPiece != null && b.ChessPiece.PieceName.Text == "King" && b.ChessPiece.PieceColor.YAxisDirectionIsUp);
            var blackKings = blocks.Count(b => b.ChessPiece != null && b.ChessPiece.PieceName.Text == "King" && !b.ChessPiece.PieceColor.YAxisDirectionIsUp);
            if ((whiteKings == 0) != (blackKings == 0) || whiteKings > 1 || blackKings > 1)
                notification.AddError(new Message("If a king is present, the board must contain exactly one king per side."));
            if (_requireKings && (whiteKings != 1 || blackKings != 1))
                notification.AddError(new Message("A playable chess game must contain exactly one king for each side."));

            return notification;
        }

        #endregion
    }
}

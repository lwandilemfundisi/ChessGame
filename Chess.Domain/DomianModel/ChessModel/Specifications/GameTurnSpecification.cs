using Chess.Domain.DomianModel.ChessModel.Rules;
using Chess.Domain.DomianModel.ChessModel.ValueObjects;
using Microservice.Framework.Domain;
using Microservice.Framework.Domain.Rules.Notifications;
using System.Linq;

namespace Chess.Domain.DomianModel.ChessModel.Specifications
{
    public sealed class GameTurnSpecification : Specification<Move>
    {
        private readonly Board _board;

        public GameTurnSpecification(Board board) { _board = board; }

        protected override Notification IsNotSatisfiedBecause(Move move)
        {
            var result = Notification.CreateEmpty();
            if (_board.GameStatus != ChessRules.InProgress && _board.GameStatus != ChessRules.Check)
                result.AddError(new Message("The game has ended."));

            var piece = _board.Blocks.FirstOrDefault(b => b.ChessPiece?.Id == move.PieceId)?.ChessPiece;
            if (piece == null)
                result.AddError(new Message("The selected piece is not on this board."));
            var hasBothKings = _board.Blocks.Any(b => b.ChessPiece != null && ChessRules.Name(b.ChessPiece) == "King" && ChessRules.IsWhite(b.ChessPiece)) &&
                _board.Blocks.Any(b => b.ChessPiece != null && ChessRules.Name(b.ChessPiece) == "King" && !ChessRules.IsWhite(b.ChessPiece));
            if (hasBothKings && piece != null && ChessRules.IsWhite(piece) != _board.WhiteToMove)
                result.AddError(new Message("It is not this piece's turn to move."));
            return result;
        }
    }
}

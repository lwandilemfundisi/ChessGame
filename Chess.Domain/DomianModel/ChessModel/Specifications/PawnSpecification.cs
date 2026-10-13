using Chess.Domain.DomianModel.ChessModel.Entities;
using System.Collections.Generic;

namespace Chess.Domain.DomianModel.ChessModel.Specifications
{
    /// <summary>
    /// Uses the shared piece rules for pawn movement. Pawn-specific geometry,
    /// direction, capture, and initial double-step validation live there.
    /// En passant requires previous-move state and is not inferred from the board.
    /// </summary>
    public class PawnSpecification : PieceSpecification
    {
        public PawnSpecification(IReadOnlyCollection<Block> board, uint enPassantX = 0, uint enPassantY = 0)
            : base(board, enPassantX, enPassantY)
        {
        }
    }
}

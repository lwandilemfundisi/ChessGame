using Chess.Domain.DomianModel.ChessModel.Entities;
using Chess.Domain.DomianModel.ChessModel.ValueObjects;
using Microservice.Framework.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Domain.DomianModel.ChessModel.Specifications
{
    public abstract class PieceSpecification : Specification<ChessPiece>
    {
        #region Constructors

        protected PieceSpecification(
            Move move, 
            IReadOnlyCollection<Block> board)
        {
            Move = move;
            Board = board;
        }

        #endregion

        #region Properties

        public Move Move { get; }
        public IReadOnlyCollection<Block> Board { get; }

        #endregion
    }
}

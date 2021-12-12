using Chess.Domain.DomianModel.ChessModel.ValueObjects;
using Chess.Domain.DomianModel.ChessModel.ValueObjects.LookupValueObjects;
using Microservice.Framework.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Domain.DomianModel.ChessModel.Entities
{
    public class ChessPiece : Entity<ChessPieceId>
    {
        #region Properties

        public PieceName PieceName { get; set; }

        public Color PieceColor { get; set; }

        public uint XCoordinate { get; set; }

        public uint YCoordinate { get; set; }

        public bool HasMovedSinceStart { get; set; }

        #endregion
    }
}
